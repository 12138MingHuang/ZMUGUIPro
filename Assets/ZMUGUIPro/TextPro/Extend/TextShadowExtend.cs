using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    [System.Serializable]
    public class TextShadowExtend
    {
        // 控制是否使用阴影
        [SerializeField] private bool _useShadow;
        
        // 阴影颜色（四个角的颜色，支持渐变效果）
        [SerializeField] private Color _shadowColorTopLeft = Color.white;
        [SerializeField] private Color _shadowColorTopRight = Color.white;
        [SerializeField] private Color _shadowColorBottomLeft = Color.white;
        [SerializeField] private Color _shadowColorBottomRight = Color.white;
        
        // 阴影的偏移距离
        [SerializeField] private Vector2 _effectDistance = new Vector2(1f, -1f);

        // 阴影最大偏移距离
        private const float KMaxEffectDistance = 600f;
        
        // 顶点颜色偏移
        private Vector2 _vertexColorOffset = Vector2.zero;

        // 控制是否使用阴影的属性
        public bool UseShadow
        {
            get { return _useShadow; }
            set { _useShadow = value; }
        }
        
        /// <summary>
        /// 顶点阴影颜色,左上
        /// </summary>
        public Color VertexTopLeft { get => _shadowColorTopLeft; set => _shadowColorTopLeft = value; }
        
        /// <summary>
        /// 顶点阴影颜色,右上
        /// </summary>
        public Color VertexTopRight { get => _shadowColorTopRight; set => _shadowColorTopRight = value; }
        
        /// <summary>
        /// 顶点阴影颜色,左下
        /// </summary>
        public Color VertexBottomLeft { get => _shadowColorBottomLeft; set => _shadowColorBottomLeft = value; }
        
        /// <summary>
        /// 顶点阴影颜色,右下
        /// </summary>
        public Color VertexBottomRight { get => _shadowColorBottomRight; set => _shadowColorBottomRight = value; }
        
        /// <summary>
        /// 控制阴影偏移距离，限制最大和最小值，防止超出范围
        /// </summary>
        public Vector2 EffectDistance
        {
            get => _effectDistance;
            set
            {
                if (value.x > KMaxEffectDistance) value.x = KMaxEffectDistance;
                if (value.x < -KMaxEffectDistance) value.x = -KMaxEffectDistance;
                if (value.y > KMaxEffectDistance) value.y = KMaxEffectDistance;
                if (value.y < -KMaxEffectDistance) value.y = -KMaxEffectDistance;
                
                if (_effectDistance == value) return;
                
                _effectDistance = value;
            }
        }

        /// <summary>
        /// 应用阴影效果，将生成的阴影顶点添加到 `verts` 列表中。
        /// </summary>
        /// <param name="verts">顶点列表</param>
        /// <param name="min">矩形左下角</param>
        /// <param name="max">矩形右上角</param>
        /// <param name="color">传入的阴影颜色</param>
        /// <param name="x">X轴偏移</param>
        /// <param name="y">Y轴偏移</param>
        protected void ApplyShadow(List<UIVertex> verts, Vector2 min, Vector2 max, Color32 color, float x, float y)
        {
            UIVertex vt;
            int start = 0, end = verts.Count;
            
            // 检查容量是否足够，必要时增加容量
            var neededCapacity = verts.Count + end - start;
            if (verts.Capacity < neededCapacity) verts.Capacity = neededCapacity;

            for (int i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt); // 复制当前顶点
                
                // 调整位置并设置阴影颜色
                Vector3 v = vt.position;
                v.x += x;
                v.y += y;
                vt.position = v;
                vt.color = RemapColor(min, max, color, v);
                verts[i] = vt;
            }
        }
        
        /// <summary>
        /// 映射阴影颜色，使用四角颜色和顶点位置进行渐变计算
        /// </summary>
        /// <param name="min">左下角</param>
        /// <param name="max">右上角</param>
        /// <param name="color">基础颜色</param>
        /// <param name="pos">顶点位置</param>
        /// <returns>映射后的颜色</returns>
        private Color32 RemapColor(Vector2 min, Vector2 max, Color32 color, Vector3 pos)
        {
            // 计算 x 和 y 的映射比例
            float x01 = max.x == min.x ? 0f : Mathf.Clamp01((pos.x - min.x) / (max.x - min.x));
            float y01 = max.y == min.y ? 0f : Mathf.Clamp01((pos.y - min.y) / (max.y - min.y));

            // 应用偏移以调整颜色渐变的范围
            x01 -= _vertexColorOffset.x * (_vertexColorOffset.x > 0f ? x01 : (1f - x01));
            y01 -= _vertexColorOffset.y * (_vertexColorOffset.y > 0f ? y01 : (1f - y01));

            // 四角颜色之间插值
            Color newColor = Color.Lerp(
                Color.Lerp(VertexBottomLeft, VertexBottomRight, x01),
                Color.Lerp(VertexTopLeft, VertexTopRight, x01),
                y01
            );

            // 返回新的颜色值
            return newColor;
        }

        /// <summary>
        /// 为 `VertexHelper` 填充阴影顶点
        /// </summary>
        /// <param name="vh">顶点帮助器</param>
        /// <param name="rectTransform">矩形变换</param>
        /// <param name="color">阴影颜色</param>
        public void PopulateMesh(VertexHelper vh, RectTransform rectTransform, Color color)
        {
            if (UseShadow)
            {
                Vector2 min = rectTransform.pivot;
                min.Scale(-rectTransform.rect.size); // 计算左下角位置
                Vector2 max = rectTransform.rect.size + min; // 计算右上角位置
                
                List<UIVertex> output = new List<UIVertex>();
                vh.GetUIVertexStream(output); // 获取当前顶点
                
                // 应用阴影效果
                ApplyShadow(output, min, max, color, EffectDistance.x, EffectDistance.y);
                
                // 清除原顶点并填充更新后的顶点
                vh.Clear();
                vh.AddUIVertexTriangleStream(output);
            }
        }
    }
}
