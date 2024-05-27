using UIBinding;

namespace BasicUsage
{
    public class TestViewScrollCellViewModel : ViewModelBase
    {
#if !DISABLE_UIBINDING_INJECT
        [Notify] public string Index { get; set; }
        [Notify] public SimpleCommand CellClickCommand { get; set; }
#else
        [Notify] public string Index { get => Get<string>(); set => Set(value); }
        [Notify] public SimpleCommand CellClickCommand { get => Get<SimpleCommand>(); set => Set(value); }
#endif
    }
}
