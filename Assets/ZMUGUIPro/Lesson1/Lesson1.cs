using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.UGUIPro;

public class Lesson1 : MonoBehaviour
{
    private async void Start()
    {
        //1.多语言Excel表配置生成方式
        //生成流程：菜单栏-Tools-生成Excel多语言配置文件
        
        //2.多语言Excel读取、解析、输出路径配置 ExcelToConfig
        
        //3.多语言功能使用演示
        //初始化多语言系统，自动加载本地对应语言配置文件
        // await LocalizationManager.Instance.InitLanguageConfig();
        
        //4.切换语言
        // await LocalizationManager.Instance.SwitchLanguage(LanguageType.English);
        
        //5.多语言图片加载方式 ImageProBase
    }
    
    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            await LocalizationManager.Instance.SwitchLanguage(LanguageType.English);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            await LocalizationManager.Instance.SwitchLanguage(LanguageType.Chinese);
        }
    }
}
