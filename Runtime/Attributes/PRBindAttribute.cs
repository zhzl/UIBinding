using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PRBindAttribute : Attribute
    {
        /// <summary>
        /// 该属性可以绑定的数据类型
        /// </summary>
        public List<Type> DataTypes { get; private set; }

        /// <summary>
        /// 该属性绑定的目标组件类型
        /// </summary>
        public Type Component { get; private set; }

        public PRBindAttribute(Type component, params Type[] dataTypes)
        {
            if (!typeof(Component).IsAssignableFrom(component))
            {
                throw new BindingException("Binding component type must inherit from UnityEngine.Component");
            }

            Component = component;
            DataTypes = new List<Type>();
            DataTypes.AddRange(dataTypes);
        }
    }
}
