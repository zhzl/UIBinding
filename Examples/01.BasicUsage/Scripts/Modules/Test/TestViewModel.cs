using UIBinding;

namespace BasicUsage
{
    public class TestViewModel : ViewModelBase
    {
#if !DISABLE_UIBINDING_INJECT
        [Notify] public int ClickCount { get; set; }
        [Notify] public SimpleCommand ClickCommand { get; set; }
        [Notify] public ObservableList<TestViewScrollCellViewModel> ScrollList { get; set; }
#else
        [Notify] public int ClickCount { get => Get<int>(); set => Set(value); }
        [Notify] public SimpleCommand ClickCommand { get => Get<SimpleCommand>(); set => Set(value); }
        [Notify] public ObservableList<TestViewScrollCellViewModel> ScrollList { get => Get<ObservableList<TestViewScrollCellViewModel>>(); set => Set(value); }
#endif
    }
}
