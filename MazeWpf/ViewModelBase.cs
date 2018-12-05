namespace MazeWpf
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using MazeWpf.Interface;

    public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        public Action ConfigurationFunc { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public bool IsConfigured { get; private set; }
        public bool IsPopulated { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => this._isBusy;
            protected set => this.SetAndRaise(ref this._isBusy, value);
        }

        protected ViewModelBase(bool requiresConfiguration)
        {
            // USAGE: ViewModel construction/initialization is a 3-step process:
            //
            //    1. The ViewModel is created and, when running in the host program, all specified dependencies are
            //       injected into its constructor. When running in the Designer, all the constructor-specified
            //       dependencies will be 'null', so any initialization that requires the use of one of those objects
            //       should happen in ConstructAsync() instead.
            //
            //      ... then, when NOT running in the Designer:
            //
            //    2. ConstructAsync() is called to give the ViewModel a chance to do any
            //       initialization that requires access to one of the constructor-injected dependencies. The derived
            //       class does NOT need to override 'ConstructAsync' otherwise.
            //
            //    3. Configure() is called to give the ViewModel a chance to SET any runtime data or dependencies.
            //       This MUST be implemented by the ViewModel class if 'requiresConfiguration' is set to 'true' and the
            //       derived ViewModel class MUST call the base.Configure() BEFORE it does any of its own processing.
            //       Note that this is a SYNCHRONOUS call and so long-running tasks should be performed here. Generally,
            //       the View should just save any specified dependencies for later use (such as by the
            //       PopulateInitialContentAsync() call)
            //
            //    4. PopulateInitialContentAsync() is the last method called as part of the ViewModel's initialization,
            //       after the actual View is fully loaded
            //

            if (!requiresConfiguration) {
                this.IsConfigured = true;
            }
        }

        public virtual Task ConstructAsync()
            {
            // USAGE: Derived classes can use this method to do any sort of initialization that requires calling out
            // to one of the dependencies that were injected into its constructor. That sort of initialization should
            // NOT happen in the constructor directly, for two main reasons:
            //
            //     1. The constructor cannot ensure that any asynchronous activity is performed correctly
            //     2. It's possible/likely that the Designer will construct the ViewModel class, in which case all of
            //        the injected dependencies will be unavailable (they will probably be 'null'). The
            //        'ConstructAsync' call will never happen from the Designer, so those calls are safe to make here.
            //
            // NOTE: derived class does NOT need to call this if it overrides it
            //

            return Task.CompletedTask;
        }


        public virtual Task RefreshContentAsync()
        {
            return Task.CompletedTask;
        }

        protected void Configure()
        {
            // USAGE: This will be called after a ViewModel class is constructed and initialized (i.e., the
            // 'ConstructAsync' method has been called), but BEFORE the ViewModel is made available to any View.
            // Derived classes MUST implement their own overload of Configure IF the 'requiresConfiguration'
            // flag has been set. Additionally, anyone who creates a ViewModel MUST call the overloaded version of
            // Configure(...) - which should generally be done in the call to the ViewModelFactory's
            // 'CreateViewModelAsync()'
            //
            // Derived classes should use this method to provide any runtime data / dependencies that cannot be
            // injected into the constructor.

            // NOTE: dervived class MUST call this base method (before its own logic) from its overload
            this.IsConfigured = true;
        }

        public virtual Task PopulateInitialContentAsync()
        {
            // USAGE: This will be called after the ViewModel has been fully configured and the View is ready to show
            // data
            //
            // NOTE: derived class does NOT need to call this if it overrides it
            //
            this.IsPopulated = true;
            return Task.CompletedTask;
        }

        protected virtual void SetAndRaise<T>(ref T backingField, T newValue, [CallerMemberName] string propertyName = null)
        {
            backingField = newValue;
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected async Task RunLongTask(Func<Task> longTask)
        {
            this.IsBusy = true;
            await longTask();
            this.IsBusy = false;
        }
    }
}
