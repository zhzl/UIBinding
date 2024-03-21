using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public enum PR
    {
        None,

        /// <summary>
        /// 位置
        /// </summary>
        [PRBind(typeof(RectTransform), typeof(Vector2), typeof(Vector3))]
        Position,

        /// <summary>
        /// 大小
        /// </summary>
        [PRBind(typeof(RectTransform), typeof(Vector2), typeof(Vector3))]
        Size,

        /// <summary>
        /// 缩放
        /// </summary>
        [PRBind(typeof(RectTransform), typeof(Vector2), typeof(Vector3))]
        Scale,

        /// <summary>
        /// 可见性
        /// </summary>
        [PRBind(typeof(Transform), typeof(bool))]
        Visible,

        /// <summary>
        /// CanvasGroup 透明度
        /// </summary>
        [PRBind(typeof(CanvasGroup), typeof(float))]
        Alpha,

        /// <summary>
        /// 文本内容
        /// </summary>
        [PRBind(typeof(Text), typeof(int), typeof(float), typeof(string))]
        Text,

        /// <summary>
        /// Image sprite
        /// </summary>
        [PRBind(typeof(Image), typeof(string))]
        Image,

        /// <summary>
        /// 图片填充率
        /// </summary>
        [PRBind(typeof(Image), typeof(float))]
        ImageFillAmount,

        /// <summary>
        /// 图片透明度
        /// </summary>
        [PRBind(typeof(Image), typeof(float))]
        ImageAlpha,

        /// <summary>
        /// 图片颜色
        /// </summary>
        [PRBind(typeof(Image), typeof(Color))]
        ImageColor,

        /// <summary>
        /// RawImage texture
        /// </summary>
        [PRBind(typeof(RawImage), typeof(string))]
        RawImage,

        /// <summary>
        /// 输入框
        /// </summary>
        [PRBind(typeof(InputField), typeof(string))]
        Input,

        /// <summary>
        /// 按钮点击命令
        /// </summary>
        [PRBind(typeof(Button), typeof(SimpleCommand))]
        ButtonClickCommand,

        /// <summary>
        /// 设置列表数量
        /// </summary>
        [PRBind(typeof(ListView), typeof(int))]
        ListViewCount,

        /// <summary>
        /// 刷新指定列表cell
        /// </summary>
        [PRBind(typeof(ListView), typeof(int))]
        ListCellRefresh,

        /// <summary>
        /// 列表刷新回调
        /// </summary>
        [PRBind(typeof(ListView), typeof(SimpleCommand))]
        ListViewUpdateCommand,

        /// <summary>
        /// 列表数据源
        /// </summary>
        [PRBind(typeof(ListView), typeof(ObservableList<>))]
        ListViewDataSource,

        /// <summary>
        /// 嵌套ViewModel
        /// </summary>
        [PRBind(typeof(Transform), typeof(ViewModelBase))]
        SubViewModel,

        /// <summary>
        /// 滑动条进度
        /// </summary>
        [PRBind(typeof(Slider), typeof(float))]
        SliderValue,

        /// <summary>
        /// 开关
        /// </summary>
        [PRBind(typeof(Toggle), typeof(bool))]
        ToggleValue,

        /// <summary>
        /// 加载预制
        /// </summary>
        [PRBind(typeof(Transform), typeof(LoadPrefabCommand))]
        LoadPrefabCommand,
    }
}
