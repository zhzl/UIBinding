using System;
using UnityEngine;

namespace BasicUsage
{
    public static class UIManager
    {
        public static void OpenView<T>(string path) where T : UIView
        {
            ResManager.LoadAssetAsync<GameObject>(path, prefab =>
            {
                var root = GameObject.Find("Canvas");
                var view = GameObject.Instantiate(prefab, root.transform);
                view.name = view.name.Replace("(Clone)", "");
                var uiview = (UIView)Activator.CreateInstance(typeof(T));
                uiview.OnInitialize(view.transform);
                uiview.OnOpen();
            });
        }
    }
}
