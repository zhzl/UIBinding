using System;
using System.Linq;
using System.Reflection;

namespace UIBinding
{
    /// <summary>
    /// 反射相关扩展
    /// </summary>
    public static class ReflectionExtensions
    {
        public static bool IsGenericType(this System.Type type, System.Type genericType)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition().Equals(genericType));
        }

        public static bool IsGenericSubclassOf(this System.Type type, System.Type supperType)
        {
            return (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition().Equals(supperType));
        }

        public static System.Type GetGenericArgument(this System.Type type, int index)
        {
            var arguments = type.GetGenericArguments();
            return index < arguments.Length ? arguments[index] : null;
        }

        public static bool HasAttribute<T>(this System.Type type) where T : System.Attribute
        {
            var attrs = type.GetCustomAttributes(true);
            return attrs != null && attrs.Any(a => a.GetType().Equals(typeof(T)));
        }

        /// <summary>
        /// 当前属性是否存在指定特性
        /// </summary>
        public static bool HasAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            var attrs = property.GetCustomAttributes();
            return attrs != null && attrs.Any(a => a.GetType().Equals(typeof(T)));
        }

        public static bool HasAttribute<T>(this System.Reflection.FieldInfo field)
        {
            var attrs = field.GetCustomAttributes(true);
            return attrs != null && attrs.Any(a => a.GetType().Equals(typeof(T)));
        }

        public static T GetAttribute<T>(this System.Type type) where T : System.Attribute
        {
            var attrs = type.GetCustomAttributes(true);
            return attrs.FirstOrDefault(a => a.GetType().Equals(typeof(T))) as T;
        }

        public static T GetAttribute<T>(this System.Reflection.FieldInfo field) where T : System.Attribute
        {
            var attrs = field.GetCustomAttributes(true);
            return attrs.FirstOrDefault(a => a.GetType().Equals(typeof(T))) as T;
        }

        public static T[] GetAttributes<T>(this System.Reflection.FieldInfo field) where T : System.Attribute
        {
            var attrs = field.GetCustomAttributes(true);
            return attrs.Where(a => a.GetType().Equals(typeof(T))).OfType<T>().ToArray();
        }
    }
}
