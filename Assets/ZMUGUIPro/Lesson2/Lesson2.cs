using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lesson2 : MonoBehaviour
{
    private TextMeshPro t;
    // Start is called before the first frame update
    void Start()
    {
        t = transform.Find("ll").GetComponent<TextMeshPro>();
        t.text = "Hello World";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
