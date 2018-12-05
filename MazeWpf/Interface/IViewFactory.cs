namespace MazeWpf.Interface
{
    public interface IViewFactory
    {
        object CreateViewFromViewModel(IViewModel viewModel);

        void ShowModalDialog(IViewModel viewModel);
    }
}
