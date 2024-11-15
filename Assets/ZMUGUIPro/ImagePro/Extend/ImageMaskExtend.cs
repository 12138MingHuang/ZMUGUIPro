using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using System;

namespace ZM.UGUIPro
{
    /// <summary>
    /// ImageMaskExtend 类用于扩展 Unity 的 Image 组件，实现圆形和扇形填充、圆环绘制等功能。
    /// </summary>
    [Serializable]
    public class ImageMaskExtend
    {
        public Image m_Image; // 绑定的 Image 组件
        private RectTransform rectTransform; // 缓存的 RectTransform

        [SerializeField]
        public bool m_IsUseMaskImage = false; // 是否启用遮罩图像功能

        //private Sprite z_Sprite;
        //public Sprite sprite { get { return z_Sprite; } set { if (SetPropertyUtilityExtend.SetClass(ref z_Sprite, value)) m_Image.SetAllDirty(); } }
        
        /// <summary>
        /// Image 的 overrideSprite 属性
        /// </summary>
        public Sprite overrideSprite
        {
            get { return m_Image.overrideSprite; }
            set
            {
                m_Image.overrideSprite = value;
                m_Image.SetAllDirty();
            }
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="image">绑定的 Image 组件</param>
        public void Initialize(Image image)
        {
            m_Image = image;
            rectTransform = m_Image.rectTransform;
            innerVertices = new List<Vector3>();
            outterVertices = new List<Vector3>();
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器模式下的初始化方法
        /// </summary>
        /// <param name="image">绑定的 Image 组件</param>
        public void EditorInitializa(Image image)
        {
            m_Image = image;
            rectTransform = m_Image.rectTransform;
        }
#endif

        /// <summary>
        /// 更新方法（在运行时更新三角形宽度约束）
        /// </summary>
        public void Update()
        {
            this.m_TrisCont = Mathf.Clamp(this.m_TrisCont, 0, m_Image.rectTransform.rect.width / 2);
        }

        [SerializeField]
        [Tooltip("圆形或扇形填充比例")]
        [Range(0, 1)]
        public float m_FillPercent = 1f; // 填充比例（0~1）

        [SerializeField]
        [Tooltip("是否填充圆形")]
        public bool m_Fill = true; // 是否为实心圆

        [Tooltip("圆环宽度")]
        public float m_TrisCont = 5; // 圆环的宽度

        [SerializeField]
        [Tooltip("圆形分段数量")]
        [Range(3, 100)]
        public int m_Segements = 20; // 圆形分段数量

        public List<Vector3> innerVertices; // 内环顶点列表
        public List<Vector3> outterVertices; // 外环顶点列表

        /// <summary>
        /// 绘制 Mesh 数据
        /// </summary>
        /// <param name="vh">VertexHelper 用于生成顶点和三角形数据</param>
        public void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            innerVertices.Clear();
            outterVertices.Clear();

            float degreeDelta = 2 * Mathf.PI / m_Segements; // 每段的角度增量
            int curSegements = (int)(m_Segements * m_FillPercent); // 当前填充的分段数量

            float tw = rectTransform.rect.width; // 宽度
            float th = rectTransform.rect.height; // 高度
            float outerRadius = rectTransform.pivot.x * tw; // 外半径
            float innerRadius = outerRadius - m_TrisCont; // 内半径

            Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            float uvCenterX = (uv.x + uv.z) * 0.5f;
            float uvCenterY = (uv.y + uv.w) * 0.5f;
            float uvScaleX = (uv.z - uv.x) / tw;
            float uvScaleY = (uv.w - uv.y) / th;

            float curDegree = 0; // 当前角度
            UIVertex uiVertex; // 顶点数据
            Vector2 curVertice; // 当前顶点坐标

            if (m_Fill) // 绘制实心圆或扇形
            {
                // 添加中心点顶点
                curVertice = Vector2.zero;
                uiVertex = new UIVertex
                {
                    color = m_Image.color,
                    position = curVertice,
                    uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY)
                };
                vh.AddVert(uiVertex);

                // 添加外环顶点
                for (int i = 1; i <= curSegements; i++)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    curVertice = new Vector2(cosA * outerRadius, sinA * outerRadius);
                    curDegree += degreeDelta;

                    uiVertex = new UIVertex
                    {
                        color = m_Image.color,
                        position = curVertice,
                        uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY)
                    };
                    vh.AddVert(uiVertex);
                    outterVertices.Add(curVertice);
                }

                // 添加三角形
                for (int i = 1; i < curSegements; i++)
                {
                    vh.AddTriangle(0, i, i + 1);
                }
                if (m_FillPercent == 1) // 如果填充完整，则首尾相连
                {
                    vh.AddTriangle(0, curSegements, 1);
                }
            }
            else // 绘制圆环
            {
                for (int i = 0; i < curSegements; i++)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    curDegree += degreeDelta;

                    // 添加内环顶点
                    curVertice = new Vector3(cosA * innerRadius, sinA * innerRadius);
                    uiVertex = new UIVertex
                    {
                        color = m_Image.color,
                        position = curVertice,
                        uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY)
                    };
                    vh.AddVert(uiVertex);
                    innerVertices.Add(curVertice);

                    // 添加外环顶点
                    curVertice = new Vector3(cosA * outerRadius, sinA * outerRadius);
                    uiVertex = new UIVertex
                    {
                        color = m_Image.color,
                        position = curVertice,
                        uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY)
                    };
                    vh.AddVert(uiVertex);
                    outterVertices.Add(curVertice);
                }

                // 添加三角形
                for (int i = 0; i < curSegements - 1; i++)
                {
                    vh.AddTriangle(i * 2 + 1, i * 2, i * 2 + 3);
                    vh.AddTriangle(i * 2, i * 2 + 2, i * 2 + 3);
                }
                if (m_FillPercent == 1) // 首尾相连
                {
                    vh.AddTriangle(curSegements * 2 - 1, curSegements * 2 - 2, 1);
                    vh.AddTriangle(curSegements * 2 - 2, 0, 1);
                }
            }
        }

        /// <summary>
        /// 判断点是否在图形内（使用射线交叉算法）
        /// 点击点在图形内返回true，否则返回false
        /// <param name="p">点击点 </param>
        /// <param name="outterVertices">外环顶点 </param>
        /// <param name="innerVertices">内环顶点 </param> 
        /// </summary>
        public bool Contains(Vector2 p, List<Vector3> outterVertices, List<Vector3> innerVertices)
        {
            var crossNumber = 0;
            RayCrossing(p, innerVertices, ref crossNumber); // 检测内环
            RayCrossing(p, outterVertices, ref crossNumber); // 检测外环
            return (crossNumber & 1) == 1;
        }

        /// <summary>
        /// 使用RayCrossing算法判断点击点是否在封闭多边形里
        /// </summary>
        /// <param name="p">点击点 </param>
        /// <param name="vertices">多边形顶点</param>
        /// <param name="crossNumber">交点数量</param>
        private void RayCrossing(Vector2 p, List<Vector3> vertices, ref int crossNumber)
        {
            for (int i = 0, count = vertices.Count; i < count; i++)
            {
                var v1 = vertices[i];
                var v2 = vertices[(i + 1) % count];

                // 检测交点
                if (((v1.y <= p.y) && (v2.y > p.y)) || ((v1.y > p.y) && (v2.y <= p.y)))
                {
                    if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        crossNumber++;
                    }
                }
            }
        }
    }
}
