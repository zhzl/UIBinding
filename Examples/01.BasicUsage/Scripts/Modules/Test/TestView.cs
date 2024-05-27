using System;
using UIBinding;

namespace BasicUsage
{
    public class TestView : UIView
    {
        SimpleCommand clickCellCommand;

        public override void OnOpen()
        {
            var vm = GetViewModel<TestViewModel>();
            vm.ClickCount = 0;
            vm.ClickCommand = new SimpleCommand(ClickCountCommand);

            clickCellCommand = new SimpleCommand(ClickCellCommand);
            vm.ScrollList = new ObservableList<TestViewScrollCellViewModel>();
            for (int i = 0; i < 100; i++)
            {
                vm.ScrollList.Add(new TestViewScrollCellViewModel()
                {
                    Index = i.ToString(),
                    CellClickCommand = clickCellCommand,
                });
            }
        }

        private void ClickCellCommand(ICommandArgs args)
        {
            var clickArgs = args as ButtonClickCommandArgs;
            UnityEngine.Debug.Log($"click cell: {clickArgs.listViewArgs.index}");
        }

        private void ClickCountCommand(ICommandArgs args)
        {
            var vm = GetViewModel<TestViewModel>();
            vm.ClickCount++;
        }
    }
}
