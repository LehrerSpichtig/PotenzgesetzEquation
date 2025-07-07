using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MouseHandlerScript : MonoBehaviour
{
    private GameObject selectedCube;
    private bool BabyOn;
    List<GameObject> cubes;
    public GameObject cubis;
    public GameObject cubisII;
    public bool showCubes;
    public Background bg;
    public EquationGenerator eg;
    bool turn;
    public void GenerateCube(string path, Vector3 position)
    {
        GameObject cube = Instantiate(cubis);
        cube.SetActive(showCubes);
        cube.name = path;
        cube.transform.position = position;
        cubes.Add(cube);
    }

    public void GenerateCubeII(string path, Vector3 position)
    {
        GameObject cube = Instantiate(cubisII);
        cube.SetActive(showCubes);
        cube.name = path;
        cube.transform.position = position;
        cubes.Add(cube);
    }

    public int NumberOfPath(string path)
    {
        int rtn = 0;
        if (cubes != null)
        {
            foreach (GameObject cube in cubes)
            {
                if (cube.name == path) rtn++;
            }

        }
        return rtn;
    }

    // Start is called before the first frame update
    void Start()
    {
        cubes = new List<GameObject>();
        turn = true;
        BabyOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
    }

    public void DestroyCubes()
    {
        foreach (GameObject c in cubes)
        {
            
            Destroy(c);
        }
        cubes = new List<GameObject>();
    }

    void HandleMouseInput()
    {
        // Handle mouse button press
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {

            // Check if the mouse is over any cube
            RaycastHit rch;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rch))
            {

                if (rch.collider != null && cubes.Contains(rch.collider.gameObject))
                {
                    bg.ChangePosition(true);
                    if (turn) { eg.ChangePosition(true); turn = false; }
                    selectedCube = rch.collider.gameObject;
                
                }

            }

        }

        // Handle mouse button release
        if (Input.GetMouseButtonUp(0))
        {
            selectedCube = null;
            bg.ChangePosition(false);
            if (!turn) { eg.ChangePosition(false); turn = true; }
        }

        // Handle dragging
        if (selectedCube != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit rch))
            {
                Vector3 pnt = rch.point;
                pnt.z = 0;
                selectedCube.transform.position = pnt;

            }
        }
    }

    public GameObject GetSelectedCube()
    {
        return selectedCube;
    }

    public bool GetBabyOn()
    {
        return BabyOn;
    }

    public void changeBabyOn()
    {
        BabyOn = !BabyOn;

    }


}
