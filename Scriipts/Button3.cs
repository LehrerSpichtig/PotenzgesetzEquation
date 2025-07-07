using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Button3 : MonoBehaviour
{
    public EquationGenerator eg;
    [SerializeField] private TMP_Text text;
    string a = "Feedback?";
    string b = "Kein Feedback?";
    // Start is called before the first frame update
    void Start()
    {
        if (eg.GetFeedbackValue()) text.SetText(b);
        else text.SetText(a);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pressed()
    {


        if (eg.GetFeedbackValue())
        {
            eg.SetFeedbackToZero();
            text.SetText(a);
        }
        else text.SetText(b);
        eg.changeFeedbackValue();
    }
}
