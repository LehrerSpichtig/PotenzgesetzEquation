using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
public class SliderScript : MonoBehaviour
{
    public MouseHandlerScript mhs;
    public Slider slider;
    public TMP_Text text;
    public GameObject cube;
    public GameObject sphere;
    private Vector3 pos;
    private List<GameObject> cubes;
    // Start is called before the first frame update
    void Start()
    {
        pos = slider.transform.position;
        cubes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value > 0  && slider.transform.position == new Vector3(0, 0, 30))
        {
            int bookk = mhs.NumberOfPath(slider.name);
            if (bookk == 0)
            {
                for (int i = 0; i < slider.value; i++)
                {
                    mhs.GenerateCubeII(slider.name, sphere.transform.position + new Vector3(0, -i, 0));
                }
            }
            else
            {
                if (bookk != slider.value) Debug.Log("ALARM: bookk = " + bookk + " while slider.value = " + slider.value);
            }
        }
    }

    public void GenerateCubes()
    {
        slider.transform.position = new Vector3(0,0,30);
        cubes = new List<GameObject>();
        for (int i = 0; i< slider.value; i++)
        {
            //GameObject c = Instantiate(cube);
            mhs.GenerateCubeII(slider.name, sphere.transform.position + new Vector3(0, -i, 0));

            
        }
    }

    public void BringBack()
    {
        DestroyCubes();
        slider.transform.position = pos;
    }

    public void DestroyCubes()
    {
        foreach (GameObject c in cubes) { Destroy(c); }
        cubes = new List<GameObject>();
    }

    public void MinOne()
    {
        if (slider.value > 0)
        {
            slider.value--;
        }
    }

    public void HandleSliderValueChanged()
    {
        text.SetText(slider.value.ToString());
    }

    public void SetValue(int v)
    {
        slider.value = v;
    }

    public int GetValue()
    {
        return (int)slider.value;
    }

}
