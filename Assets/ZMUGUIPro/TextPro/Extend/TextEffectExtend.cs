using UnityEngine;
using UnityEngine.Serialization;

namespace ZM.UGUIPro
{
    [System.Serializable]
    public class TextEffectExtend
    {
        [SerializeField] private bool _useTextEffect; // 是否启用文本特效
        [SerializeField] private float _lerpValue = 0; // 差值，用于平滑过渡效果
        [SerializeField] private bool _openShaderOutLine = true; // 是否启用Shader描边效果
        [SerializeField] private TextProOutLine _outlineEx; // 用于保存自定义的描边组件
        [SerializeField] private bool _enableOutLine = false; // 是否启用描边
        [SerializeField] private float _outLineWidth = 1; // 描边宽度
        [SerializeField] public GradientType _gradientType = GradientType.OneColor; // 渐变类型
        [SerializeField] private Color32 _topColor = Color.white; // 顶部颜色
        [SerializeField] private Color32 _middleColor = Color.white; // 中部颜色
        [SerializeField] private Color32 _bottomColor = Color.white; // 底部颜色
        [SerializeField] private Color32 _outLineColor = Color.yellow; // 描边颜色
        [SerializeField] private Camera _camera; // 渲染时使用的摄像机
        [SerializeField, UnityEngine.Range(0, 1)] private float _alpha = 1; // 透明度，范围在0到1之间
        [SerializeField, UnityEngine.Range(0.1f, 0.9f)] private float _colorOffset = 0.5f; // 颜色偏移
        [SerializeField] public TextEffect _textEffect; // 关联的文本特效组件

        /// <summary>
        /// 是否启用文本特效
        /// </summary>
        public bool UseTextEffect
        {
            get => _useTextEffect;
            set => _useTextEffect = value;
        }

        /// <summary>
        /// 是否启用描边
        /// </summary>
        public bool UseOutLine => _enableOutLine;

        /// <summary>
        /// 保存序列化数据，将特效组件和摄像机进行初始化
        /// </summary>
        /// <param name="textPro">文本组件</param>
        public void SaveSerializeData(TextPro textPro)
        {
            // 获取TextEffect组件，若不存在则自动添加
            _textEffect = textPro.GetComponent<TextEffect>();
            if (_textEffect == null)
            {
                int insid = textPro.GetInstanceID();

                // 遍历所有TextPro组件，找到与当前实例相同的并添加TextEffect组件
                TextPro[] textProArray = Transform.FindObjectsOfType<TextPro>();
                for (int i = 0; i < textProArray.Length; i++)
                {
                    if (textProArray[i].GetInstanceID() == insid)
                    {
                        _textEffect = textProArray[i].gameObject.AddComponent<TextEffect>();
                        _textEffect.hideFlags = HideFlags.HideInInspector; // 隐藏特效组件
                        break;
                    }
                }
            }

            // 初始化摄像机，优先查找带有 "MainCamera" 标签的对象
            if (_camera == null)
            {
                GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
                if (mainCamera != null)
                {
                    _camera = mainCamera.GetComponent<Camera>();
                }
                else
                {
                    // 若找不到主摄像机，则获取场景中的任意摄像机
                    _camera = Transform.FindObjectOfType<Camera>();
                }
            }
        }
    }
}
