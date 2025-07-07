using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float speed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("d"))
        {
            
            transform.position += Vector3.right * speed;// * Time.deltaTime;
        }
        if (Input.GetKeyDown("a"))
        {
            transform.position += Vector3.left * speed;// * Time.deltaTime;
        }
        if (Input.GetKeyDown("w"))
        {
            transform.position += Vector3.up * speed;// * Time.deltaTime;
        }
        if (Input.GetKeyDown("s"))
        {
            transform.position += Vector3.down * speed;// * Time.deltaTime;
        }
    }
}
