namespace MazeWpf
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Diagnostics;

    using SimpleInjector;

    using MazeWpf.Interface;
    using MazeWpf.Extension;

    public static class Program
    {
        private static Container _container;

        [STAThread]
        public static void Main()
        {
            _container = ConfigureContainer();

            var app = new App {ShutdownMode = ShutdownMode.OnLastWindowClose};
            app.InitializeComponent();

#if DEBUG
            _container.Verify();
#endif

            var viewModelFactory = _container.GetInstance<IViewModelFactory>();

            var mainWindowViewModel = viewModelFactory.CreateViewModelAsync<MainWindowViewModel>().Result;

            var mainWindow = (MainWindow)_container.GetInstance<IViewFactory>()
                .CreateViewFromViewModel(mainWindowViewModel);

            app.Run(mainWindow);
        }

        private static Container ConfigureContainer()
        {
            var container = new Container();

            var programAssembly = AssemblyExtension.AssemblyFromType(typeof(Program));

            //
            // Convention-based registrations
            foreach (var controlType in programAssembly.GetConcreteTypesImplementing<ContentControl>()) {
                container.Register(controlType);
            }

            foreach (var viewModelType in programAssembly.GetConcreteTypesImplementing<IViewModel>()) {
                container.Register(viewModelType);
            }


            //
            // Other registrations
            container.RegisterSingleton<TypeLookup>(() => new TypeLookup(programAssembly));
            container.RegisterSingleton<IViewFactory, ViewFactory>();
            container.RegisterSingleton<IViewModelFactory, ViewModelFactory>();

            //
            // Disable support for auto-resolving concrete types
            container.ResolveUnregisteredType += (s, e) =>
            {
                if (!e.Handled && !e.UnregisteredServiceType.IsAbstract)
                {
                    throw new InvalidOperationException(
                        e.UnregisteredServiceType.ToFriendlyName() + " has not been registered.");
                }
            };

            return container;
        }
    }

    public class TypeLookup
    {
        public IReadOnlyList<(Type viewModelType, Type viewType)> ViewModelToViewMappings { get; }

        public TypeLookup(Assembly sourceAssembly)
        {
            this.ViewModelToViewMappings = CreateViewModelToViewMappings(sourceAssembly);
        }

        private static List<(Type viewModelType, Type viewType)> CreateViewModelToViewMappings(Assembly assembly)
        {
            // @todo(PQP): This should be profiled and optimized if necessary
            var viewModelTypes = assembly.GetConcreteTypesImplementing<IViewModel>().ToDictionary(t => t.Name, t => t);
            var viewTypes  = assembly.GetConcreteTypesImplementing<ContentControl>().ToDictionary(t => t.Name, t => t);

            var list = new List<(Type viewModelType, Type viewType)>();

            foreach (var vt in viewModelTypes)
            {
                if (!vt.Key.EndsWith("ViewModel")) {
                    // $@"WARNING: cannot map view from specified ViewModel type: '{viewModelTypeName}'"
                    continue;
                }

                var viewTypeName = vt.Key.Substring(0, vt.Key.Length - "ViewModel".Length);
                if (!viewTypes.TryGetValue(viewTypeName, out Type viewType))
                {
                    // ($@"WARNING: Could not find View type named '{viewTypeName}'"
                    continue;
                }

                list.Add((vt.Value, viewType));
            }

            return list;
        }
    }

    public class ViewFactory : IViewFactory
    {
        private readonly Container container;
        private readonly Dictionary<string, Type> viewTypeLookup;

        public ViewFactory(Container container, TypeLookup typeLookup)
        {
            this.container = container;

            this.viewTypeLookup = typeLookup.ViewModelToViewMappings
                .ToDictionary(kv => kv.viewModelType.Name, kv => kv.viewType);
        }

        public object CreateViewFromViewModel(IViewModel viewModel)
        {
            // @todo(PQP): Modify this to use GetViewModelToViewMappings()
            var viewModelTypeName = viewModel.GetType().Name;

            if (!viewModelTypeName.EndsWith("ViewModel")) {
                throw new ArgumentException(
                    $@"ViewFactory cannot create view from specified ViewModel type: '{viewModelTypeName}'",
                    nameof(viewModel));
            }

            if (!this.viewTypeLookup.TryGetValue(viewModelTypeName, out Type viewType))
            {
                throw new ArgumentException($@"Could not lookup View type from ViewModel named '{viewModelTypeName}'", nameof(viewModel));
            }

            var view = this.container.GetInstance(viewType);

            if (view is FrameworkElement frameworkElement) {
                frameworkElement.DataContext = viewModel;

                if (viewModel is ViewModelBase viewModelBase) {

                    if (viewModelBase.ConfigurationFunc != null && !viewModelBase.IsConfigured) {
                        viewModelBase.ConfigurationFunc();

                        if (!viewModelBase.IsConfigured) {
                            throw new InvalidOperationException($"ViewModel type {viewModel.GetType().Name} must be configured during creation!");
                        }
                    }

                    frameworkElement.Loaded += async (sender, args) => {
                        Debug.WriteLine($"{view.GetType().Name}: Loading started...");
                        if (!viewModelBase.IsPopulated) {
                            await viewModelBase.PopulateInitialContentAsync();
                            viewModelBase.IsPopulated = true;
                        }
                        Debug.WriteLine($"{view.GetType().Name}: Loading complete.");
                    };
                }
            }

            Debug.WriteLine($"{view.GetType().Name}: Returning view");
            return view;
        }

        public void ShowModalDialog(IViewModel viewModel)
        {
            var window = this.CreateViewFromViewModel(viewModel) as Window;
            if (null == window) {
                throw new InvalidOperationException($"Could not create Window from: {viewModel.GetType().Name}");
            }

            window.ShowDialog();
        }
    }
}