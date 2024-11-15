using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZM.UGUIPro
{
    /// <summary>
    /// ButtonPressScaleExtend 类用于实现按钮按下时的缩放效果。
    /// </summary>
    [System.Serializable]
    public class ButtonPressScaleExtend
    {
        // 是否启用按下缩放功能
        [SerializeField]
        private bool m_IsUsePressScale = true;
        public bool UsePressScale { get { return m_IsUsePressScale; } }

        [Header("默认缩放")]
        // 按钮正常状态下的缩放大小
        [SerializeField]
        private Vector3 m_NormalScale = Vector3.one;

        [Header("按下缩放")]
        // 按钮按下状态下的缩放大小
        [SerializeField]
        private Vector3 m_PressScale = new Vector3(1.1f, 1.1f, 1.1f);

        /// <summary>
        /// 按下按钮时调用，应用按下缩放效果。
        /// </summary>
        /// <param name="trans">按钮的 Transform 对象</param>
        /// <param name="interactable">按钮是否可交互</param>
        public void OnPointerDown(Transform trans, bool interactable)
        {
            // 如果启用了缩放功能且按钮可交互，则设置为按下缩放大小
            if (m_IsUsePressScale && interactable)
            {
                trans.localScale = m_PressScale;
            }
        }

        /// <summary>
        /// 抬起按钮时调用，恢复到正常缩放效果。
        /// </summary>
        /// <param name="trans">按钮的 Transform 对象</param>
        /// <param name="interactable">按钮是否可交互</param>
        public void OnPointerUp(Transform trans, bool interactable)
        {
            // 如果启用了缩放功能且按钮可交互，则恢复到正常缩放大小
            if (m_IsUsePressScale && interactable)
            {
                trans.localScale = m_NormalScale;
            }
        }
    }
}
