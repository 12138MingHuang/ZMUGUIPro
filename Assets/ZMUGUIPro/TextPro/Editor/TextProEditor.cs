using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEngine;

namespace ZM.UGUIPro
{
    [CustomEditor(typeof(TextPro), true)]
    [CanEditMultipleObjects]
    public class TextProEditor : GraphicEditor
    {
        private static bool _textSpacingPanelOpen = false;
        private static bool _vertexColorPanelOpen = false;
        private static bool _textShadowPanelOpen = false;
        private static bool _textOutlinePanelOpen = false;
        private static bool _localizationTextPanelOpen = false;
        private static bool _textEffectPanelOpen = false;

        private string[] _titlesE = { "字符间距", "顶点颜色", "阴影", "多语言", "描边&渐变" };

        private string[] _titlesC = { "Text Spacing", "Vertex Color", "Shadow", "LocalizationText", "Outline & Gradient" };

        private SerializedProperty m_Text;
        private SerializedProperty m_FontData;

        // text spacing
        private SerializedProperty _useTextSpacing;
        private SerializedProperty _textSpacing;

        // Vertex Color
        private SerializedProperty _useVertexColor;
        private SerializedProperty _vertexColorFilter;
        private SerializedProperty _vertexColorOffset;
        private SerializedProperty _vertexTopLeft;
        private SerializedProperty _vertexTopRight;
        private SerializedProperty _vertexBottomLeft;
        private SerializedProperty _vertexBottomRight;

        // Shadow
        private SerializedProperty _useShadow;
        private SerializedProperty _shadowColorTopLeft;
        private SerializedProperty _shadowColorTopRight;
        private SerializedProperty _shadowColorBottomLeft;
        private SerializedProperty _shadowColorBottomRight;
        private SerializedProperty _shadowEffectDistance;

        // Localization
        private SerializedProperty _useLocalization;
        private SerializedProperty _key;
        private SerializedProperty _changeFont;

        // Text Effect
        private SerializedProperty _useTextEffect;
        private SerializedProperty _gradientType;
        private SerializedProperty _topColor;
        private SerializedProperty _openShaderOutLine;
        private SerializedProperty _middleColor;
        private SerializedProperty _bottomColor;
        private SerializedProperty _colorOffset;
        private SerializedProperty _enableOutLine;
        private SerializedProperty _outLineColor;
        private SerializedProperty _outLineWidth;
        private SerializedProperty m_Camera;
        private SerializedProperty _lerpValue;
        private SerializedProperty _alpha;
        private SerializedProperty _textEffect;

