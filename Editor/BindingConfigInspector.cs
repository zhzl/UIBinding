using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UIBinding
{
    [CustomEditor(typeof(BindingConfig))]
    public class BindingConfigInspector : Editor
    {
        private SerializedProperty vmKlassGuid;
        private SerializedProperty vmKlassName;
        private SerializedProperty itemList;
        private List<BindingItem> tempItemList;
        private Dictionary<string, System.Type> vmPropertyTypeMapping;
        private Dictionary<string, string> vmPropertyFriendlyNames;
        private BindingPR bindingPR;

        private void OnEnable()
        {
            vmKlassGuid = serializedObject.FindProperty("vmKlassGuid");
            vmKlassName = serializedObject.FindProperty("vmKlassName");
            itemList = serializedObject.FindProperty("itemList");
            tempItemList = new List<BindingItem>();
            vmPropertyTypeMapping = new Dictionary<string, System.Type>();
            vmPropertyFriendlyNames = new Dictionary<string, string>();
            bindingPR = new BindingPR(); 
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool missScript = false;
            if (!BindingUtils.TryGetMonoScript(vmKlassGuid.stringValue, out var script))
            {
                missScript = true;
            }

            var scriptObject = EditorGUILayout.ObjectField("ViewModel", script, typeof(MonoScript), false);
            if (scriptObject != null)
            {
                script = scriptObject as MonoScript;

                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(script, out string guid, out long localid))
                {
                    vmKlassGuid.stringValue = guid;
                }

                var klass = BindingUtils.GetClass(script);
                if (klass == null)
                {
                    EditorGUILayout.HelpBox($"MonoScript({script.name}) cant find class. ", MessageType.Error);
                    return;
                }

                vmKlassName.stringValue = klass.FullName;

                SyncProperties(klass);
                DrawProperty();
            }
            else
            {
                if (!missScript)
                {
                    vmKlassName.stringValue = string.Empty;
                    vmKlassGuid.stringValue = string.Empty;
                }
            }

            if (missScript)
            {
                EditorGUILayout.HelpBox($"{target.name} ViewModel({vmKlassName}) reference missing. ", MessageType.Error);
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SyncProperties(System.Type klass)
        {
            var vmIndex = 0;
            var rebuild = false;
            var klassProperties = klass.GetProperties();
            if (klassProperties.Length == 0)
            {
                itemList.arraySize = 0;
                return;
            }

            for (int i = 0; i < klassProperties.Length; i++)
            {
                var p = klassProperties[i];
                if (!p.HasAttribute<NotifyAttribute>())
                    continue;

                if (vmIndex + 1 > tempItemList.Count)
                {
                    rebuild = true;
                    break;
                }

                if (!p.Name.Equals(tempItemList[vmIndex].vmPropertyName, System.StringComparison.Ordinal))
                {
                    rebuild = true;
                    break;
                }

                vmIndex++;
            }

            if (rebuild)
            {
                var targetItemList = (target as BindingConfig).itemList;
                tempItemList.Clear();
                vmPropertyTypeMapping.Clear();
                vmPropertyFriendlyNames.Clear();
                for (int i = 0; i < klassProperties.Length; i++)
                {
                    var p = klassProperties[i];
                    if (!p.HasAttribute<NotifyAttribute>())
                        continue;

                    tempItemList.Add(CreateBindingItem(targetItemList, p.Name));
                    vmPropertyTypeMapping.Add(p.Name, p.PropertyType);
                    vmPropertyFriendlyNames.Add(p.Name, $"{p.Name} ({p.PropertyType.FriendlyName()})");
                }

                itemList.arraySize = tempItemList.Count;
                for (int i = 0; i < tempItemList.Count; i++)
                {
                    var item = tempItemList[i];
                    var element = itemList.GetArrayElementAtIndex(i);
                    var elementVmPropertyName = element.FindPropertyRelative("vmPropertyName");
                    var elementUiProperties = element.FindPropertyRelative("uiProperties");
                    var elementUiObjects = element.FindPropertyRelative("uiObjects");
                    var elementUiComponents = element.FindPropertyRelative("uiComponents");

                    elementVmPropertyName.stringValue = item.vmPropertyName;

                    elementUiProperties.arraySize = item.uiProperties.Count;
                    for (int j = 0; j < item.uiProperties.Count; j++)
                    {
                        var innerElement = elementUiProperties.GetArrayElementAtIndex(j);
                        innerElement.stringValue = item.uiProperties[j];
                    }

                    elementUiObjects.arraySize = item.uiObjects.Count;
                    for (int j = 0; j < item.uiObjects.Count; j++)
                    {
                        var innerElement = elementUiObjects.GetArrayElementAtIndex(j);
                        innerElement.objectReferenceValue = item.uiObjects[j];
                    }

                    elementUiComponents.arraySize = item.uiComponents.Count;
                    for (int j = 0; j < item.uiComponents.Count; j++)
                    {
                        var innerElement = elementUiComponents.GetArrayElementAtIndex(j);
                        innerElement.objectReferenceValue = item.uiComponents[j];
                    }
                }
            }
        }

        private BindingItem CreateBindingItem(List<BindingItem> itemList, string vmPropertyName)
        {
            var newItem = new BindingItem();
            newItem.vmPropertyName = vmPropertyName;
            newItem.uiProperties = new List<string>();
            newItem.uiObjects = new List<GameObject>();
            newItem.uiComponents = new List<Component>();

            if (itemList != null)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    var binding = itemList[i];
                    if (vmPropertyName.Equals(binding.vmPropertyName, System.StringComparison.Ordinal))
                    {
                        newItem.uiProperties.AddRange(binding.uiProperties);
                        newItem.uiObjects.AddRange(binding.uiObjects);
                        newItem.uiComponents.AddRange(binding.uiComponents);
                        break;
                    }
                }
            }

            // 至少有一个值
            if (newItem.uiProperties.Count == 0)
            {
                newItem.uiProperties.Add(BindingPR.None);
                newItem.uiObjects.Add(null);
                newItem.uiComponents.Add(null);
            }

            return newItem;
        }

        private void DrawProperty()
        {
            for (int i = 0; i < itemList.arraySize; i++)
            {
                var element = itemList.GetArrayElementAtIndex(i);

                var elementVmPropertyName = element.FindPropertyRelative("vmPropertyName");
                var elementUiProperties = element.FindPropertyRelative("uiProperties");
                var elementUiObjects = element.FindPropertyRelative("uiObjects");
                var elementUiComponents = element.FindPropertyRelative("uiComponents");

                var vmPropertyType = vmPropertyTypeMapping[elementVmPropertyName.stringValue];
                var vmPropertyFriendlyName = vmPropertyFriendlyNames[elementVmPropertyName.stringValue];

                var uiPropertyCount = elementUiProperties.arraySize;
                for (int j = 0; j < uiPropertyCount; j++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var vmPropertyDesc = j == 0 ? vmPropertyFriendlyName : " ";
                    EditorGUILayout.PrefixLabel(vmPropertyDesc);

                    var uiObject = elementUiObjects.GetArrayElementAtIndex(j);
                    var uiProperty = elementUiProperties.GetArrayElementAtIndex(j);
                    var uiComponent = elementUiComponents.GetArrayElementAtIndex(j);

                    var bindingObject = (GameObject)EditorGUILayout.ObjectField(uiObject.objectReferenceValue, typeof(GameObject), true);

                    var oldUiProperty = BindingPR.NoneValue;
                    var newUiProperty = BindingPR.NoneValue;

                    if (!string.IsNullOrEmpty(uiProperty.stringValue))
                    {
                        oldUiProperty = bindingPR.ToValue(uiProperty.stringValue);
                        if (oldUiProperty == -1)
                        {
                            UnityEngine.Debug.LogError($"vmPropertyDesc 未找到属性 {uiProperty.stringValue}, 已自动移除");
                            uiProperty.stringValue = BindingPR.None;
                            oldUiProperty = 0;
                        }
                    }

                    var popupType = BindingUtils.GetPopupType(vmPropertyType);
                    if (bindingPR.TryGetPopup(popupType, out var popup))
                    {
                        newUiProperty = EditorGUILayout.IntPopup(oldUiProperty, popup.displayList, popup.valueList);
                        if (System.Array.IndexOf(popup.valueList, newUiProperty) == -1)
                        {
                            newUiProperty = BindingPR.NoneValue;
                        }
                    }
                    else
                    {
                        newUiProperty = EditorGUILayout.IntPopup(BindingPR.NoneValue, new string[] { BindingPR.None }, new int[] { BindingPR.NoneValue });
                    }

                    if (bindingObject == null)
                    {
                        uiObject.objectReferenceValue = null;
                        uiComponent.objectReferenceValue = null;
                        uiProperty.stringValue = BindingPR.None;
                    }

                    if (bindingObject == null && newUiProperty != BindingPR.NoneValue)
                    {
                        EditorUtility.DisplayDialog("错误", "请先指定预制", "关闭");
                        break;
                    }

                    if (bindingObject != null)
                    {
                        var newUiObject = EditorUtility.InstanceIDToObject(bindingObject.GetInstanceID());
                        if (newUiObject != uiObject.objectReferenceValue || newUiProperty != oldUiProperty)
                        {
                            uiObject.objectReferenceValue = newUiObject;
                            uiComponent.objectReferenceValue = null;
                            uiProperty.stringValue = popup.ToDisplay(newUiProperty);
                            UpdateBindingProperty(uiProperty, uiComponent, newUiProperty, bindingObject);
                        }
                    }

                    if (GUILayout.Button("+", GUILayout.Width(18)))
                    {
                        elementUiProperties.InsertArrayElementAtIndex(j);
                        elementUiObjects.InsertArrayElementAtIndex(j);
                        elementUiComponents.InsertArrayElementAtIndex(j);

                        // clear data
                        var insertObject = elementUiObjects.GetArrayElementAtIndex(j + 1);
                        var insertProperty = elementUiProperties.GetArrayElementAtIndex(j + 1);
                        var insertComponent = elementUiComponents.GetArrayElementAtIndex(j + 1);
                        insertObject.objectReferenceValue = null;
                        insertComponent.objectReferenceValue = null;
                        insertProperty.stringValue = BindingPR.None;

                        break;
                    }

                    if (GUILayout.Button("-", GUILayout.Width(18)))
                    {
                        if (uiPropertyCount == 1)
                        {
                            EditorUtility.DisplayDialog("错误", "至少保留一条属性", "关闭");
                            break;
                        }
                        elementUiProperties.DeleteArrayElementAtIndex(j);
                        elementUiObjects.DeleteArrayElementAtIndex(j);
                        elementUiComponents.DeleteArrayElementAtIndex(j);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void UpdateBindingProperty(SerializedProperty uiProperty, SerializedProperty uiComponent, int newUiProperty, GameObject bindingObject)
        {
            if (newUiProperty != BindingPR.NoneValue)
            {
                bindingPR.TryGetComponent(newUiProperty, out var componentType);
                var component = bindingObject.GetComponent(componentType);
                if (component != null)
                {
                    uiComponent.objectReferenceValue = EditorUtility.InstanceIDToObject(component.GetInstanceID());
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", $"{bindingObject.name}不存在组件{componentType.Name}", "关闭");
                    uiComponent.objectReferenceValue = null;
                    uiProperty.stringValue = BindingPR.None;
                }
            }
        }
    }
}
