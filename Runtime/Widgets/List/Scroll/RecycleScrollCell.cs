using UnityEngine;

namespace UIBinding
{
    public class RecycleScrollCell : MonoBehaviour
    {
        /// <summary>
        /// 当前Cell的Index
        /// </summary>
        public int index;

        /// <summary>
        /// RectTransform
        /// </summary>
        private RectTransform rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }
                return rectTransform;
            }
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
