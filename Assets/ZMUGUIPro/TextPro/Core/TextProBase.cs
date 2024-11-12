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

        public void ModifyMesh(Mesh mesh)
        {
            throw new System.NotImplementedException();
        }

        public void ModifyMesh(VertexHelper verts)
        {
            throw new System.NotImplementedException();
        }
    }
}
