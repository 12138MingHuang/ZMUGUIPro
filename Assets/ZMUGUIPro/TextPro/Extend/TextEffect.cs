
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    public enum GradientType
    {
        OneColor = 0,
        TwoColor = 1,
        ThreeColor = 2
    }

    [DisallowMultipleComponent]
    [Serializable]
    public class TextEffect : BaseMeshEffect
    {
        private const string OutLineShaderName = "TextPro/Text";

        private bool _initedParams;
        
        [SerializeField]
        [HideInInspector]
        private bool _useTextEffect;
        public bool UseTextEffect
        {
            get
            {
                return _useTextEffect;
            }
            set
            {
                _useTextEffect = value;
            }
        }
        [SerializeField] [HideInInspector] private float _lerpValue = 0;
        [SerializeField] [HideInInspector] private bool _openShaderOutLine;
        [SerializeField] [HideInInspector] private TextProOutLine _outlineEx;
        [SerializeField] [HideInInspector] private bool _enableOutLine = false;
        [SerializeField] [HideInInspector] private float _outLineWidth = 1;
        [SerializeField] [HideInInspector] private GradientType _gradientType = GradientType.TwoColor;
        [SerializeField] [HideInInspector] private Color32 _topColor = Color.white;
        [SerializeField] [HideInInspector] private Color32 _middleColor = Color.white;
        [SerializeField] [HideInInspector] private Color32 _bottomColor = Color.white;
        [SerializeField] [HideInInspector] private Color32 _outLineColor = Color.black;
        [SerializeField] [HideInInspector] private Camera _camera;
        [SerializeField, UnityEngine.Range(0, 1)] [HideInInspector] private float _alpha = 1;
        [UnityEngine.Range(0.1f, 0.9f)] [SerializeField] [HideInInspector] private float _colorOffset = 0.5f;


        private List<UIVertex> iVertices = new List<UIVertex>();
        private Vector3[] m_OutLineDis = new Vector3[4];

        private Text m_Text;

        public Text TextGraphic
        {
            get
            {
                if (!this.m_Text && base.graphic)
                {
                    this.m_Text = base.graphic as Text;
                }
                else
                {
                    if (!base.graphic)
                        throw new Exception("No Find base Graphic!!");
                }

                return this.m_Text;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (!string.IsNullOrEmpty(this.TextGraphic.text))
            {
                this.UpdateOutLineInfos();
            }
            this.hideFlags = HideFlags.HideInInspector;
        }
        public void SetUserEffect(bool isUseTextEffect)
        {
            _useTextEffect = isUseTextEffect;
        }
        public void SetLerpValue(float lerpValue)
        {
            _lerpValue = lerpValue;
        }
        public void SetCamera(Camera camera)
        {
            if (_camera == camera) return;
            this._camera = camera;
        }
        public void SetGradientType(GradientType gradientType)
        {
            this._gradientType = gradientType;
        }

        public GradientType GetGradientType()
        {
            return _gradientType;
        }

        public void SetTopColor(Color topColor)
        {
            this._topColor = topColor;
        }
        public Color GetTopColor()
        {
            return _topColor;
        }

        public void SetMiddleColor(Color middleColor)
        {
            this._middleColor = middleColor;
        }

        public void SetBottomColor(Color bottomColor)
        {
            this._bottomColor = bottomColor;
        }

        public void SetEnableOutline(bool enable)
        {
            if (this._enableOutLine == enable) return;
            this._enableOutLine = enable;
        }

        public void SetColorOffset(float colorOffset)
        {

            this._colorOffset = colorOffset;
        }

        public void SetOutLineColor(Color outLineColor)
        {
            this._outLineColor = outLineColor;
            if (base.graphic && this._outlineEx)
            {
                this._outlineEx.SetOutLineColor(this._outLineColor);
                base.graphic.SetAllDirty();
            }

        }

        public void SetOutLineWidth(float outLineWidth)
        {
            this._outLineWidth = outLineWidth;
            if (base.graphic && this._outlineEx)
            {
                this._outlineEx.SetOutLineWidth(this._outLineWidth);
                base.graphic.SetAllDirty();
            }
        }

        public void SetAlpah(float setAlphaValue)
        {
            this._alpha = setAlphaValue;
            byte alphaByte = (byte)(this._alpha * 255);
            this._topColor.a = alphaByte;
            this._bottomColor.a = alphaByte;
            this._middleColor.a = alphaByte;
            this. _outLineColor.a = alphaByte;
            if (base.graphic && this._outlineEx) {
                base.graphic.SetAllDirty();
            }
        }

        public void SetShaderOutLine(bool outlineUseShader)
        {
            if (!_useTextEffect) return;
            if (!this._outlineEx)
            {
                this._outlineEx = this.gameObject.GetComponent<TextProOutLine>();
                if (!this._outlineEx)
                    this._outlineEx = this.gameObject.AddComponent<TextProOutLine>();
                this._outlineEx.graphic = base.graphic;
            }
            else
            {
                this._outlineEx.enabled = true;
            }
            this._outlineEx.hideFlags = HideFlags.HideInInspector;
            this._openShaderOutLine = outlineUseShader;
            this.UpdateOutLineInfos();
        }

        public void UpdateOutLineInfos()
        {
            if (!_useTextEffect) return;
            if (!this._outlineEx) return;
            this._outlineEx.SwitchShaderOutLine(this._openShaderOutLine);
            this._outlineEx.SetUseThree(this._gradientType == GradientType.ThreeColor);
            this._outlineEx.SetOutLineColor(this._outLineColor);
            this._outlineEx.SetOutLineWidth(this._outLineWidth);
            this.UpdateOutLineMaterial();
            if (base.graphic != null)
            {
                this.OpenShaderParams();
                base.graphic.SetAllDirty();
            }
        }


        private void OpenShaderParams()
        {
            if (!_useTextEffect) return;
            if (base.graphic && !this._initedParams)
            {
                if (base.graphic.canvas)
                {
                    var v1 = graphic.canvas.additionalShaderChannels;
                    var v2 = AdditionalCanvasShaderChannels.TexCoord1;
                    if ((v1 & v2) != v2)
                    {
                        base.graphic.canvas.additionalShaderChannels |= v2;
                    }

                    v2 = AdditionalCanvasShaderChannels.TexCoord2;
                    if ((v1 & v2) != v2)
                    {
                        base.graphic.canvas.additionalShaderChannels |= v2;
                    }

                    v2 = AdditionalCanvasShaderChannels.TexCoord3;
                    if ((v1 & v2) != v2)
                    {
                        base.graphic.canvas.additionalShaderChannels |= v2;
                    }

                    v2 = AdditionalCanvasShaderChannels.Tangent;
                    if ((v1 & v2) != v2)
                    {
                        base.graphic.canvas.additionalShaderChannels |= v2;
                    }

                    v2 = AdditionalCanvasShaderChannels.Normal;
                    if ((v1 & v2) != v2)
                    {
                        base.graphic.canvas.additionalShaderChannels |= v2;
                    }
                    this._initedParams = true;
                }
            }
        }

        private void _ProcessVertices(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }
            if (!_useTextEffect) return;
            var count = vh.currentVertCount;
            if (count == 0)
                return;

            /*
             *  TL--------TR
             *  |          |^
             *  |          ||
             *  CL--------CR
             *  |          ||
             *  |          |v
             *  BL--------BR
             * **/


            for (int i = 0; i < count; i++)
            {
                UIVertex vertex = UIVertex.simpleVert;
                vh.PopulateUIVertex(ref vertex, i);
                this.iVertices.Add(vertex);
            }
            vh.Clear();

            for (int i = 0; i < this.iVertices.Count; i += 4)
            {

                UIVertex TL = GeneralUIVertex(this.iVertices[i + 0]);
                UIVertex TR = GeneralUIVertex(this.iVertices[i + 1]);
                UIVertex BR = GeneralUIVertex(this.iVertices[i + 2]);
                UIVertex BL = GeneralUIVertex(this.iVertices[i + 3]);

                //先绘制上四个
                UIVertex CR = default(UIVertex);
                UIVertex CL = default(UIVertex);

                //如果是OneColor模式，则颜色不做二次处理
                if (this._gradientType == GradientType.OneColor)
                {

                }
                else
                {
                    TL.color = this._topColor;
                    TR.color = this._topColor;
                    BL.color = this._bottomColor;
                    BR.color = this._bottomColor;
                }


                if (this._enableOutLine)
                {

                    if (!this._openShaderOutLine)
                    {
                        if (this._outlineEx)
                        {
                            this._outlineEx.enabled = false;
                        }

                        this.m_OutLineDis[0].Set(-this._outLineWidth, this._outLineWidth, 0); //LT
                        this.m_OutLineDis[1].Set(this._outLineWidth, this._outLineWidth, 0); //RT
                        this.m_OutLineDis[2].Set(-this._outLineWidth, -this._outLineWidth, 0); //LB
                        this.m_OutLineDis[3].Set(this._outLineWidth, -this._outLineWidth, 0); //RB


                        for (int j = 0; j < 4; j++)
                        {
                            //四个方向
                            UIVertex o_TL = GeneralUIVertex(TL);
                            UIVertex o_TR = GeneralUIVertex(TR);
                            UIVertex o_BR = GeneralUIVertex(BR);
                            UIVertex o_BL = GeneralUIVertex(BL);


                            o_TL.position += this.m_OutLineDis[j];
                            o_TR.position += this.m_OutLineDis[j];
                            o_BR.position += this.m_OutLineDis[j];
                            o_BL.position += this.m_OutLineDis[j];

                            o_TL.color = this._outLineColor;
                            o_TR.color = this._outLineColor;
                            o_BR.color = this._outLineColor;
                            o_BL.color = this._outLineColor;

                            vh.AddVert(o_TL);
                            vh.AddVert(o_TR);

                            if (this._gradientType == GradientType.ThreeColor)
                            {
                                UIVertex o_CR = default(UIVertex);
                                UIVertex o_CL = default(UIVertex);

                                o_CR = GeneralUIVertex(this.iVertices[i + 2]);
                                o_CL = GeneralUIVertex(this.iVertices[i + 3]);
                                //var New_S_Point = this.ConverPosition(o_TR.position, o_BR.position, this._colorOffset);

                                o_CR.position.y = Mathf.Lerp(o_TR.position.y, o_BR.position.y, this._colorOffset);
                                o_CL.position.y = Mathf.Lerp(o_TR.position.y, o_BR.position.y, this._colorOffset);

                                if (Mathf.Approximately(TR.uv0.x, BR.uv0.x))
                                {
                                    o_CR.uv0.y = Mathf.Lerp(TR.uv0.y, BR.uv0.y, this._colorOffset);
                                    o_CL.uv0.y = Mathf.Lerp(TL.uv0.y, BL.uv0.y, this._colorOffset);
                                }
                                else
                                {
                                    o_CR.uv0.x = Mathf.Lerp(TR.uv0.x, BR.uv0.x, this._colorOffset);
                                    o_CL.uv0.x = Mathf.Lerp(TL.uv0.x, BL.uv0.x, this._colorOffset);
                                }

                                o_CR.color = this._outLineColor;
                                o_CL.color = this._outLineColor;


                                vh.AddVert(o_CR);
                                vh.AddVert(o_CL);
                            }

                            vh.AddVert(o_BR);
                            vh.AddVert(o_BL);
                        }
                    }
                }

                if (this._gradientType == GradientType.ThreeColor && this._enableOutLine && this._openShaderOutLine)
                {
                    UIVertex t_TL = GeneralUIVertex(TL);
                    UIVertex t_TR = GeneralUIVertex(TR);
                    UIVertex t_BR = GeneralUIVertex(BR);
                    UIVertex t_BL = GeneralUIVertex(BL);

                    // t_TL.color.a = 0;
                    // t_TR.color.a = 0;
                    // t_BR.color.a = 0;
                    // t_BL.color.a = 0;
                    //vh.AddVert(t_TL);
                    //vh.AddVert(t_TR);

                    //vh.AddVert(t_BR);
                    //vh.AddVert(t_BL);
                }

                vh.AddVert(TL);
                vh.AddVert(TR);

                if (this._gradientType == GradientType.ThreeColor)
                {
                    CR = GeneralUIVertex(this.iVertices[i + 2]);
                    CL = GeneralUIVertex(this.iVertices[i + 3]);
                    //var New_S_Point = this.ConverPosition(TR.position, BR.position, this._colorOffset);

                    CR.position.y = Mathf.Lerp(TR.position.y, BR.position.y - 0.1f, this._colorOffset);
                    CL.position.y = Mathf.Lerp(TR.position.y, BR.position.y, this._colorOffset);



                    if (Mathf.Approximately(TR.uv0.x, BR.uv0.x))
                    {
                        CR.uv0.y = Mathf.Lerp(TR.uv0.y, BR.uv0.y, this._colorOffset);
                        CL.uv0.y = Mathf.Lerp(TL.uv0.y, BL.uv0.y, this._colorOffset);
                    }
                    else
                    {
                        CR.uv0.x = Mathf.Lerp(TR.uv0.x, BR.uv0.x, this._colorOffset);
                        CL.uv0.x = Mathf.Lerp(TL.uv0.x, BL.uv0.x, this._colorOffset);
                    }


                    CR.color = this._middleColor;
                    CL.color = this._middleColor;
                    // CR.color = Color32.Lerp(this._middleColor, this._bottomColor, this._lerpValue);
                    // CL.color = Color32.Lerp(this._middleColor, this._bottomColor, this._lerpValue);
                    vh.AddVert(CR);
                    vh.AddVert(CL);
                }

                //绘制下四个
                if (this._gradientType == GradientType.ThreeColor)
                {
                    vh.AddVert(CL);
                    vh.AddVert(CR);
                }
                vh.AddVert(BR);
                vh.AddVert(BL);




            }

            for (int i = 0; i < vh.currentVertCount; i += 4)
            {
                vh.AddTriangle(i + 0, i + 1, i + 2);
                vh.AddTriangle(i + 2, i + 3, i + 0);
            }
        }
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!_useTextEffect) return;
            this.iVertices.Clear();
            //if (m_Text.text.Equals("Bonus"))
            //{
            //    Debuger.ColorLog(LogColor.Yellow, "Bonus>>>>>>>>>>>>>Start>>>>>>>>>>>>>>Bonus>>>>>>");
            //}
            this._ProcessVertices(vh);

            if (this._enableOutLine && this._outlineEx)
            {
                this._outlineEx.ModifyMesh(vh);
            }
            //if (m_Text.text.Equals("Bonus"))
            //{
            //    Debuger.ColorLog(LogColor.Cyan, "Bonus>>>>>>>>>>>>>End>>>>>>>>>>>>>>Bonus>>>>>>");
            //}
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (this._openShaderOutLine)
            {
                if (!_useTextEffect) return;
                this.UpdateOutLineMaterial();
                this.Refresh();
            }
        }
