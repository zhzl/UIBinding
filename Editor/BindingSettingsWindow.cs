using UIBinding;
using UnityEditor;

namespace UIBindingEditor
{
    public class BindingSettingsWindow : EditorWindow
    {
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
            }
        }
    }
}
