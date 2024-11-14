using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZM.UGUIPro
{
    /// <summary>
    /// 用于存储多语言文本数据的类，包括键值对形式的多语言内容。
    /// </summary>
    [System.Serializable]
    public class LocalizationData
    {
        public string key;
        public string value;
    }

    /// <summary>
    /// 多语言配置数据管理类，负责加载和解析多语言数据。
    /// </summary>
    public class LocalizationDataConfig
    {
        /// <summary>
        /// 多语言数据的导出路径，资源文件应保存在该路径下
        /// </summary>
        public const string OutputConfigPath = "ZMUGUIPro/Localization/ExcelData";
        
        private bool _isConfigLoading = false; // 标识当前配置文件是否正在加载，防止重复加载

        /// <summary>
        /// 异步加载指定语言的多语言配置文件。
        /// </summary>
        /// <param name="languageType">语言类型（如英文、中文）</param>
        /// <returns>返回包含多语言数据的列表，如果加载失败返回 null</returns>
        public async Task<List<LocalizationData>> LoadConfig(LanguageType languageType)
        {
            // 如果配置正在加载，则直接返回 null
            if(_isConfigLoading) return null;
            
            _isConfigLoading = true;  // 设置加载标志
            
            string[] languageNames = Enum.GetNames(typeof(LanguageType));
            string name = languageNames[(int) languageType];
            string configPath = $"Assets/{OutputConfigPath}/{name}/{name}.txt";
#if UNITY_EDITOR
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(configPath);
#else
            //TextAsset textAsset = AssetsManager.Instance.LoadTextAsset(configPath);
            TextAsset textAsset = Resources.Load<TextAsset>(configPath);
#endif

            // 检查是否成功加载文本资源
            if (textAsset != null && textAsset.text != null)
            {
                string json = textAsset.text;
                List<LocalizationData> localizationDataList = null;
                // 使用异步任务解析 JSON 数据
                await Task.Run(() =>
                {
                    localizationDataList = JsonConvert.DeserializeObject<List<LocalizationData>>(json);
                });
                _isConfigLoading = false;
                return localizationDataList;
            }
            _isConfigLoading = false;
            return null;
        }

        /// <summary>
        /// 编辑器模式下同步加载多语言配置文件，便于在编辑器中进行调试。
        /// </summary>
        /// <param name="languageType">语言类型</param>
        /// <returns>返回多语言数据列表，如果加载失败返回 null</returns>
        public List<LocalizationData> LoadConfigEditor(LanguageType languageType)
        {
            // 防止重复加载
            if (_isConfigLoading) return null;

            _isConfigLoading = true;  // 设置加载标志
            
            // 构建语言对应的配置文件路径
            string[] languageNames = Enum.GetNames(typeof(LanguageType));
            string name = languageNames[(int) languageType];
            string configPath = $"Assets/{OutputConfigPath}/{name}/{name}.txt";
            
#if UNITY_EDITOR
            // 使用 AssetDatabase 加载资源文件
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(configPath);
#else
            TextAsset textAsset = AssetsManager.Instance.LoadTextAsset(configPath);
            // TextAsset textAsset = Resources.Load<TextAsset>(configPath);
#endif
            // 检查是否成功加载文本资源
            if (textAsset != null && textAsset.text != null)
            {
                string json = textAsset.text;
                List<LocalizationData> localizationDataList = null;
                localizationDataList = JsonConvert.DeserializeObject<List<LocalizationData>>(json);
                _isConfigLoading = false;
                return localizationDataList;
            }
            _isConfigLoading = false;
            return null;
        }
    }
}
