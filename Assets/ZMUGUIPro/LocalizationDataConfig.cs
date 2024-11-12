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

namespace ZMUGUIPro
{
    [System.Serializable]
    public class LocalizationData
    {
        public string key;
        public string value;
    }

    public class LocalizationDataConfig
    {
        public const string OutputConfigPath = "ZMUGUIPro/Localization/ExcelData";
        
        private bool _isConfigLoading = false;

        public async Task<List<LocalizationData>> LoadConfig(LanguageType languageType)
        {
            
        }
    }
}
