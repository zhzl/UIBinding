using System;
using UnityEngine;

namespace BasicUsage
{
    public static class ResManager
    {
        public static void LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
        {
            var request = Resources.LoadAsync<T>(path);
            request.completed += op => onComplete((op as ResourceRequest).asset as T);
        }
    }
}
