using UnityEngine;

namespace UIBinding
{
    public class ButtonClickCommandArgs : ICommandArgs
    {
        /// <summary>
        /// 按钮 Transform
        /// </summary>
        public UITransform transform;

        /// <summary>
        /// 列表参数，只有按钮属于某个列表才有值
        /// </summary>
        public ListViewCommandArgs listViewArgs;
    }
}