#endif

        public void Refresh()
        {
            if (base.graphic)
            {
                base.graphic.SetVerticesDirty();
            }

        }

        private void UpdateOutLineMaterial()
        {
            if (!_useTextEffect) return;
#if !UNITY_EDITOR
            if (base.graphic && base.graphic.material == base.graphic.defaultMaterial)
            {
                Shader shader = Shader.Find(OutLineShaderName);
                if (shader)
                {
                    base.graphic.material = new Material(shader);
                }
            }
#else
            if (!Application.isPlaying)
            {
                if (base.graphic && base.graphic.material == base.graphic.defaultMaterial)
                {
                    Material material= UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/ZMUGUIPro/TextPro/Shaders/TextPro-Text.mat");
                    if (material==null)
                        Debug.LogError("Text Out Line Material Not Find  Plase Check Material Path!");
                    base.graphic.material = material;   
                }
            }
            else
            {
                if (base.graphic && base.graphic.material == base.graphic.defaultMaterial)
                {
                    Shader shader = Shader.Find(OutLineShaderName);
                    if (shader)
                    {
                        base.graphic.material = new Material(shader);
                    }
                }
            }
#endif


            if (base.graphic)
            {
                Texture fontTexture = null;
                if (this.TextGraphic)
                {
                    if (this.graphic && this.TextGraphic.font)
                    {
                        fontTexture = this.TextGraphic.font.material.mainTexture;
                    }

                    if (base.graphic.material && base.graphic.material != base.graphic.defaultMaterial)
                        base.graphic.material.mainTexture = fontTexture;
                }
            }
        }

        public static UIVertex GeneralUIVertex(UIVertex vertex)
        {
            UIVertex result = UIVertex.simpleVert;
            result.normal = new Vector3(vertex.normal.x, vertex.normal.y, vertex.normal.z);
            result.position = new Vector3(vertex.position.x, vertex.position.y, vertex.position.z);
            result.tangent = new Vector4(vertex.tangent.x, vertex.tangent.y, vertex.tangent.z, vertex.tangent.w);
            result.uv0 = new Vector2(vertex.uv0.x, vertex.uv0.y);
            result.uv1 = new Vector2(vertex.uv1.x, vertex.uv1.y);
            result.color = vertex.color;
            return result;
        }
    }
}