        protected override void OnEnable()
        {
            base.OnEnable();

            TextPro textPlus = (TextPro)this.target;
            textPlus.TextEffectExtend.SaveSerializeData(textPlus);

            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");

            // text spacing
            _useTextSpacing = serializedObject.FindProperty("_textSpacingExtend._useTextSpacing");
            _textSpacing = serializedObject.FindProperty("_textSpacingExtend._textSpacing");

            // Vertex Color
            _useVertexColor = serializedObject.FindProperty("_vertexColorExtend._useVertexColor");
            _vertexColorFilter = serializedObject.FindProperty("_vertexColorExtend._vertexColorFilter");
            _vertexTopLeft = serializedObject.FindProperty("_vertexColorExtend._vertexTopLeft");
            _vertexTopRight = serializedObject.FindProperty("_vertexColorExtend._vertexTopRight");
            _vertexBottomLeft = serializedObject.FindProperty("_vertexColorExtend._vertexBottomLeft");
            _vertexBottomRight = serializedObject.FindProperty("_vertexColorExtend._vertexBottomRight");
            _vertexColorOffset = serializedObject.FindProperty("_vertexColorExtend._vertexColorOffset");

            // Shadow
            _useShadow = serializedObject.FindProperty("_textShadowExtend._useShadow");
            _shadowColorTopLeft = serializedObject.FindProperty("_textShadowExtend._shadowColorTopLeft");
            _shadowColorTopRight = serializedObject.FindProperty("_textShadowExtend._shadowColorTopRight");
            _shadowColorBottomLeft = serializedObject.FindProperty("_textShadowExtend._shadowColorBottomLeft");
            _shadowColorBottomRight = serializedObject.FindProperty("_textShadowExtend._shadowColorBottomRight");
            _shadowEffectDistance = serializedObject.FindProperty("_textShadowExtend._effectDistance");

            // Localization
            _useLocalization = serializedObject.FindProperty("_localizationTextExtend._useLocalization");
            _key = serializedObject.FindProperty("_localizationTextExtend._key");
            _changeFont = serializedObject.FindProperty("_localizationTextExtend._changeFont");

            // Text Effect
            _useTextEffect = serializedObject.FindProperty("_textEffectExtend._useTextEffect");
            _alpha = serializedObject.FindProperty("_textEffectExtend._alpha");
            _gradientType = serializedObject.FindProperty("_textEffectExtend._gradientType");
            _topColor = serializedObject.FindProperty("_textEffectExtend._topColor");
            _openShaderOutLine = serializedObject.FindProperty("_textEffectExtend._openShaderOutLine");
            _middleColor = serializedObject.FindProperty("_textEffectExtend._middleColor");
            _bottomColor = serializedObject.FindProperty("_textEffectExtend._bottomColor");
            _colorOffset = serializedObject.FindProperty("_textEffectExtend._colorOffset");
            _enableOutLine = serializedObject.FindProperty("_textEffectExtend._enableOutLine");
            _outLineColor = serializedObject.FindProperty("_textEffectExtend._outLineColor");
            _outLineWidth = serializedObject.FindProperty("_textEffectExtend._outLineWidth");
            m_Camera = serializedObject.FindProperty("_textEffectExtend.m_Camera");
            _lerpValue = serializedObject.FindProperty("_textEffectExtend._lerpValue");
            _textEffect = serializedObject.FindProperty("_textEffectExtend.textEffect");

            // Panel Open
            _textSpacingPanelOpen = EditorPrefs.GetBool("UGUIPro._textSpacingPanelOpen", _textSpacingPanelOpen);
            _vertexColorPanelOpen = EditorPrefs.GetBool("UGUIPro._vertexColorPanelOpen", _vertexColorPanelOpen);
            _textShadowPanelOpen = EditorPrefs.GetBool("UGUIPro._textShadowPanelOpen", _textShadowPanelOpen);
            _textOutlinePanelOpen = EditorPrefs.GetBool("UGUIPro._textOutlinePanelOpen", _textOutlinePanelOpen);
            _localizationTextPanelOpen =
                EditorPrefs.GetBool("UGUIPro._localizationTextPanelOpen", _localizationTextPanelOpen);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            TextProGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void TextProGUI()
        {
            GUI.enabled = false;
            if (_textEffect.objectReferenceValue != null)
            {
                EditorGUILayout.ObjectField("Graphic", ((TextEffect)_textEffect.objectReferenceValue).TextGraphic,
                    typeof(Text), false);
            }

            GUI.enabled = true;

            TextProDrawEditor.TextSpacingGUI(GetTitle(0), _useTextSpacing, _textSpacing, ref _textSpacingPanelOpen);

            TextProDrawEditor.VertexColorGUI(
                GetTitle(1),
                _useVertexColor,
                _vertexTopLeft,
                _vertexTopRight,
                _vertexBottomLeft,
                _vertexBottomRight,
                _vertexColorFilter,
                _vertexColorOffset,
                ref _vertexColorPanelOpen
            );

            TextProDrawEditor.TextShadowGUI(
                GetTitle(2),
                _useShadow,
                _shadowColorTopLeft,
                _shadowColorTopRight,
                _shadowColorBottomLeft,
                _shadowColorBottomRight,
                _shadowEffectDistance,
                ref _textShadowPanelOpen
            );

            TextProDrawEditor.LocalizationGUI(
                GetTitle(3),
                ref _localizationTextPanelOpen,
                0f,
                _useLocalization,
                _key,
                _changeFont
            );

            TextProDrawEditor.TextEffectGUI(
                GetTitle(4),
                _useTextEffect,
                ref _textEffectPanelOpen,
                _gradientType,
                _topColor,
                _openShaderOutLine,
                _middleColor,
                _bottomColor,
                _colorOffset,
                _enableOutLine,
                _outLineColor,
                _outLineWidth,
                m_Camera,
                _lerpValue,
                _alpha,
                (TextEffect)_textEffect.objectReferenceValue
            );

            if (GUI.changed)
            {
                EditorPrefs.SetBool("UGUIPro._textSpacingPanelOpen", _textSpacingPanelOpen);
                EditorPrefs.SetBool("UGUIPro._vertexColorPanelOpen", _vertexColorPanelOpen);
                EditorPrefs.SetBool("UGUIPro._textShadowPanelOpen", _textShadowPanelOpen);
                EditorPrefs.SetBool("UGUIPro._textOutlinePanelOpen", _textOutlinePanelOpen);
                EditorPrefs.SetBool("UGUIPro._localizationTextPanelOpen", _localizationTextPanelOpen);
                EditorPrefs.SetBool("UGUIPro._textEffectPanelOpen", _textEffectPanelOpen);
            }
        }

        private string GetTitle(int index)
        {
            return UGUIProSetting.EditorLanguageType == 0 ? _titlesE[index] : _titlesC[index];
        }
    }
}