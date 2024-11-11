using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    [System.Serializable]
    public class VertexColorExtend
    {
        public enum ColorFilterType
        {
            /// <summary>
            /// 基础色加上新颜色
            /// </summary>
            Additive,
            
            /// <summary>
            /// 颜色叠加
            /// </summary>
            OverLap
        }
        
        #region 变量属性
        
        [SerializeField] private bool _userVertexColor;
        [SerializeField] private ColorFilterType _vertexColorFilter = ColorFilterType.Additive;
        [SerializeField] private Color _vertexTopLeft = Color.white;
        [SerializeField] private Color _vertexTopRight = Color.white;
        [SerializeField] private Color _vertexBottomLeft = Color.white;
        [SerializeField] private Color _vertexBottomRight = Color.white;
        [SerializeField] private Vector2 _vertexColorOffset = Vector2.zero;

        /// <summary>
        /// 是否使用顶点颜色
        /// </summary>
        public bool UserVertexColor
        {
            get => _userVertexColor;
            set => _userVertexColor = value;
        }

        /// <summary>
        /// 顶点颜色过滤类型
        /// </summary>
        public ColorFilterType VertexColorFilter
        {
            get => _vertexColorFilter;
            set => _vertexColorFilter = value;
        }

        /// <summary>
        /// 顶点左上颜色
        /// </summary>
        public Color VertexTopLeft
        {
            get => _vertexTopLeft;
            set => _vertexTopLeft = value;
        }

        /// <summary>
        /// 顶点右上颜色
        /// </summary>
        public Color VertexTopRight
        {
            get => _vertexTopRight;
            set => _vertexTopRight = value;
        }

        /// <summary>
        /// 顶点左下颜色
        /// </summary>
        public Color VertexBottomLeft
        {
            get => _vertexBottomLeft;
            set => _vertexBottomLeft = value;
        }

        /// <summary>
        /// 顶点右下颜色
        /// </summary>
        public Color VertexBottomRight
        {
            get => _vertexBottomRight;
            set => _vertexBottomRight = value;
        }

        /// <summary>
        /// 顶点颜色偏移
        /// </summary>
        public Vector2 VertexColorOffset
        {
            get => _vertexColorOffset;
            set => _vertexColorOffset = value;
        }
        #endregion

        /// <summary>
        /// 填充顶点颜色
        /// </summary>
        /// <param name="toFill"> 所填充的顶点 </param>
        /// <param name="rectTransform"> 所填充的矩形 </param>
        /// <param name="color"> 填充颜色 </param>
        public void PopulateMesh(VertexHelper toFill, RectTransform rectTransform, Color color)
        {
            if (UserVertexColor)
            {
                Vector2 min = rectTransform.pivot;
                min.Scale(-rectTransform.rect.size);
                Vector2 max = rectTransform.rect.size + min;
                int len = toFill.currentVertCount;
                for (int i = 0; i < len; i++)
                {
                    UIVertex v = new UIVertex();
                    toFill.PopulateUIVertex(ref v, i);
                    v.color = RemapColor(min, max, color, v.position);
                    toFill.SetUIVertex(v, i);
                }
            }
        }

        /// <summary>
        /// 顶点颜色映射
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="color"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Color RemapColor(Vector2 min, Vector2 max, Color color, Vector3 pos)
        {
            float x01 = max.x == min.x ? 0f : Mathf.Clamp01((pos.x - min.x) / (max.x - min.x));
            float y01 = max.y == min.y ? 0f : Mathf.Clamp01((pos.y - min.y) / (max.y - min.y));
            x01 -= VertexColorOffset.x * (VertexColorOffset.x > 0f ? x01 : 1f - x01);
            y01 -= VertexColorOffset.y * (VertexColorOffset.y > 0f ? y01 : 1f - y01);
            Color newColor = Color.Lerp(
                Color.Lerp(VertexBottomLeft, VertexBottomRight, x01),
                Color.Lerp(VertexTopLeft, VertexTopRight, x01),
                y01);
            switch (VertexColorFilter)
            {
                default:
                case ColorFilterType.Additive:
                    return newColor + color;
                case ColorFilterType.OverLap:
                    float a = Mathf.Max(newColor.a, color.a);
                    newColor = Color.Lerp(color, newColor, newColor.a);
                    newColor.a = a;
                    return newColor;
            }
        }
    }
}