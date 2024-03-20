using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// 一个 BindingItem 对应一个 ViewModel 的属性
    /// </summary>
    [Serializable]
    public class BindingItem
    {
        public string vmPropertyName;
        public List<GameObject> uiObjects;
        public List<Component> uiComponents;
        public List<string> uiProperties;
        public List<ISyncProperty> syncProperties;
    }

    /// <summary>
    /// 数据绑定配置
    /// </summary>
    public class BindingConfig : MonoBehaviour
    {
        /// <summary>
        /// ViewModel 文件的 guid
        /// </summary>
        [SerializeField]
        private string vmKlassGuid;
        /// <summary>
        /// 绑定的 ViewModel 文件
        /// </summary>
        public string vmKlassName;
        /// <summary>
        /// 绑定的属性数据
        /// </summary>
        public List<BindingItem> itemList;
    }
}
