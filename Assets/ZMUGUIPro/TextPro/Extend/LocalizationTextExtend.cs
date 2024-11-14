using TMPro;
using UnityEngine;

namespace ZM.UGUIPro
{
    [System.Serializable]
    public class LocalizationTextExtend
    {
        [SerializeField] private bool _useLocalization; // 是否启用本地化文本
        [SerializeField] private bool _changeFont = true; // 是否允许更换字体
        [SerializeField] private string _key; // 本地化文本的键值
        
        private TextPro _textPro; // 用于处理 TextPro 类型文本
        private TextMeshPro _textMeshPro; // 用于处理 TextMeshPro 类型文本
        
        /// <summary>
        /// 是否启用本地化文本
        /// </summary>
        public bool UseLocalization
        {
            get => _useLocalization;
            set => _useLocalization = value;
        }

        /// <summary>
        /// 是否允许更换字体
        /// </summary>
        public bool ChangeFont
        {
            get => _changeFont;
            set => _changeFont = value;
        }

        /// <summary>
        /// 本地化文本的键值
        /// </summary>
        public string Key
        {
            get => _key;
            set => _key = value;
        }

        /// <summary>
        /// 初始化本地化设置并注册语言更改监听器（适用于 TextPro 类型）
        /// </summary>
        public void Initialize(TextProBase textPro)
        {
            _textPro = textPro as TextPro;
            LocalizationManager.Instance.AddLanguageChangeListener(UpdateText);
        }

        /// <summary>
        /// 初始化本地化设置并注册语言更改监听器（适用于 TextMeshPro 类型）
        /// </summary>
        public void Initialize(TextMeshPro textMeshPro)
        {
            _textMeshPro = textMeshPro;
            LocalizationManager.Instance.AddLanguageChangeListener(UpdateText);
        }
        
        /// <summary>
        /// 初始化字体变更监听器（适用于 TextPro 类型）
        /// </summary>
        public void InitFontListener(TextProBase textPro)
        {
            _textPro = textPro as TextPro;
            LocalizationManager.Instance.AddFontChangeListener(UpdateFont);
        }

        /// <summary>
        /// 释放语言变更监听器
        /// </summary>
        public void Release()
        {
            LocalizationManager.Instance.RemoveLanguageChangeListener(UpdateText);
        }
        
        /// <summary>
        /// 移除字体变更监听器
        /// </summary>
        public void RemoveFontListener()
        {
            LocalizationManager.Instance.RemoveFontChangeListener(UpdateFont);
        }

        /// <summary>
        /// 更新文本内容，根据当前的本地化设置和指定的键值
        /// </summary>
        public void UpdateText()
        {
            if (!_useLocalization) return;
            
            // 根据本地化管理器的内容设置文本
            if (_textPro != null)
                _textPro.text = LocalizationManager.Instance.GetLocalizationText(_key);
            else if (_textMeshPro != null)
                _textMeshPro.text = LocalizationManager.Instance.GetLocalizationText(_key);
        }

        /// <summary>
        /// 更新字体，根据当前的字体更改设置
        /// </summary>
        public void UpdateFont()
        {
            if (!_changeFont) return;
            
            // 调用本地化管理器更改字体
            if (_textPro != null)
                LocalizationManager.Instance.ChangeFont(_textPro);
        }
    }
}
