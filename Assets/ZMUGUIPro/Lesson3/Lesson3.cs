using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.UGUIPro;

public class Lesson3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    async void Update()
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
