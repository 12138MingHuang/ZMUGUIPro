using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using ZM.UGUIPro;
namespace ZM.UGUIPro
{
    [System.Serializable]
    public class TextMeshPro : TextMeshProUGUI
    {
        [SerializeField]
        private LocalizationTextExtend _localizationTextExtend = new LocalizationTextExtend();


        protected override void Awake()
        {
            base.Awake();
            if (_localizationTextExtend.UseLocalization)
                _localizationTextExtend.Initialize(this);
            _localizationTextExtend.UpdateFont();

            _localizationTextExtend.UpdateFont();
            _localizationTextExtend.UpdateText();
        }
 
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_localizationTextExtend.UseLocalization)
                _localizationTextExtend.Release();

        }

    }
}