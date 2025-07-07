using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Button1 : MonoBehaviour
{
    public TNHandler tnh;
    [SerializeField] private TMP_Text text;
    string a = "Unbeschränkter?";
    string b = "Eingeschränkter?";

    // Start is called before the first frame update
    void Start()
    {
        if (tnh.GetReduce()) text.SetText( a);
        else text.SetText(b);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pressed()
    {


        if (tnh.GetReduce())
        {
            text.SetText(b);
        }
        else text.SetText(a);
        tnh.changeReduce();
    }


}
