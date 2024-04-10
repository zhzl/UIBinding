﻿using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UIBinding
{
    public static class BindingTool
    {
        /// <summary>
        /// 查找绑定丢失的预制
        /// </summary>
        [MenuItem("Tools/UIBinding/Find bind missing prefabs", priority = 1)]
        static void FindMissingPrefabs()
        {
            BindingPR bindingPR = new BindingPR();
            var guids = AssetDatabase.FindAssets("t:Prefab");
            EditorUtility.DisplayProgressBar("Finding...", "Start check...", 0);
            int count = 0;
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                count++;
                EditorUtility.DisplayProgressBar("Finding...", $"Check prefab {prefab.name}", (float)count / guids.Length);

                var bindingConfigs = prefab.GetComponentsInChildren<BindingConfig>(true);
                if (bindingConfigs == null || bindingConfigs.Length == 0)
                    continue;

                foreach (var bindingConfig in bindingConfigs)
                {
                    CheckPerffectConfig(prefab, bindingConfig, path, bindingPR);
                }
            }

            EditorUtility.ClearProgressBar();
            UnityEngine.Debug.Log($"============ Find bind missing prefabs complete! ===========");
        }

        /// <summary>
        /// 查找预制丢失绑定的节点
        /// </summary>
        [MenuItem("GameObject/UIBinding/Find bind missing nodes")]
        [MenuItem("Assets/UIBinding/Find bind missing nodes")]
        static void FindUIParticle(MenuCommand menuCommand)
        {
            var prefab = menuCommand.context as GameObject;
            if (prefab != null)
            {
                BindingPR bindingPR = new BindingPR();
                var bindingConfigs = prefab.GetComponentsInChildren<BindingConfig>(true);
                if (bindingConfigs == null || bindingConfigs.Length == 0)
                {
                    UnityEngine.Debug.Log($"============ Find bind missing nodes complete! ===========");
                    return;
                }

                foreach (var bindingConfig in bindingConfigs)
                {
                    CheckPerffectConfig(prefab, bindingConfig, prefab.name, bindingPR);
                }
            }

            UnityEngine.Debug.Log($"============ Find bind missing nodes complete! ===========");
        }

        /// <summary>
        /// 检查绑定配置是否完整
        /// </summary>
        private static bool CheckPerffectConfig(GameObject prefab, BindingConfig bindingConfig, string path, BindingPR bindingPR)
        {
            // 检查ViewModel是否存在
            var vmKlassGuidField = typeof(BindingConfig).GetField("vmKlassGuid", BindingFlags.NonPublic | BindingFlags.Instance);
            var vmKlassGuid = vmKlassGuidField.GetValue(bindingConfig).ToString();

            if (string.IsNullOrEmpty(vmKlassGuid) || !BindingUtils.TryGetMonoScript(vmKlassGuid, out var _))
            {
                UnityEngine.Debug.LogError($"[Bind missing]: {path}, missing node: {bindingConfig.gameObject.name}");
                return false;
            }

            foreach (var bindingItem in bindingConfig.itemList)
            {
                for (int i = 0; i < bindingItem.uiObjects.Count; i++)
                {
                    var uiObject = bindingItem.uiObjects[i];
                    var uiProperty = bindingItem.uiProperties[i];
                    var uiComponent = bindingItem.uiComponents[i];

                    // 检查对象或者属性是否存在
                    if (uiObject == null || BindingPR.None.Equals(uiProperty, System.StringComparison.Ordinal))
                    {
                        UnityEngine.Debug.LogError($"[Bind missing]: {path}, missing node: {bindingConfig.gameObject.name}");
                        return false;
                    }

                    // 检查组件是否匹配
                    var propertyValue = bindingPR.ToValue(uiProperty);
                    bindingPR.TryGetComponent(propertyValue, out var componentType);
                    var component = uiObject.GetComponent(componentType);
                    if (component == null)
                    {
                        UnityEngine.Debug.LogError($"[Bind missing]: {path}, missing node: {bindingConfig.gameObject.name}");
                        return false;
                    }

                    if (component != uiComponent)
                    {
                        UnityEngine.Debug.LogError($"[Bind missing]: {path}, missing node: {bindingConfig.gameObject.name}");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}