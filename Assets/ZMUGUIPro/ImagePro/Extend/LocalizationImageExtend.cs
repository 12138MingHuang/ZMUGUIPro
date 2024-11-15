using UnityEngine;

namespace ZM.UGUIPro
{
    /// <summary>
    /// 本地化图像类型枚举，用于确定图像的加载方式。
    /// Static 表示静态加载；DynamicLoad 表示动态加载。
    /// </summary>
    [System.Serializable]
    public enum LocalizationImageType
    {
        Static,
        DynamicLoad,
    }

    /// <summary>
    /// 本地图像加载类型枚举，用于确定图像是通过 Resources 还是 AssetBundle 加载。
    /// Resources 表示资源文件夹加载；AssetBundle 表示资源包加载。
    /// </summary>
    [System.Serializable]
    public enum LocalImageLoadType
    {
        Resources,
        AssetBundle,
    }

    /// <summary>
    /// 本地化图像扩展类，用于处理图像的多语言本地化逻辑。
    /// 支持通过静态或动态方式加载不同语言的图像。
    /// </summary>
    [System.Serializable]
    public class LocalizationImageExtend
    {
        // 是否启用多语言功能
        [SerializeField]
        private bool m_IsUseLocalization;
        public bool IsUseLocalization { get { return m_IsUseLocalization; } set { m_IsUseLocalization = value; } }

        // 本地化图像类型：静态加载或动态加载
        [SerializeField]
        private LocalizationImageType m_LocalizationImageType = LocalizationImageType.Static;

        // 图像加载方式：Resources 或 AssetBundle
        [SerializeField]
        private LocalImageLoadType m_LocalImageLoadType = LocalImageLoadType.AssetBundle;

        // 静态本地化图像数组，每种语言对应一个 Sprite
        [SerializeField]
        private Sprite[] m_LocalSprites = null;

        // 静态图像对应的大小数组，每种语言对应一个 Vector2 尺寸
        [SerializeField]
        private Vector2[] m_SpriteSizev2 = null;

        // 动态加载图像的路径数组，每种语言对应一个路径
        [SerializeField]
        private string[] m_LocalSpritePaths = null;

        // 动态加载图像的大小数组，每种语言对应一个 Vector2 尺寸
        [SerializeField]
        private Vector2[] m_LoadSpriteSizev2 = null;

        // 图像的名称，用于动态加载资源
        [SerializeField]
        private string m_ImageName = null;
        // 关联的 ImagePro 对象
        private ImagePro m_ImagePro;

        /// <summary>
        /// 初始化本地化图像扩展功能，绑定到指定的 ImageProBase 对象上。
        /// 添加语言变更监听器以更新图像。
        /// </summary>
        /// <param name="imagePro">要关联的 ImageProBase 对象</param>
        public void Initialize(ImageProBase imagePro)
        {
            m_ImagePro = (ImagePro)imagePro;
            LocalizationManager.Instance.AddLanguageChangeListener(UpdateImage);
        }

        /// <summary>
        /// 更新图像以适应当前语言设置，根据静态或动态加载方式选择不同的处理逻辑。
        /// </summary>
        public void UpdateImage()
        {
            // 如果未启用多语言功能，则退出
            if (!m_IsUseLocalization)
                return;

            if (m_ImagePro != null)
            {
                // 获取当前语言对应的索引
                int index = LocalizationManager.Instance.GetLocalizationImageIndex();
                Vector2 spriteSize = Vector2.zero;

                // 根据本地化图像类型选择静态或动态加载逻辑
                switch (m_LocalizationImageType)
                {
                    case LocalizationImageType.Static:
                        m_ImagePro.sprite = m_LocalSprites[index];
                        spriteSize = m_SpriteSizev2[index];
                        break;

                    case LocalizationImageType.DynamicLoad:
                        // 根据本地图像加载类型选择加载方式
                        switch (m_LocalImageLoadType)
                        {
                            case LocalImageLoadType.Resources:
                                // 从 Resources 文件夹加载图像
                                m_ImagePro.sprite = Resources.Load<Sprite>(m_LocalSpritePaths[index]);
                                spriteSize = m_LoadSpriteSizev2[index];
                                break;

                            case LocalImageLoadType.AssetBundle:
                                // 从 AssetBundle 中加载图像
                                var languageType = LocalizationManager.Instance.GetLanguageTypeName();
                                string bundlePath = ""; // bundlePath 具体路径需在此指定
                                var atlasPath = $"{bundlePath}{languageType}/{languageType}";

#if UNITY_EDITOR
                                // 在编辑器模式下直接从 Resources 加载，方便测试
                                m_ImagePro.sprite = Resources.Load<Sprite>($"{atlasPath}/{m_ImageName}.png");
#else
                                // 在运行时通过 AssetManager 加载
                                m_ImagePro.sprite = AssetsManager.Instance.LoadAtlasSprite(atlasPath, m_ImageName);
#endif
                                break;
                        }
                        break;
                }

                // 设置图像的尺寸，如果 spriteSize 为零，则使用图像的原始尺寸
                m_ImagePro.rectTransform.sizeDelta = spriteSize == Vector2.zero ? m_ImagePro.sprite.rect.size : spriteSize;
            }
        }

        /// <summary>
        /// 释放本地化图像扩展功能，移除语言变更监听器。
        /// </summary>
        public void Release()
        {
            LocalizationManager.Instance.RemoveLanguageChangeListener(UpdateImage);
        }
    }
}
