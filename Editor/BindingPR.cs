using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UIBinding
{
    /// <summary>
    /// 内置PR与自定义PR集合
    /// </summary>
    public class BindingPR
    {
        public static readonly string None = "None";
        public static readonly int NoneValue = 0;

        private readonly Dictionary<int, Type> prCompDic = new Dictionary<int, Type>();
        private readonly Dictionary<Type, PRPopup> popupDic = new Dictionary<Type, PRPopup>();
        private readonly Dictionary<string, int> enumDic = new Dictionary<string, int>();
        private int currValue;

        public BindingPR()
        {
            currValue = NoneValue;
            enumDic.Add(None, NoneValue);

            // 内置PR
            var internalPRFields = typeof(PR).GetFields();
            ParsePRFields(internalPRFields);

            // 自定义PR
            var customPR = BindingSettings.GetOrCreateSettings().customPR;
            if (customPR != null)
            {
                var klass = BindingUtils.GetEnum(customPR);
                if (klass != null)
                {
                    var customPRFields = klass.GetFields();
                    ParsePRFields(customPRFields);
                }
            }

            // 构建 PREnum
            foreach (var item in popupDic.Values)
            {
                item.Build();
            } 
        }

        public bool TryGetPopup(Type type, out PRPopup prEnum)
        {
            return popupDic.TryGetValue(type, out prEnum);
        }

        public bool TryGetComponent(int enumValue, out Type component)
        {
            return prCompDic.TryGetValue(enumValue, out component);
        }

        public int ToValue(string display)
        {
            if (enumDic.TryGetValue(display, out var value))
            {
                return value;
            }
            return -1;
        }

        private void ParsePRFields(FieldInfo[] prFields)
        {
            for (int i = 0; i < prFields.Length; i++)
            {
                var field = prFields[i];
                var bind = field.GetAttribute<PRBindAttribute>();
                if (bind == null) continue;

                if (enumDic.ContainsKey(field.Name))
                {
                    UnityEngine.Debug.LogError($"绑定属性 {field.Name} 已存在！");
                    continue;
                }
                currValue++;
                enumDic.Add(field.Name, currValue);
                prCompDic.Add(currValue, bind.Component);

                for (int j = 0; j < bind.DataTypes.Count; j++)
                {
                    var type = bind.DataTypes[j];
                    if (!popupDic.TryGetValue(type, out var prPopup))
                    {
                        prPopup = new PRPopup();
                        prPopup.Add(NoneValue, None);
                        popupDic.Add(type, prPopup);
                    }

                    prPopup.Add(currValue, field.Name);
                }
            }
        }
    }

    public class PRPopup
    {
        private readonly Dictionary<int, string> enumRevert = new Dictionary<int, string>();
        public string[] displayList;
        public int[] valueList;

        public void Add(int value, string display)
        {
            enumRevert.Add(value, display);
        }

        public void Build()
        {
            displayList = enumRevert.Values.ToArray();
            valueList = enumRevert.Keys.ToArray();
        }

        public string ToDisplay(int value)
        {
            enumRevert.TryGetValue(value, out var display);
            return display;
        }
    }
}
