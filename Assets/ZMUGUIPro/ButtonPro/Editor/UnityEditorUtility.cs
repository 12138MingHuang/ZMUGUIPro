using System;
using UnityEditor;
using UnityEngine;

namespace ZM.UGUIPro
{
    public class UnityEditorUtility
    {
        // 缓存常用的 GUI 样式
        private static Lazy<GUIStyle> gSelectButtonStyle = new Lazy<GUIStyle>(() => GetGUIStyle("flow node 3 on"));
        public static GUIStyle gCommonButtonStyle;

        /// <summary>
        /// 获取带有图标、文本和工具提示的 GUIContent。
        /// </summary>
        public static GUIContent GetUIContent(string name, string content, string tooltip = "")
        {
            GUIContent guiContent = EditorGUIUtility.IconContent(name.Trim());
            guiContent.text = content;
            guiContent.tooltip = tooltip;

            return guiContent;
        }

        /// <summary>
        /// 获取选中按钮的样式。
        /// </summary>
        public static GUIStyle GetSelectButtonStyle()
        {
            return gSelectButtonStyle.Value;
        }

        /// <summary>
        /// 查找并返回指定名称的 GUIStyle。
        /// </summary>
        public static GUIStyle GetGUIStyle(string styleName)
        {
            foreach (var style in GUI.skin.customStyles)
            {
                if (string.Equals(style.name, styleName, StringComparison.OrdinalIgnoreCase))
                {
                    style.normal.textColor = Color.yellow; // 仅在找到时修改
                    return style;
                }
            }
            Debug.LogWarning($"Style '{styleName}' not found in customStyles.");
            return null;
        }
    }
}
