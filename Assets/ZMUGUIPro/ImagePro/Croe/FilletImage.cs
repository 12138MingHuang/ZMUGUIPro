using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Sprites;
using System.Collections.Generic;

namespace ZM.UGUIPro
{
    public class FilletImage : Image
    {
        const int MaxTriangleNum = 20;   // 最大三角形数，用于优化性能
        const int MinTriangleNum = 1;    // 最小三角形数
        public float Radius = 20;        // 圆角半径
        [Range(MinTriangleNum, MaxTriangleNum)]
        public int TriangleNum = 5;      // 每个角的三角形数量

        // 重写OnPopulateMesh方法自定义网格以创建圆角效果
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Vector4 v = GetDrawingDimensions(false); // 获取绘图的边界坐标
            Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            var color32 = color;
            vh.Clear();

            // 控制radius的取值，限制在图片宽高的最小值范围内
            float radius = Radius;
            radius = Mathf.Clamp(radius, 0, Mathf.Min((v.z - v.x) / 2, (v.w - v.y) / 2));

            // 计算uv的缩放
            float uvRadiusX = radius / (v.z - v.x);
            float uvRadiusY = radius / (v.w - v.y);

            // 构建矩形主体的顶点和三角形
            AddRectangleVertices(v, uv, vh, color32, radius, uvRadiusX, uvRadiusY);

            // 创建四个圆角的顶点和三角形
            GenerateRoundedCorners(v, uv, vh, color32, radius, uvRadiusX, uvRadiusY);
        }

        // 根据图像的缩放和像素进行调整，确保在不同分辨率下图片按预期绘制
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
            Rect r = GetPixelAdjustedRect();
            Vector2 size = overrideSprite == null ? new Vector2(r.width, r.height) : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                float spriteRatio = size.x / size.y;
                float rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    float oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    float oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            return new Vector4(
                r.x + r.width * (padding.x / spriteW),
                r.y + r.height * (padding.y / spriteH),
                r.x + r.width * ((spriteW - padding.z) / spriteW),
                r.y + r.height * ((spriteH - padding.w) / spriteH)
            );
        }

        private void AddRectangleVertices(Vector4 v, Vector4 uv, VertexHelper vh, Color32 color32, float radius, float uvRadiusX, float uvRadiusY)
        {
            // 添加矩形区域的顶点
            vh.AddVert(new Vector3(v.x, v.w - radius), color32, new Vector2(uv.x, uv.w - uvRadiusY));
            vh.AddVert(new Vector3(v.x, v.y + radius), color32, new Vector2(uv.x, uv.y + uvRadiusY));
            vh.AddVert(new Vector3(v.x + radius, v.w), color32, new Vector2(uv.x + uvRadiusX, uv.w));
            vh.AddVert(new Vector3(v.x + radius, v.w - radius), color32, new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
            vh.AddVert(new Vector3(v.x + radius, v.y + radius), color32, new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
            vh.AddVert(new Vector3(v.x + radius, v.y), color32, new Vector2(uv.x + uvRadiusX, uv.y));
            vh.AddVert(new Vector3(v.z - radius, v.w), color32, new Vector2(uv.z - uvRadiusX, uv.w));
            vh.AddVert(new Vector3(v.z - radius, v.w - radius), color32, new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
            vh.AddVert(new Vector3(v.z - radius, v.y + radius), color32, new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
            vh.AddVert(new Vector3(v.z - radius, v.y), color32, new Vector2(uv.z - uvRadiusX, uv.y));
            vh.AddVert(new Vector3(v.z, v.w - radius), color32, new Vector2(uv.z, uv.w - uvRadiusY));
            vh.AddVert(new Vector3(v.z, v.y + radius), color32, new Vector2(uv.z, uv.y + uvRadiusY));

            // 添加矩形区域的三角形
            vh.AddTriangle(1, 0, 3);
            vh.AddTriangle(1, 3, 4);
            vh.AddTriangle(5, 2, 6);
            vh.AddTriangle(5, 6, 9);
            vh.AddTriangle(8, 7, 10);
            vh.AddTriangle(8, 10, 11);
        }

        private void GenerateRoundedCorners(Vector4 v, Vector4 uv, VertexHelper vh, Color32 color32, float radius, float uvRadiusX, float uvRadiusY)
        {
            // 四个角的圆心位置和uv坐标
            List<Vector2> vCenterList = new List<Vector2>
            {
                new Vector2(v.z - radius, v.w - radius),
                new Vector2(v.x + radius, v.w - radius),
                new Vector2(v.x + radius, v.y + radius),
                new Vector2(v.z - radius, v.y + radius)
            };

            List<Vector2> uvCenterList = new List<Vector2>
            {
                new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY),
                new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY),
                new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY),
                new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY)
            };

            List<int> vCenterVertList = new List<int> { 7, 3, 4, 8 };

            float degreeDelta = Mathf.PI / 2 / TriangleNum;
            float curDegree = 0;

            for (int i = 0; i < vCenterVertList.Count; i++)
            {
                int preVertNum = vh.currentVertCount;
                for (int j = 0; j <= TriangleNum; j++)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    Vector3 vPosition = new Vector3(vCenterList[i].x + cosA * radius, vCenterList[i].y + sinA * radius);
                    Vector2 uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX, uvCenterList[i].y + sinA * uvRadiusY);
                    vh.AddVert(vPosition, color32, uvPosition);
                    curDegree += degreeDelta;
                }
                curDegree -= degreeDelta;
                for (int j = 0; j < TriangleNum; j++)
                {
                    vh.AddTriangle(vCenterVertList[i], preVertNum + j + 1, preVertNum + j);
                }
            }
        }
    }
}
