namespace UIBinding
{
    public interface IAssetLoader
    {
        void LoadAssetAsync<T>(string path, System.Action<T> onComplete) where T : UnityEngine.Object;
    }
}
