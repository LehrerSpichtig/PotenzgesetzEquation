using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;
public class Button4 : MonoBehaviour
{
    public SliderScript sliderLeave,sliderFraction,sliderPGI,sliderPGII,sliderPGIII,sliderPot,sliderLift;
    
    [SerializeField] private TMP_Text text;
    string a = "Spielen?";
    string b = "Neu Setzen?";
    private bool spielen;
    // Start is called before the first frame update
    void Start()
    {
        spielen = true;
        text.SetText(a);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pressed()
    {


        if (!spielen)
        {

            text.SetText(a);
            sliderLeave.BringBack();
            sliderFraction.BringBack();
            sliderPGI.BringBack();
            sliderPGII.BringBack();
            sliderPGIII.BringBack();
            sliderPot.BringBack();
            sliderLift.BringBack();
        }
        else
        {
            text.SetText(b);
            //sliderLeave.DestroyCubes();
            sliderLeave.GenerateCubes();
            sliderFraction.GenerateCubes();
            sliderPGI.GenerateCubes();
            sliderPGII.GenerateCubes();
            sliderPGIII.GenerateCubes();
            sliderPot.GenerateCubes();
            sliderLift.GenerateCubes();

        }
        spielen = !spielen;
        
    }
}

