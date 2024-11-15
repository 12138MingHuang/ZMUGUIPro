using UnityEngine;

namespace ZM.UGUIPro
{
    [System.Serializable]
    public class LocalizationTextMeshProExtend
    {

        [SerializeField]
        private bool m_UseLocalization;
        public bool UseLocalization
        {
            get
            {
                return m_UseLocalization;
            }
            set
            {
                m_UseLocalization = value;
            }
        }
        [SerializeField]
        private string m_Key;
        public string Key { get { return m_Key; } }

        private TextPro mTextPro;

        public void Initialize(TextProBase textPro)
        {
            mTextPro =(TextPro) textPro;
            LocalizationManager.Instance.AddLanguageChangeListener(UpdateText);
        }
        public void Release()
        {
            LocalizationManager.Instance.RemoveLanguageChangeListener(UpdateText);
        }
        public void UpdateText()
        {
            if (m_UseLocalization == false)
                return;
            if (mTextPro != null)
            {
                mTextPro.text = LocalizationManager.Instance.GetLocalizationText(m_Key);
            }

        }
    }
}
