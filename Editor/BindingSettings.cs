using System.Threading;
using UnityEditor;
using UnityEngine;

namespace UIBinding
{
    public enum SourcePath
    {
        RelativeToScriptAssemblies,
        RelativeToHybridCLRData,
        RelativeToAssets,
    }

    /// <summary>
    /// 绑定配置
    /// </summary>
    public class BindingSettings : ScriptableObject
    {
        /// <summary>
        /// 自定义PR
        /// </summary>
        public MonoScript customPR;

        /// <summary>
        /// 注入相对路径
        /// </summary>
        public SourcePath injectPath;

        /// <summary>
        /// 注入DLL
        /// </summary>
        public string injectDll;

        static int wasSettingsDirCreated = 0;
        const string SETTINGS_ASSET_PATH = "Assets/Editor/BindingSettings.asset";

        public static BindingSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<BindingSettings>(SETTINGS_ASSET_PATH);

            if (settings == null)
            {
                settings = FindBindingSettings();
            }

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<BindingSettings>();

                if (!AssetDatabase.IsValidFolder("Assets/Editor") && Interlocked.Exchange(ref wasSettingsDirCreated, 1) == 0)
                    AssetDatabase.CreateFolder("Assets", "Editor");

                if (Interlocked.Exchange(ref wasSettingsDirCreated, 1) == 0)
                    AssetDatabase.CreateAsset(settings, SETTINGS_ASSET_PATH);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return settings;
        }

        static BindingSettings FindBindingSettings()
        {
            string typeSearchString = " t:BindingSettings";
            string[] guids = AssetDatabase.FindAssets(typeSearchString);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var settings = AssetDatabase.LoadAssetAtPath<BindingSettings>(path);
                if (settings != null)
                    return settings;
            }
            return null;
        }
    }
}
