using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    [System.Serializable]
    public class TextSpacingExtend
    {
        [SerializeField] private bool _userTextSpacing;
        /// <summary>
        /// 用户自定义的文本间距
        /// </summary>
        public bool UserTextSpacing
        {
            get => _userTextSpacing;
            set => _userTextSpacing = value;
        }
        
        [SerializeField]
        [Range(-10, 100)]
        private float _textSpacing = 1f;
        /// <summary>
        /// 文本间距
        /// </summary>
        public float TextSpacing
        {
            get => _textSpacing;
            set => _textSpacing = value;
        }

        /// <summary>
        /// 填充文本的Mesh
        /// </summary>
        /// <param name="toFill"> Mesh </param>
        public void PopulateMesh(VertexHelper toFill)
        {
            if (UserTextSpacing)
            {
                if(toFill.currentVertCount == 0)
                {
                    return;
                }
                // 创建一个空的 List，用于存储当前的 UI 顶点数据。
                List<UIVertex> vertexList = new List<UIVertex>();

                // 从 VertexHelper（toFill）中获取顶点数据流，并填充到 vertexList 中。
                toFill.GetUIVertexStream(vertexList);

                // 获取当前顶点数量。UI 文本每个字符通常会有 6 个顶点。
                int indexCount = toFill.currentIndexCount;

                // 声明一个 UIVertex 变量，用于存储当前顶点。
                UIVertex vt;

                // 从第一个字符（顶点索引从 6 开始）开始循环处理顶点
                for (int i = 6; i < indexCount; i++)
                {
                    // 获取当前顶点的 UIVertex 数据
                    vt = vertexList[i];

                    // 计算每个字符的水平偏移：对于每个字符的6个顶点，将它们按字符索引整体向右偏移。
                    // 使用 (i / 6) 计算字符索引，再乘以 _textSpacing 得到水平位移量
                    vt.position += new Vector3(_textSpacing * (i / 6), 0, 0);

                    // 更新顶点列表中的当前顶点信息
                    vertexList[i] = vt;

                    // 由于 UI 文本的顶点顺序是特定的，下面代码处理每个字符的顶点在 VertexHelper 中的正确位置
                    // 对于每个字符，顶点 0, 1, 2 和 4 需要更新位置

                    // 更新索引关系：i % 6 <= 2 表示顶点 0, 1, 2
                    if (i % 6 <= 2)
                    {
                        // i / 6 * 4 计算出当前字符的基底索引 + 顶点相对位置（% 6）
                        toFill.SetUIVertex(vt, i / 6 * 4 + i % 6);
                    }
                    // i % 6 == 4 表示顶点 4，这个顶点与前面的顶点 3 重叠，需要单独处理
                    if (i % 6 == 4)
                    {
                        // 特殊情况：更新顶点位置时减去 1，以正确映射到四边形的顶点顺序
                        toFill.SetUIVertex(vt, i / 6 * 4 + i % 6 - 1);
                    }
                }

            }
        }
    }
}
