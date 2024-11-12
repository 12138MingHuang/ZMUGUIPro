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
            // 检查是否使用自定义顶点颜色
            if (UserVertexColor)
            {
                // 获取矩形的最小和最大位置，用于颜色映射
                Vector2 min = rectTransform.pivot;
                min.Scale(-rectTransform.rect.size);
                Vector2 max = rectTransform.rect.size + min;

                // 遍历每个顶点
                int len = toFill.currentVertCount;
                for (int i = 0; i < len; i++)
                {
                    UIVertex v = new UIVertex();
                    toFill.PopulateUIVertex(ref v, i);

                    // 使用 RemapColor 方法重新映射颜色
                    v.color = RemapColor(min, max, color, v.position);

                    // 将更新后的顶点颜色重新设置到 VertexHelper 中
                    toFill.SetUIVertex(v, i);
                }
            }
        }

        /// <summary>
        /// 根据顶点位置映射颜色值
        /// </summary>
        /// <param name="min"> 矩形左下角的坐标 </param>
        /// <param name="max"> 矩形右上角的坐标 </param>
        /// <param name="color"> 传入的基础颜色 </param>
        /// <param name="pos"> 当前顶点的位置 </param>
        /// <returns> 映射后的颜色值 </returns>
        private Color RemapColor(Vector2 min, Vector2 max, Color color, Vector3 pos)
        {
            // 计算 x 和 y 的映射比例 (0-1 范围)，用于在矩形区域内按位置插值颜色
            float x01 = max.x == min.x ? 0f : Mathf.Clamp01((pos.x - min.x) / (max.x - min.x));
            float y01 = max.y == min.y ? 0f : Mathf.Clamp01((pos.y - min.y) / (max.y - min.y));

            // 应用顶点颜色偏移，使得颜色在 x 和 y 方向上可以产生偏移效果
            x01 -= VertexColorOffset.x * (VertexColorOffset.x > 0f ? x01 : 1f - x01);
            y01 -= VertexColorOffset.y * (VertexColorOffset.y > 0f ? y01 : 1f - y01);

            // 根据映射比例在四角颜色之间进行插值
            Color newColor = Color.Lerp(
                Color.Lerp(VertexBottomLeft, VertexBottomRight, x01), // 左下和右下的颜色插值，水平方向
                Color.Lerp(VertexTopLeft, VertexTopRight, x01),       // 左上和右上的颜色插值，水平方向
                y01                                                   // 垂直方向的插值，控制上下两个水平插值结果之间的过渡
            );

            // 根据指定的颜色过滤类型返回最终颜色
            switch (VertexColorFilter)
            {
                // Additive 模式：将基础颜色和插值后的顶点颜色相加
                default:
                case ColorFilterType.Additive:
                    return newColor + color;

                // OverLap 模式：在两个颜色之间根据透明度进行叠加，取更高的透明度
                case ColorFilterType.OverLap:
                    float a = Mathf.Max(newColor.a, color.a); // 获取更高的透明度
                    newColor = Color.Lerp(color, newColor, newColor.a); // 使用透明度插值
                    newColor.a = a; // 设置最终颜色的透明度
                    return newColor;
            }
        }
    }
}