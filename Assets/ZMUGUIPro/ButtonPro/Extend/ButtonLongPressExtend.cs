using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ZM.UGUIPro
{
    /// <summary>
    /// ButtonLongPressExtend 类用于实现按钮的长按功能。
    /// </summary>
    [System.Serializable]
    public class ButtonLongPressExtend
    {
        // 是否启用长按功能
        [SerializeField]
        private bool m_IsUseLongPress;

        [Header("长按时间")]
        // 触发长按事件的所需持续时间
        [SerializeField]
        private float m_Duration;

        // 长按事件
        [SerializeField]
        private ButtonClickEvent m_ButtonLongPressEvent;

        // 记录按下按钮的时间
        private float m_PointerDownTime;

        /// <summary>
        /// 在按钮按下时调用，用于记录按下的开始时间。
        /// </summary>
        public void OnPointerDown()
        {
            // 记录按下按钮的实时开始时间
            m_PointerDownTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 持续检测按钮是否满足长按时间条件。
        /// </summary>
        public void OnUpdateSelected()
        {
            // 检查当前是否超过了指定的长按持续时间
            if (m_Duration >= 0 && Time.realtimeSinceStartup - m_PointerDownTime >= m_Duration)
            {
                // 触发长按事件
                m_ButtonLongPressEvent?.Invoke();
                
                // 重置选中状态，避免重复触发长按事件
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        /// <summary>
        /// 添加长按事件的监听器。
        /// </summary>
        /// <param name="callback">要添加的回调函数</param>
        public void AddListener(UnityAction callback, float longPressTime = 0.3f)
        {
            m_IsUseLongPress = true;
            m_Duration = longPressTime;
            m_ButtonLongPressEvent.AddListener(callback);
        }
    }
}
