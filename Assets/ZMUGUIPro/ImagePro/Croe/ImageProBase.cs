using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    /// <summary>
    /// ImageProBase 类继承自 Unity 的 Image 类，扩展了图片的多语言支持和遮罩功能。
    /// </summary>
    [System.Serializable]
    public class ImageProBase : Image
    {
        // 多语言图片扩展实例
        [SerializeField]
        private LocalizationImageExtend m_LocalizationImage = new LocalizationImageExtend();
        
        // 外部访问多语言图片扩展的属性
        public LocalizationImageExtend LocalizationImageExtend { get { return m_LocalizationImage; } }

        // 图片遮罩扩展实例
        [SerializeField]
        private ImageMaskExtend m_ImageMaskExtend = new ImageMaskExtend();

        // 重写 sprite 属性，在重新分配图片时调用 SpriteReassign 方法
        public new Sprite sprite { get { return base.sprite; } set { base.sprite = value; SpriteReassign(); } }

        /// <summary>
        /// Unity 生命周期方法 Awake，在实例化时初始化多语言和遮罩功能。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            m_LocalizationImage.Initialize(this);
            m_ImageMaskExtend.Initialize(this);
        }

        /// <summary>
        /// Unity 生命周期方法 Start，在场景开始时更新多语言图片。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            m_LocalizationImage.UpdateImage();
        }

        /// <summary>
        /// Update 方法更新遮罩图形的属性。
        /// </summary>
        private void Update()
        {
            m_ImageMaskExtend.Update();
        }

        /// <summary>
        /// Unity 生命周期方法 OnDestroy，在销毁时清理资源。
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_LocalizationImage.Release();
        }

        /// <summary>
        /// 修改遮罩的填充百分比。
        /// </summary>
        /// <param name="value">填充百分比，范围在 0 到 1 之间。</param>
        public void ModifyFillPercent(float value)
        {
            m_ImageMaskExtend.m_FillPercent = value;
        }

        #if UNITY_EDITOR
        /// <summary>
        /// 在编辑器模式下调用，用于在参数变更时重新初始化遮罩功能。
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            m_ImageMaskExtend.EditorInitializa(this);
        }
        #endif

        /// <summary>
        /// 重写 OnPopulateMesh 方法，根据是否启用遮罩功能生成网格数据。
        /// </summary>
        /// <param name="vh">用于绘制网格的 VertexHelper。</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (!m_ImageMaskExtend.m_IsUseMaskImage)
            {
                base.OnPopulateMesh(vh);
            }
            else
            {
                m_ImageMaskExtend.OnPopulateMesh(vh);
            }
        }

        /// <summary>
        /// 检查射线检测的点击位置是否在图片内，并根据遮罩功能进行扩展判断。
        /// </summary>
        /// <param name="screenPoint">屏幕上的点击位置。</param>
        /// <param name="eventCamera">摄像机。</param>
        /// <returns>点击点是否在图片的有效范围内。</returns>
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!m_ImageMaskExtend.m_IsUseMaskImage)
            {
                return base.IsRaycastLocationValid(screenPoint, eventCamera);
            }
            else
            {
                Vector2 local;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);
                return m_ImageMaskExtend.Contains(local, m_ImageMaskExtend.outterVertices, m_ImageMaskExtend.innerVertices);
            }
        }

        /// <summary>
        /// 用于处理 sprite 属性重新赋值的虚方法，可在子类中重写实现自定义行为。
        /// </summary>
        public virtual void SpriteReassign() { }
    }
}
