using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    /// <summary>
    /// ButtonProBase 是对 Unity 原生 Button 的扩展类，支持双击、长按、缩放和音效功能。
    /// </summary>
    [System.Serializable]
    public class ButtonProBase : Button, IUpdateSelectedHandler
    {
        // 双击功能扩展
        [SerializeField]
        private ButtonDoubleClickExtend m_ButtonDoubleClickExtend = new ButtonDoubleClickExtend();

        // 长按功能扩展
        [SerializeField]
        private ButtonLongPressExtend m_ButtonLongPressExtend = new ButtonLongPressExtend();

        // 单击事件
        [SerializeField]
        private ButtonClickEvent m_ButtonClickEvent = new ButtonClickEvent();

        // 按压缩放功能扩展
        [SerializeField]
        private ButtonPressScaleExtend m_ButtonScaleExtend = new ButtonPressScaleExtend();

        // 点击音效扩展
        [SerializeField]
        private ButtonAudioExtend m_ButtonAudioExtend = new ButtonAudioExtend();

        // 按下时的鼠标位置
        private Vector2 m_PressPos;

        // 是否处于按下状态
        private bool mIsPreass;

        // 存储当前的 PointerEventData
        private PointerEventData mPointerEventData;

        // 在 PointerUp 时的回调
        public Action OnPointerUpListener;

        /// <summary>
        /// 重写 Unity 的 OnPointerClick，用于处理按钮点击事件。
        /// </summary>
        /// <param name="eventData">PointerEventData 对象</param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick();
        }

        /// <summary>
        /// 处理按钮点击逻辑。
        /// </summary>
        public void OnPointerClick()
        {
            if (m_ButtonAudioExtend != null)
            {
                // 播放音效并触发点击事件
                if (m_ButtonAudioExtend.OnButtonClick() && interactable)
                {
                    onClick?.Invoke();
                }
            }
            else
            {
                if (interactable)
                    onClick?.Invoke();
            }
        }

        /// <summary>
        /// 按下按钮时触发。
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            m_PressPos = eventData.position;
            mIsPreass = true;
            mPointerEventData = eventData;

            // 各种功能扩展处理
            m_ButtonLongPressExtend?.OnPointerDown();
            m_ButtonDoubleClickExtend?.OnPointerDown();
            m_ButtonScaleExtend?.OnPointerDown(transform, interactable);
            m_ButtonAudioExtend.OnPointerDown(transform);
        }

        /// <summary>
        /// 抬起按钮时触发。
        /// </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            mPointerEventData = null;
            mIsPreass = false;

            // 判断按下位置和抬起位置是否接近
            if (interactable && Mathf.Abs(Vector2.Distance(m_PressPos, eventData.position)) < 10)
            {
                m_ButtonClickEvent?.Invoke();
                m_ButtonAudioExtend.OnPointerUp(this);
            }

            // 自定义的 PointerUp 回调
            OnPointerUpListener?.Invoke();
            m_ButtonScaleExtend?.OnPointerUp(transform, interactable);

            // 清除选中状态
            EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        /// 在更新选择时触发（如长按功能）。
        /// </summary>
        public void OnUpdateSelected(BaseEventData eventData)
        {
            m_ButtonLongPressExtend?.OnUpdateSelected();
        }

        /// <summary>
        /// 添加长按事件监听器。
        /// <param name="callback">回调函数</param>
        /// <param name="longPressTime">长按时间,默认2f 秒</param> 
        /// </summary>
        public void AddButtonLongListener(UnityAction callback, float longPressTime = 2f)
        {
            m_ButtonLongPressExtend.AddListener(callback, longPressTime);
        }

        /// <summary>
        /// 添加双击事件监听器。
        /// <param name="callback">回调函数</param>
        /// <param name="doubleClickInterval">双击间隔时间,默认0.25f 秒</param> 
        /// </summary>
        public void AddButtonDoubleClickListener(UnityAction callback, float doubleClickInterval = 0.25f)
        {
            m_ButtonDoubleClickExtend.AddListener(callback, doubleClickInterval);
        }

        /// <summary>
        /// 添加单击事件监听器。
        /// </summary>
        public void AddButtonClick(UnityAction callback)
        {
            m_ButtonClickEvent.AddListener(callback);
        }

        /// <summary>
        /// 移除单击事件监听器。
        /// </summary>
        public void RemoveButtonClick(UnityAction callback)
        {
            m_ButtonClickEvent.RemoveListener(callback);
        }

        /// <summary>
        /// 在应用程序失去焦点时调用，处理按下状态的重置。
        /// </summary>
        public void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                // 如果仍处于按下状态，则触发 PointerUp
                if (mIsPreass && mPointerEventData != null)
                {
                    OnPointerUp(mPointerEventData);
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 在编辑器模式下验证属性设置。
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            // 如果启用按压缩放功能，则禁用 Button 的默认过渡效果
            if (m_ButtonScaleExtend.UsePressScale)
            {
                transition = Transition.None;
            }
        }
#endif
    }

    /// <summary>
    /// 自定义点击事件类。
    /// </summary>
    [System.Serializable]
    public class ButtonClickEvent : UnityEvent
    {
        public ButtonClickEvent() { }
    }
}
