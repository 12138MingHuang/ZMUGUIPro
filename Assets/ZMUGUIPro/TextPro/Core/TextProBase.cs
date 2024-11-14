/*----------------------------------------------------------------
* Title: ZM.UGUIPro
*
* Description: TextPro ImagePro ButtonPro TextMesh Pro
* 
* Support Function: 高性能描边、本地多语言文本、图片、按钮双击模式、长按模式、文本顶点颜色渐变、双色渐变、三色渐变
* 
* Usage: 右键-TextPro-ImagePro-ButtonPro-TextMeshPro
* 
* Author: ZhangBin
*
* Date: 2024.11.11
*
* Modify: 
--------------------------------------------------------------------*/

using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    //IMeshModifier 是 Unity 中UnityEngine.UI的一个接口，用于修改 UI 元素的网格（mesh）。它通常用于自定义 UI 渲染行为，比如在显示文本时修改其顶点数据，实现特殊的视觉效果。
    [System.Serializable]
    public class TextProBase : Text, IMeshModifier
    {
        #region 变量属性
        
        [SerializeField] public string winName; // 窗口名称
        [SerializeField] private TextSpacingExtend _textSpacingExtend = new TextSpacingExtend(); // 文本间距
        [SerializeField] private VertexColorExtend _vertexColorExtend = new VertexColorExtend(); // 文本顶点颜色
        [SerializeField] private TextShadowExtend _textShadowExtend = new TextShadowExtend(); // 文本阴影
        [SerializeField] private TextOutlineExtend _textOutlineExtend = new TextOutlineExtend(); // 文本描边
        [SerializeField] private TextEffectExtend _textEffectExtend = new TextEffectExtend(); // 文本特效
        [SerializeField] private LocalizationTextExtend _localizationTextExtend = new LocalizationTextExtend(); // 本地多语言文本

        /// <summary>
        /// 文本间距
        /// </summary>
        public TextSpacingExtend TextSpacingExtend => _textSpacingExtend;

        /// <summary>
        /// 文本顶点颜色
        /// </summary>
        public VertexColorExtend VertexColorExtend => _vertexColorExtend;

        /// <summary>
        /// 文本阴影
        /// </summary>
        public TextShadowExtend TextShadowExtend => _textShadowExtend;

        /// <summary>
        /// 文本描边
        /// </summary>
        public TextOutlineExtend TextOutlineExtend => _textOutlineExtend;

        /// <summary>
        /// 文本特效
        /// </summary>
        public TextEffectExtend TextEffectExtend => _textEffectExtend;
        
        /// <summary>
        /// 本地多语言文本
        /// </summary>
        public LocalizationTextExtend LocalizationTextExtend => _localizationTextExtend;
        
        #endregion

        protected override void Awake()
        {
            base.Awake();
            if (LocalizationTextExtend.UseLocalization)
                LocalizationTextExtend.Initialize(this);
            _localizationTextExtend.UpdateFont();
            if(LocalizationTextExtend.ChangeFont)
                LocalizationTextExtend.InitFontListener(this);
        }

        protected override void Start()
        {
            base.Start();
            _localizationTextExtend.UpdateFont();
            _localizationTextExtend.UpdateText();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (LocalizationTextExtend.UseLocalization)
                _localizationTextExtend.Release();
            
            if(LocalizationTextExtend.ChangeFont)
                _localizationTextExtend.RemoveFontListener();
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
            if(_textSpacingExtend.UseTextSpacing)
                _textSpacingExtend.PopulateMesh(toFill);
            
            if(_vertexColorExtend.UseVertexColor)
                _vertexColorExtend.PopulateMesh(toFill, rectTransform, color);
            
            if(_textShadowExtend.UseShadow)
                _textShadowExtend.PopulateMesh(toFill, rectTransform, color);
            
            if(_textOutlineExtend.UseOutline)
                _textOutlineExtend.PopulateMesh(toFill);
        }

        public void ModifyMesh(Mesh mesh)
        {
            // 此方法用于实现对 Mesh 的自定义修改（在 IMeshModifier 接口中定义）。
        }

        public void ModifyMesh(VertexHelper verts)
        {
            // 此方法用于实现对 Mesh 的自定义修改（在 IMeshModifier 接口中定义）。
        }

        /// <summary>
        /// 设置文本的透明度
        /// </summary>
        public void SetTextAlpha(float alpha)
        {
            if (_textEffectExtend.UseTextEffect && _textEffectExtend._gradientType != 0)
            {
                _textEffectExtend.SetAlpha(alpha);
            }
            else
            {
                Color32 color32 = color;
                color32.a = (byte)(alpha * 255);
                color = color32;
            }
        }
        
        /// <summary>
        /// 设置文本描边颜色
        /// </summary>
        public void SetOutLineColor(Color32 color)
        {
            if(!_textEffectExtend.UseTextEffect) return;
            _textEffectExtend.textEffect.SetOutLineColor(color);
            _textEffectExtend.UseTextEffect = false;
            _textEffectExtend.UseTextEffect = true;
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            // 编辑器模式下自动更新本地化和字体设置。
            base.OnValidate();
        }
#endif
    }
}
