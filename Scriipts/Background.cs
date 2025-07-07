using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(Screen.width+2, Screen.height+2, 1);
        transform.position += new Vector3(0, 0, 4);
        Debug.Log(transform.localScale.ToString());
        Renderer canvasRenderer = GetComponent<Renderer>();
        Material newMaterial = new Material(Shader.Find("Sprites/Default"));
        newMaterial.color = new Color(1, 1, 1, 0.0f);
        canvasRenderer.material = newMaterial;
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePosition(bool turn)
    {
        if (turn) transform.position = new Vector3(0, 0, 0);
        else transform.position = new Vector3(0, 0, 4);

    }



}
