using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.UGUIPro
{
    /// <summary>
    /// ImagePro 类继承自 ImageProBase，旨在进一步扩展图片的功能。
    /// </summary>
    [System.Serializable]
    public class ImagePro : ImageProBase
    {
        /// <summary>
        /// 清除图片缓存的功能方法，具体实现可在此方法内完成。
        /// </summary>
        public void ReleaseCache()
        {
            // TODO: 实现释放图片缓存的逻辑
        }
    }
}
