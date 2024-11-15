using UnityEngine;

namespace ZM.UGUIPro
{
    /// <summary>
    /// 用于扩展属性设置功能的静态类。
    /// 提供设置不同类型属性的方法，通过比较判断是否需要更新以避免不必要的操作。
    /// </summary>
    internal static class SetPropertyUtilityExtend
    {
        /// <summary>
        /// 设置颜色属性的方法。
        /// 判断新颜色值是否与当前颜色值相同，如果不同则更新，并返回 true；否则返回 false。
        /// </summary>
        /// <param name="currentValue">引用传递的当前颜色值。</param>
        /// <param name="newValue">新颜色值。</param>
        /// <returns>如果颜色值发生变化，则返回 true；否则返回 false。</returns>
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            // 检查颜色的 RGBA 四个通道值是否完全相同，如果相同则无需更新
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
                return false; // 无需更新，返回 false

            currentValue = newValue; // 更新为新颜色值
            return true; // 返回 true，表示值已更新
        }

        /// <summary>
        /// 设置结构体属性的方法。
        /// 判断新值是否与当前值相同，如果不同则更新，并返回 true；否则返回 false。
        /// </summary>
        /// <typeparam name="T">结构体类型。</typeparam>
        /// <param name="currentValue">引用传递的当前值。</param>
        /// <param name="newValue">新值。</param>
        /// <returns>如果值发生变化，则返回 true；否则返回 false。</returns>
        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            // 如果当前值和新值相等，则无需更新
            if (currentValue.Equals(newValue))
                return false; // 无需更新，返回 false

            currentValue = newValue; // 更新为新值
            return true; // 返回 true，表示值已更新
        }

        /// <summary>
        /// 设置类属性的方法。
        /// 判断新值是否与当前值相同，如果不同则更新，并返回 true；否则返回 false。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        /// <param name="currentValue">引用传递的当前值。</param>
        /// <param name="newValue">新值。</param>
        /// <returns>如果值发生变化，则返回 true；否则返回 false。</returns>
        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            // 如果当前值和新值均为 null 或者相等，则无需更新
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false; // 无需更新，返回 false

            currentValue = newValue; // 更新为新值
            return true; // 返回 true，表示值已更新
        }
    }
}
