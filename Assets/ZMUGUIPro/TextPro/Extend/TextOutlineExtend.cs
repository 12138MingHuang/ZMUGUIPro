using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    [System.Serializable]
    public class TextOutlineExtend
    {
        [SerializeField] private bool _useOutline; // 是否使用描边
        [SerializeField] private Color _effectColor = new Color(0f, 0f, 0f, 0.5f); // 描边颜色，默认半透明黑色
        [SerializeField] private Vector2 _effectDistance = new Vector2(1f, -1f); // 描边的偏移距离

        private const float KMaxEffectDistance = 600f; // 描边偏移的最大距离限制

        /// <summary>
        /// 是否启用描边效果
        /// </summary>
        public bool UseOutline
        {
            get => _useOutline;
            set => _useOutline = value;
        }
        
        /// <summary>
        /// 描边颜色
        /// </summary>
        public Color EffectColor
        {
            get => _effectColor;
            set => _effectColor = value;
        }

        /// <summary>
        /// 描边偏移距离
        /// </summary>
        public Vector2 EffectDistance
        {
            get => _effectDistance;
            set
            {
                // 限制偏移距离在最大范围内
                if (value.x > KMaxEffectDistance) value.x = KMaxEffectDistance;
                if (value.x < -KMaxEffectDistance) value.x = -KMaxEffectDistance;
                if (value.y > KMaxEffectDistance) value.y = KMaxEffectDistance;
                if (value.y < -KMaxEffectDistance) value.y = -KMaxEffectDistance;
                
                if (_effectDistance == value) return;
                
                _effectDistance = value;
            }
        }

        /// <summary>
        /// 应用单个方向的描边效果，不分配额外内存
        /// </summary>
        /// <param name="verts">顶点列表</param>
        /// <param name="color">描边颜色</param>
        /// <param name="start">描边起始顶点索引</param>
        /// <param name="end">描边结束顶点索引</param>
        /// <param name="x">水平偏移</param>
        /// <param name="y">垂直偏移</param>
        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            UIVertex vt;
            var neededCapacity = verts.Count + end - start;
            if (verts.Capacity < neededCapacity) verts.Capacity = neededCapacity;

            // 遍历顶点，生成偏移的描边顶点
            for (var i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt);
                
                // 调整位置
                Vector3 v = vt.position;
                v.x += x;
                v.y += y;
                vt.position = v;

                // 应用透明度混合效果
                var newColor = color;
                newColor.a = (byte)(newColor.a * verts[i].color.a / 255f);
                vt.color = newColor;

                // 更新顶点信息
                verts[i] = vt;
            }
        }

        /// <summary>
        /// 根据偏移设置在四个方向应用描边效果
        /// </summary>
        /// <param name="vh">顶点帮助类，用于处理顶点数据</param>
        public void PopulateMesh(VertexHelper vh)
        {
            if (UseOutline)
            {
                List<UIVertex> verts = new List<UIVertex>();
                vh.GetUIVertexStream(verts); // 获取当前顶点数据
                
                // 设置顶点容量，避免频繁分配内存
                var neededCapacity = verts.Count * 5;
                if (verts.Capacity < neededCapacity) verts.Capacity = neededCapacity;

                var start = 0;
                var end = verts.Count;

                // 应用右下角偏移描边
                ApplyShadowZeroAlloc(verts, EffectColor, start, verts.Count, EffectDistance.x, EffectDistance.y);
                
                // 应用右上角偏移描边
                start = end;
                end = verts.Count;
                ApplyShadowZeroAlloc(verts, EffectColor, start, verts.Count, EffectDistance.x, -EffectDistance.y);
                
                // 应用左下角偏移描边
                start = end;
                end = verts.Count;
                ApplyShadowZeroAlloc(verts, EffectColor, start, verts.Count, -EffectDistance.x, EffectDistance.y);
                
                // 应用左上角偏移描边
                start = end;
                end = verts.Count;
                ApplyShadowZeroAlloc(verts, EffectColor, start, verts.Count, -EffectDistance.x, -EffectDistance.y);
                
                // 更新顶点帮助类，使描边效果生效
                vh.Clear();
                vh.AddUIVertexTriangleStream(verts);
            }
        }
    }
}
