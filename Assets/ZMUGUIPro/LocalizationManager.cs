using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    /// <summary>
    /// 语言类型
    /// </summary>
    public enum LanguageType
    {
        None = 0,
        English = 1,
        Chinese = 2,
    }

    /// <summary>
    /// 语言字体类型
    /// </summary>
    public enum LanguageFontType
    {
        None = 0,
        English = 1,
        Chinese = 2,
    }
    
    public class LocalizationManager
    {
        private static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalizationManager();
                }
                return _instance;
            }
        }
        
        private LocalizationDataConfig _localizationDataConfig = new LocalizationDataConfig(); // 本地多语言数据配置文件
        private LanguageType _languageType; // 当前语言
        private Dictionary<LanguageType, List<LocalizationData>> _localizationDataListDic = new Dictionary<LanguageType, List<LocalizationData>>(); // 已加载的本地多语言数据列表
        private Dictionary<LanguageType, Font> fontDic = new Dictionary<LanguageType, Font>(); // 已加载的字体字典
        private List<LocalizationData> _localizationDataList
        {
            get
            {
                return _localizationDataListDic.ContainsKey(_languageType) ? _localizationDataListDic[_languageType] : null;
            }
        } // 获取当前语言的多语言数据列表
        private List<System.Action> _localizationTextList = new List<System.Action>(); // 语言改变文本监听列表
        private List<System.Action> _localizationFontList = new List<System.Action>(); // 语言改变字体监听列表
        private string _lastUseLanguageType = "lastLanguageType"; // 本地多语言标记
        
        /// <summary>
        /// 当前语言类型
        /// </summary>
        public LanguageType LanguageType
        {
            get => _languageType;
            set => _languageType = value;
        }
        
        /// <summary>
        /// 初始化语言配置
        /// </summary>
        /// <returns></returns>
        private async Task<int> InitLanguageConfig()
        {
            InitLanguageType();
            return await LoadLanguageConfig();
        }

        /// <summary>
        /// 初始化多语言类型
        /// </summary>
        private void InitLanguageType()
        {
            int intLanguageType = PlayerPrefs.GetInt(_lastUseLanguageType);
            LanguageType = intLanguageType == 0 ? LanguageType = LanguageType.English : LanguageType = (LanguageType) intLanguageType;
            PlayerPrefs.SetInt(_lastUseLanguageType, (int) LanguageType);
        }

        /// <summary>
        /// 异步加载当前语言的多语言配置文件。
        /// </summary>
        /// <returns>异步任务，返回加载状态。</returns>
        private async Task<int> LoadLanguageConfig()
        {
            if (_localizationDataList != null) return 1;

            List<LocalizationData> dataList = await _localizationDataConfig.LoadConfig(_languageType);
            if (dataList != null)
            {
                _localizationDataListDic.Add(_languageType, dataList);
            }
            return 0;
        }

        /// <summary>
        /// Editor模式下预加载配置
        /// </summary>
        public void PreLoadConfigEditor()
        {
            if(_localizationDataList != null) return;
            
            InitLanguageType();
            
            List<LocalizationData> dataList = _localizationDataConfig.LoadConfigEditor(_languageType);
            if (dataList != null)
            {
                _localizationDataListDic.Add(_languageType, dataList);
            }
        }
        
        #region 多语言获取

        /// <summary>
        /// 获取指定键的多语言数据。
        /// </summary>
        /// <param name="key">多语言键值</param>
        /// <returns>多语言数据，若找不到则返回默认值</returns>
        public LocalizationData GetLocalizationData(string key)
        {
            LocalizationData tempData = new LocalizationData
            {
                key = "noConfig",
                value = "noConfig"
            };
            
            if(string.IsNullOrEmpty(key)) return tempData;
            if (_localizationDataList == null)
            {
#if UNITY_EDITOR
                
                PreLoadConfigEditor();
#else
                PreLoadConfig();

#endif
            }
            List<LocalizationData> dataList = _localizationDataList;
            if (dataList == null)
            {
                Debug.LogError("多语言配置文件未加载");
                return tempData;
            }
            for (int i = 0; i < dataList.Count; i++)
            {
                LocalizationData data = dataList[i];
                if(string.Equals(data.key, key))
                {
                    return data;
                }
            }
            Debug.LogError("多语言配置文件未找到对应key " + key);
            return tempData;
        }

        /// <summary>
        /// 获取指定键的多语言字符串值。
        /// </summary>
        /// <param name="key">多语言键值</param>
        /// <returns>多语言字符串值</returns>
        public string GetLocalizationDataValue(string key)
        {
            return GetLocalizationData(key).value;
        }

        /// <summary>
        /// 获取指定键的多语言文本（支持是否需要文本校正）。
        /// </summary>
        /// <param name="key">多语言键值</param>
        /// <param name="needCorrect">是否需要文本校正</param>
        /// <returns>多语言文本</returns>
        public string GetLocalizationText(string key, bool needCorrect = true)
        {
            if (_localizationDataList == null)
            {
#if UNITY_EDITOR
                    PreLoadConfigEditor();
#else
                    PreLoadConfig();
#endif
            }

            if (_localizationDataList == null)
            {
                return "";
            }
            for (int i = 0; i < _localizationDataList.Count; i++)
            {
                LocalizationData data = _localizationDataList[i];
                if (string.Equals(data.key, key))
                {
                    return data.value;
                }
            }
            Debug.LogError("多语言配置文件未找到对应key " + key);
            return "";
        }

        /// <summary>
        /// 获取当前语言的图片索引。
        /// </summary>
        /// <returns>图片索引值</returns>
        public int GetLocalizationImageIndex()
        {
            return (int) _languageType;
        }
        
        /// <summary>
        /// 获取当前语言类型的名称。
        /// </summary>
        /// <returns>语言类型名称</returns>
        public string GetLanguageTypeName()
        {
            return _languageType.ToString();
        }
        
        #endregion

        /// <summary>
        /// 切换语言并重新加载相应配置，触发语言变更的文本和字体更新。
        /// </summary>
        /// <param name="languageType">目标语言类型</param>
        /// <returns>异步任务，返回切换结果</returns>
        public async Task<string> SwitchLanguage(LanguageType languageType)
        {
            _languageType = languageType;
            PlayerPrefs.SetInt(_lastUseLanguageType, (int) _languageType);
            //等待对应语言配置加载完成
            await LoadLanguageConfig();

            for (int i = 0; i < _localizationTextList.Count; i++)
            {
                _localizationTextList[i]?.Invoke();
            }

            for (int i = 0; i < _localizationFontList.Count; i++)
            {
                _localizationFontList[i]?.Invoke();
            }
            return "";
        }

        /// <summary>
        /// 根据语言类型更改指定文本的字体。
        /// </summary>
        /// <param name="text">待更改字体的 UI 文本对象</param>
        /// <param name="setType">指定语言类型</param>
        public void ChangeFont(Text text, LanguageType setType = LanguageType.None)
        {
            if (text != null)
            {
                var currentType = setType == LanguageType.None ? LanguageType : setType;
                
                string fontName = "defaultFont";
                switch (currentType)
                {
                    case LanguageType.English:
                        fontName = "Siddhanta Unity";
                        break;
                }
                Font font = null;
                if (!fontDic.ContainsKey(LanguageType))
                {
                    font = Resources.Load<Font>("Font/" + fontName);
                    fontDic.Add(LanguageType, font);
                }
                else
                {
                    font = fontDic[LanguageType];
                }
                if (font != null) text.font = font;
            }
        }

        #region 事件监听

        /// <summary>
        /// 添加语言切换时的文本刷新回调。
        /// </summary>
        /// <param name="localizationTextCall">刷新文本的回调函数</param>
        public void AddLanguageChangeListener(System.Action localizationTextCall)
        {
            if(!_localizationTextList.Contains(localizationTextCall)) _localizationTextList.Add(localizationTextCall);
        }

        /// <summary>
        /// 移除语言切换时的文本刷新回调。
        /// </summary>
        /// <param name="localizationTextCall">需要移除的回调函数</param>
        public void RemoveLanguageChangeListener(System.Action localizationTextCall)
        {
            if(_localizationTextList.Contains(localizationTextCall)) _localizationTextList.Remove(localizationTextCall);
        }

        /// <summary>
        /// 添加语言切换时的字体刷新回调。
        /// </summary>
        /// <param name="localizationTextCall">刷新字体的回调函数</param>
        public void AddFontChangeListener(System.Action localizationTextCall)
        {
            if(!_localizationFontList.Contains(localizationTextCall)) _localizationFontList.Add(localizationTextCall);
        }

        public void RemoveFontChangeListener(System.Action localizationTextCall)
        {
            if(_localizationFontList.Contains(localizationTextCall)) _localizationFontList.Remove(localizationTextCall);
        }
        
        #endregion
    }
}
