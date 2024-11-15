using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.UGUIPro;

public class Lesson4 : MonoBehaviour
{
    public ButtonPro button;
    private void Start()
    {
        //1.ButtonPro使用教程
        
        //2.动态添加事件
        //普通事件添加
        // button.AddButtonClick(OnButtonClick);
        // button.onClick.AddListener(OnButtonClick);
        
        //长按事件添加
        // button.AddButtonLongListener(OnLongPressButtonClick);
        
        //双击事件添加
        // button.AddButtonDoubleClickListener(OnDoubleButtonClick);
    }
    
    
    public void OnButtonClick()
    {
        Debug.Log("普通点击事件触发");
    }
    
    public void OnLongPressButtonClick()
    {
        Debug.Log("长按点击事件触发");
    }

    public void OnDoubleButtonClick()
    {
        Debug.Log("双击点击事件触发");
    }
}
