namespace MazeWpf
{
    using System;
    using System.Threading.Tasks;

    using SimpleInjector;

    using MazeWpf.Interface;

    public class ViewModelFactory : IViewModelFactory
    {
        private readonly Container container;

        public ViewModelFactory(Container container)
        {
            this.container = container;
        }

        public async Task<TViewModel> CreateViewModelAsync<TViewModel>(Action<TViewModel> fnConfigure = null) where TViewModel : class, IViewModel
        {
            var viewModel = this.container.GetInstance<TViewModel>();

            var viewModelBase = viewModel as ViewModelBase;
            if (null != viewModelBase) {
                await viewModelBase.ConstructAsync();

                if (fnConfigure != null) {
                    viewModelBase.ConfigurationFunc = () => fnConfigure(viewModel);
                }
            }

            return viewModel;
        }
    }
}
