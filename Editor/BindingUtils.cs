using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Text.RegularExpressions;
using UnityEditor;

namespace UIBinding
{
    /// <summary>
    /// 绑定工具类
    /// </summary>
    public static class BindingUtils
    {
        /// <summary>
        /// 获取类型名字的友好显示
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FriendlyName(this System.Type type)
        {
            using (var p = new CSharpCodeProvider())
            {
                var r = new CodeTypeReference(type);
                return p.GetTypeOutput(r).Replace($"{type.Namespace}.", "");
            }
        }

        /// <summary>
        /// 查找 guid 关联的脚本文件
        /// </summary>
        public static bool TryGetMonoScript(string guid, out MonoScript script)
        {
            script = null;

            // 没有 guid，表示空对象
            if (string.IsNullOrEmpty(guid))
                return true;

            // 有 guid，但是找不到资源路径，说明引用丢失
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
                return false;

            // 有路径，但是找不到资源，说明引用丢失
            script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script == null)
                return false;

            return true;
        }

        /// <summary>
        /// 通过属性类型获取Popup类型
        /// </summary>
        /// <returns></returns>
        public static Type GetPopupType(Type vmPropertyType)
        {
            if (vmPropertyType.IsGenericType(typeof(ObservableList<>)))
            {
                return vmPropertyType.GetGenericTypeDefinition();
            }
            else if (vmPropertyType.IsSubclassOf(typeof(ViewModelBase)))
            {
                return typeof(ViewModelBase);
            }

            return vmPropertyType;
        }

        /// <summary>
        /// 获取 Class Type
        /// </summary>
        public static Type GetClass(MonoScript script)
        {
            string pattern = @"namespace\s+(\w+)\s*{[^}]*\bclass\s+([\w.]+)\b";
            return LookupType(script, pattern);
        }

        /// <summary>
        /// 获取 Enum Type
        /// </summary>
        public static Type GetEnum(MonoScript script)
        {
            string pattern = @"namespace\s+(\w+)\s*{[^}]*\benum\s+([\w.]+)\b";
            return LookupType(script, pattern);
        }

        /// <summary>
        /// 先尝试通过 MonoScript 的 GetClass 查找
        /// 如果没找到则通过反射查找
        /// </summary>
        /// <param name="script">MonoScript 对象</param>
        /// <param name="pattern">正则表达式，用于查找命名空间和类型名字</param>
        /// <returns></returns>
        private static Type LookupType(MonoScript script, string pattern)
        {
            var klass = script.GetClass();
            if (klass != null)
            {
                return klass;
            }

            Match match = Regex.Match(script.text, pattern);

            if (match.Success)
            {
                string namespaceName = match.Groups[1].Value;
                string typeName = match.Groups[2].Value;

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var type = assembly.GetType($"{namespaceName}.{typeName}");
                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}
