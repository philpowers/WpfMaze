namespace MazeWpf.Interface
{
    using System;
    using System.Threading.Tasks;


    public interface IViewModelFactory
    {
        Task<TViewModel> CreateViewModelAsync<TViewModel>(Action<TViewModel> fnConfigure = null) where TViewModel : class, IViewModel;
    }
}