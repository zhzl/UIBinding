using System;
using System.Collections.Generic;
using UnityEditor;

namespace UIBindingEditor
{
    public class BindingSettingsWindow : EditorWindow
    {
        private const string DISABLE_UIBINDING_INJECT = "DISABLE_UIBINDING_INJECT";

        private Editor editor;

        [MenuItem("Tools/UIBinding/Settings")]
        public static void OpenSettingsWindow()
        {
            var window = EditorWindow.GetWindow<BindingSettingsWindow>("Binding Settings");
            var settings = BindingSettings.GetOrCreateSettings();
            window.editor = Editor.CreateEditor(settings);
        }

        private void OnGUI()
        {
            if (editor != null)
            {
                editor.OnInspectorGUI();
                DrawInjectSwitch();
            }
        }

        private void DrawInjectSwitch()
        {
            var oldDisableInject = IsDisableInject();
            var newDisableInject = EditorGUILayout.Toggle($"Disable inject", oldDisableInject);
            if (newDisableInject != oldDisableInject)
            {
                SetDisableInject(newDisableInject);
            }
        }

        private void SetDisableInject(bool disable)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var defineArr = defines.Split(';');
            if (disable && Array.IndexOf(defineArr, DISABLE_UIBINDING_INJECT) < 0)
            {
                var defineList = new List<string>(defineArr);
                defineList.Add(DISABLE_UIBINDING_INJECT);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(';', defineList));
            }
            else if (!disable && Array.IndexOf(defineArr, DISABLE_UIBINDING_INJECT) >= 0)
            {
                var defineList = new List<string>(defineArr);
                defineList.Remove(DISABLE_UIBINDING_INJECT);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(';', defineList));
            }
        }

        private bool IsDisableInject()
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return defines.Contains(DISABLE_UIBINDING_INJECT, StringComparison.Ordinal);
        }
    }
}
