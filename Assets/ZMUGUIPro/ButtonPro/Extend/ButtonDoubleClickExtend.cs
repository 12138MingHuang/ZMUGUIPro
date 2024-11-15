using UnityEngine;
using UnityEngine.Events;

namespace ZM.UGUIPro
{
    /// <summary>
    /// ButtonDoubleClickExtend 类用于实现按钮的双击功能拓展。
    /// </summary>
    [System.Serializable]
    public class ButtonDoubleClickExtend
    {
        // 是否启用双击功能
        [SerializeField]
        private bool m_IsUseDoubleClick;

        [Header("有效时间")]
        // 双击的最大时间间隔
        [SerializeField]
        private float m_ClickInterval;

        // 记录上一次按下按钮的时间
        [SerializeField]
        private float m_LastPointerDownTime;

        // 双击触发事件
        [SerializeField]
        private ButtonClickEvent m_ButtonClickedEvent;

        /// <summary>
        /// 在按下按钮时调用，检查是否在双击的有效时间间隔内。
        /// </summary>
        public void OnPointerDown()
        {
            // 检查时间间隔是否小于设定的双击间隔时间
            m_LastPointerDownTime = Time.realtimeSinceStartup - m_LastPointerDownTime < m_ClickInterval ? 0 : Time.realtimeSinceStartup;

            // 如果满足双击条件，则触发双击事件
            if (m_LastPointerDownTime == 0)
            {
                m_ButtonClickedEvent?.Invoke();
            }
        }

        /// <summary>
        /// 添加回调事件监听器。
        /// </summary>
        /// <param name="callback">要添加的回调函数</param>
        /// <param name="doubleClickInterval">双击的时间间隔</param>
        public void AddListener(UnityAction callback, float doubleClickInterval)
        {
            m_IsUseDoubleClick = true;
            m_ClickInterval = doubleClickInterval;
            m_ButtonClickedEvent.AddListener(callback);
        }
    }
}
