using UIBinding;
using UnityEngine;

namespace BasicUsage
{
    public class GameLauncher : MonoBehaviour
    {
        void Awake()
        {
            // 初始化绑定
            BindingInitializer.Initialize(new CustomSyncRegister());

            // 打开测试窗口
            UIManager.OpenView<TestView>("UI/Test/TestView");
        }
    }
}

