using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    public static class TextProDrawEditor
    {
        [MenuItem("GameObject/UI/Text Pro", priority = 0)]
        public static void CreateTextPro()
        {
            GameObject root = new GameObject("Text Pro", typeof(RectTransform), typeof(TextPro));
            ResetInCanvasFor((RectTransform)root.transform);
            root.GetComponent<TextPro>().text = "Text Pro";
            var text = root.GetComponent<TextPro>();
            text.text = "Text Pro";
            text.color = Color.white;
            text.raycastTarget = false;
            text.rectTransform.sizeDelta = new Vector2(200, 50);
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleCenter;
            root.transform.localPosition = Vector3.zero;
        }

        public static void TextSpacingGUI(string title, SerializedProperty _useTextSpacing,
            SerializedProperty _textSpacing, ref bool _textSpacingPanelOpen)
        {
            LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(_useTextSpacing);
                if (_useTextSpacing.boolValue)
                {
                    Space();
                    LayoutHorizontal(() =>
                    {
                        EditorGUI.PropertyField(GUIRect(0, 18), _textSpacing, new GUIContent());
                    });
                }
            }, title, ref _textSpacingPanelOpen, true);
        }

        public static void VertexColorGUI(string title, SerializedProperty _useVertexColor,
            SerializedProperty _vertexTopLeft, SerializedProperty _vertexTopRight, SerializedProperty _vertexBottomLeft,
            SerializedProperty _vertexBottomRight, SerializedProperty _vertexColorFilter,
            SerializedProperty _vertexColorOffset, ref bool _vertexColorPanelOpen)
        {
            LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(_useVertexColor);
                if (_useVertexColor.boolValue)
                {
                    Space();
                    LayoutHorizontal(() =>
                    {
                        EditorGUI.PropertyField(GUIRect(0, 18), _vertexTopLeft, new GUIContent());
                        Space();
                        EditorGUI.PropertyField(GUIRect(0, 18), _vertexTopRight, new GUIContent());
                    });
                    Space();
                    LayoutHorizontal(() =>
                    {
                        EditorGUI.PropertyField(GUIRect(0, 18), _vertexBottomLeft, new GUIContent());
                        Space();
                        EditorGUI.PropertyField(GUIRect(0, 18), _vertexBottomRight, new GUIContent());
                    });
                    Space();
                    _vertexColorFilter.enumValueIndex =
                        (int)(VertexColorExtend.ColorFilterType)EditorGUILayout.EnumPopup(
                            new GUIContent("Filter"),
                            (VertexColorExtend.ColorFilterType)_vertexColorFilter.enumValueIndex
                        );
                    Vector2 newOffset = EditorGUILayout.Vector2Field("Offset", _vertexColorOffset.vector2Value);
                    newOffset.x = Mathf.Clamp(newOffset.x, -1f, 1f);
                    newOffset.y = Mathf.Clamp(newOffset.y, -1f, 1f);
                    _vertexColorOffset.vector2Value = newOffset;
                    Space();
                }
            }, title, ref _vertexColorPanelOpen, true);
        }

        public static void TextShadowGUI(string title, SerializedProperty _useShadow,
            SerializedProperty _shadowColorTopLeft, SerializedProperty _shadowColorTopRight,
            SerializedProperty _shadowColorBottomLeft, SerializedProperty _shadowColorBottomRight,
            SerializedProperty _shadowEffectDistance, ref bool _textShadowPanelOpen)
        {
            LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(_useShadow);
                if (_useShadow.boolValue)
                {
                    Space();
                    LayoutHorizontal(() =>
                    {
                        EditorGUI.PropertyField(GUIRect(0, 18), _shadowColorTopLeft, new GUIContent());
                        Space();
                        EditorGUI.PropertyField(GUIRect(0, 18), _shadowColorTopRight, new GUIContent());
                    });
                    Space();
                    LayoutHorizontal(() =>
                    {
                        EditorGUI.PropertyField(GUIRect(0, 18), _shadowColorBottomLeft, new GUIContent());
                        Space();
                        EditorGUI.PropertyField(GUIRect(0, 18), _shadowColorBottomRight, new GUIContent());
                    });
                    Space();
                    EditorGUILayout.PropertyField(_shadowEffectDistance);
                }
            }, title, ref _textShadowPanelOpen, true);
        }

        public static void TextEffectGUI(string title, SerializedProperty _useTextEffect, ref bool _textEffectPanelOpen,
            SerializedProperty _gradientType,
            SerializedProperty _topColor,
            SerializedProperty _openShaderOutLine,
            SerializedProperty _middleColor,
            SerializedProperty _bottomColor,
            SerializedProperty _colorOffset,
            SerializedProperty _enableOutLine,
            SerializedProperty _outLineColor,
            SerializedProperty _outLineWidth,
            SerializedProperty m_Camera,
            SerializedProperty _lerpValue,
            SerializedProperty _alpha,
            TextEffect textEffect)
        {
            LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(_useTextEffect);
                textEffect?.SetUserEffect(_useTextEffect.boolValue);
                if (_useTextEffect.boolValue)
                {
                    Space();
                    //_alpha = m_Alpha.floatValue;
                    //_alpha = EditorGUILayout.Slider("Alpha", _alpha, 0, 1);
                    EditorGUILayout.PropertyField(_alpha);
                    EditorGUILayout.PropertyField(_gradientType);
                    EditorGUILayout.PropertyField(m_Camera);
                    EditorGUILayout.PropertyField(_enableOutLine);
                    textEffect?.SetEnableOutline(_enableOutLine.boolValue);
                    textEffect?.SetCamera((Camera)m_Camera.objectReferenceValue);


                    if (_enableOutLine.boolValue)
                    {
                        EditorGUILayout.PropertyField(_outLineWidth);
                        EditorGUILayout.PropertyField(_lerpValue);
                        textEffect.SetLerpValue(_lerpValue.floatValue);
                        textEffect.SetOutLineWidth(_outLineWidth.floatValue);
                        _openShaderOutLine.boolValue =
                            EditorGUILayout.Toggle("Open Shader OutLine", _openShaderOutLine.boolValue);
                        textEffect.SetShaderOutLine(_openShaderOutLine.boolValue);
                    }

                    textEffect.SetGradientType((GradientType)_gradientType.enumValueIndex);
                    if (_gradientType.enumValueIndex == 2)
                    {
                        EditorGUILayout.PropertyField(_middleColor);
                        textEffect.SetMiddleColor(_middleColor.colorValue);
                    }

                    if (_gradientType.enumValueIndex != 0)
                    {
                        EditorGUILayout.PropertyField(_topColor);
                        EditorGUILayout.PropertyField(_bottomColor);
                        textEffect.SetBottomColor(_bottomColor.colorValue);
                        textEffect.SetTopColor(_topColor.colorValue);
                    }

                    if (_enableOutLine.boolValue)
                    {
                        EditorGUILayout.PropertyField(_outLineColor);
                        textEffect.SetOutLineColor(_outLineColor.colorValue);
                    }

                    textEffect.SetAlpha(_alpha.floatValue);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(_colorOffset);
                    textEffect.SetColorOffset(_colorOffset.floatValue);
                    textEffect.UpdateOutLineInfos();
                }
            }, title, ref _textEffectPanelOpen, true);
        }

        private static bool isCheckLoaclData = false;

        public static void LocalizationGUI(string title, ref bool _panelOpen, float space, SerializedProperty useThis,
            SerializedProperty key, SerializedProperty changeFont)
        {
            LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(changeFont);
                EditorGUILayout.PropertyField(useThis);
                if (useThis.boolValue)
                {
                    EditorGUILayout.PropertyField(key);
                    isCheckLoaclData = EditorGUILayout.Toggle("CheckLocalizationData", isCheckLoaclData);
                    if (isCheckLoaclData)
                    {
                        if (key != null)
                        {
                            LocalizationData data = LocalizationManager.Instance.GetLocalizationData(key.stringValue);
                            if (data != null)
                            {
                                GUILayout.Space(10);
                                FieldInfo[] propertyInfos = data.GetType().GetFields();
                                for (int i = 0; i < propertyInfos.Length; i++)
                                {
                                    FieldInfo info = propertyInfos[i];
                                    EditorGUILayout.TextField(info.Name + ":", info.GetValue(data).ToString(),
                                        new GUIStyle() { normal = new GUIStyleState() { textColor = Color.blue } });
                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("The key value not find!", MessageType.Error);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("The key value not find!", MessageType.Error);
                        }
                    }
                }
            }, title, ref _panelOpen, true);
        }

        public static void SimpleUseGUI(string title, ref bool _panelOpen, float space, SerializedProperty useThis,
            params SerializedProperty[] sps)
        {
            LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(useThis);
                if (useThis.boolValue)
                {
                    foreach (var s in sps)
                    {
                        if (s != null)
                        {
                            EditorGUILayout.PropertyField(s);
                        }
                    }
                }
            }, title, ref _panelOpen, true);
        }

        private static void ResetInCanvasFor(RectTransform root)
        {
            root.SetParent(Selection.activeTransform);
            if (!InCanvas(root))
            {
                Transform canvasTF = GetCreateCanvas();
                root.SetParent(canvasTF);
            }

            if (!Transform.FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
            {
                GameObject eg = new GameObject("EventSystem");
                eg.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eg.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            root.localScale = Vector3.one;
            root.localPosition = new Vector3(root.localPosition.x, root.localPosition.y, 0f);
            Selection.activeGameObject = root.gameObject;
        }


        private static bool InCanvas(Transform tf)
        {
            while (tf.parent)
            {
                tf = tf.parent;
                if (tf.GetComponent<Canvas>())
                {
                    return true;
                }
            }

            return false;
        }

        private static Transform GetCreateCanvas()
        {
            Canvas c = Object.FindObjectOfType<Canvas>();
            if (c)
            {
                return c.transform;
            }
            else
            {
                GameObject g = new GameObject("Canvas");
                c = g.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                g.AddComponent<CanvasScaler>();
                g.AddComponent<GraphicRaycaster>();
                return g.transform;
            }
        }

        private static void LayoutFrameBox(System.Action action, string label, ref bool open, bool box = false)
        {
            bool _open = open;
            LayoutVertical(() =>
            {
                _open = GUILayout.Toggle(
                    _open,
                    label,
                    GUI.skin.GetStyle("foldout"),
                    GUILayout.ExpandWidth(true),
                    GUILayout.Height(18)
                );
                if (_open)
                {
                    action();
                }
            }, box);
            open = _open;
        }

        private static Rect GUIRect(float width, float height)
        {
            return GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(width <= 0),
                GUILayout.ExpandHeight(height <= 0));
        }


        private static void Space(float space = 4f)
        {
            GUILayout.Space(space);
        }

        private static void LayoutHorizontal(System.Action action, bool box = false)
        {
            if (box)
            {
                GUIStyle style = new GUIStyle(GUI.skin.box);
                GUILayout.BeginHorizontal(style);
            }
            else
            {
                GUILayout.BeginHorizontal();
            }

            action();
            GUILayout.EndHorizontal();
        }


        private static void LayoutVertical(System.Action action, bool box = false)
        {
            if (box)
            {
                GUIStyle style = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(6, 6, 2, 2)
                };
                GUILayout.BeginVertical(style);
            }
            else
            {
                GUILayout.BeginVertical();
            }

            action();
            GUILayout.EndVertical();
        }
    }
}