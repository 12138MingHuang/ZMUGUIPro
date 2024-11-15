using UnityEngine;

namespace ZM.UGUIPro
{
    [System.Serializable]
    public class ButtonAudioExtend
    {
        // 是否启用点击音效
        [SerializeField]
        private bool m_IsUseClickAudio;

        /// <summary>
        /// 在按下按钮时调用，用于处理按钮按下的行为。
        /// 目前仅传入按钮的 Transform 参数，未实现具体行为。
        /// </summary>
        /// <param name="trans">按钮的 Transform 对象</param>
        public void OnPointerDown(Transform trans)
        {

        }

        /// <summary>
        /// 在按钮抬起时调用，触发按钮点击的主要行为。
        /// </summary>
        /// <param name="buttonProBase">传入的 ButtonProBase 对象，用于访问其点击事件</param>
        public void OnPointerUp(ButtonProBase buttonProBase)
        {
            // 触发按钮点击事件
            // buttonProBase.OnPointerClick();
        }

        /// <summary>
        /// 按钮点击时调用，用于判断是否播放点击音效。
        /// </summary>
        /// <returns>是否成功触发点击事件的标志</returns>
        public bool OnButtonClick()
        {
            if (m_IsUseClickAudio)
            {
                // 播放点击音效
                // MusicManager.Instance.PlaySoundByPath(SoundDefine.clickSoundPath);
                ButtonProDemo.Instance.PlaySound();
            }

            return true; // 目前无条件返回 true，后续可考虑实际操作是否成功的条件
        }
    }
}
