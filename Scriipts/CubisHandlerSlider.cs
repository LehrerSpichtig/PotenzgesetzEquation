using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
//using Unity.PlasticSCM.Editor.WebApi;
//using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Assertions.Must;

public class CubisHandlerSlider : MonoBehaviour
{
    //private Node<string> term;
    private bool isTriggered;
    private string targetName;

    public MouseHandlerScript BallController;
    public EquationGenerator EQG;
    public SliderScript sLeave,sFraction,sPGI,sPGII,sPGIII,sPot,sLift;

    public CubeSounder CubiSounder;
    public GameObject DispLeave, DispFract, DispPGI, DispPGII, DispPGIII, DispSqrt, DispLift/*, DispBaby*/;
    public List<int> LeaveList;
    public List<int> FractList;
    public List<int> PGIList;
    public List<int> PGIIList;
    public List<int> PGIIIList;
    public List<int> LiftList;
    public List<int> PotentiationList;
    public GameObject sphere;
    List<GameObject> spheres;
    private IEnumerator coroutine, coroutine2, coroutine3, coroutine4, coroutine5, coroutine6, coroutine7;
    private bool isHit;
    private List<GameObject> Displays;
    List<List<TextMeshPro>> leaveTexts;
    List<List<TextMeshPro>> fractionTexts;
    List<List<TextMeshPro>> pgITexts;
    List<List<TextMeshPro>> pgIITexts;
    List<List<TextMeshPro>> pgIIITexts;
    List<List<TextMeshPro>> potentiationTexts;
    List<List<TextMeshPro>> liftTexts;
    List<List<TextMeshPro>> rowTexts;
    public TextMeshPro text;
    Vector3 startpos;
    int nrLeaveOps = 0;
    int nrFracOps = 0;
    int nrPGIOps = 0;
    int nrPGIIOps = 0;
    int nrPGIIIOps = 0;
    int nrPotOps = 0;
    int nrLiftOps = 0;
    //bool standard;
    string aktuellerName;
    List<string> nrNames;


    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position;
        isTriggered = false;
        targetName = "test";
        isHit = false;
        Displays = new List<GameObject>();
        TextMeshPro texto = Instantiate(text);
        texto.fontSize = 3;
        texto.SetText("Rechnung");
        texto.transform.position = DispLeave.transform.position + Vector3.down * 0.5f;
        TextMeshPro texto2 = Instantiate(text);
        texto2.fontSize = 3;
        texto2.SetText("Bruch-\nrechnung");
        texto2.transform.position = DispFract.transform.position + Vector3.down * 0.5f;
        TextMeshPro texto3 = Instantiate(text);
        texto3.fontSize = 3;
        texto3.SetText("Potenz-\ngesetz I");
        texto3.transform.position = DispPGI.transform.position + Vector3.down * 0.5f;
        TextMeshPro texto4 = Instantiate(text);
        texto4.fontSize = 3;
        texto4.SetText("Potenz-\ngesetz II");
        texto4.transform.position = DispPGII.transform.position + Vector3.down * 0.5f;
        TextMeshPro texto5 = Instantiate(text);
        texto5.fontSize = 3;
        texto5.SetText("Potenz-\ngesetz III");
        texto5.transform.position = DispPGIII.transform.position + Vector3.down * 0.5f;
        TextMeshPro texto6 = Instantiate(text);
        texto6.fontSize = 3;
        texto6.SetText("Ver-/Ent-\nwurzeln");
        texto6.transform.position = DispSqrt.transform.position + Vector3.down * 0.5f;
        TextMeshPro texto7 = Instantiate(text);
        texto7.fontSize = 3;
        texto7.SetText("Exponent-\nnegation\n(Lift)");
        texto7.transform.position = DispLift.transform.position + Vector3.down * 0.75f;
        //standard = true;
        aktuellerName = "";
        spheres = new List<GameObject>();
        nrNames = new List<string>();
        for (int i = 0; i < LeaveList.Count + FractList.Count + PGIList.Count + PGIIList.Count + PGIIIList.Count + PotentiationList.Count + LiftList.Count; i++)
        {
            nrNames.Add(i.ToString());
        }




    }

    IEnumerator ShowDisplayTerms(List<List<TextMeshPro>> txts)
    {

        foreach (List<TextMeshPro> txt in txts)
        {
            foreach (TextMeshPro t in txt) { t.transform.position -= new Vector3(0, 0, 12); }
            yield return new WaitForSeconds(2);
            foreach (TextMeshPro t in txt) t.transform.position += new Vector3(0, 0, 12);
        }




    }



    // Update is called once per frame
    void Update()
    {




        if (transform.position != startpos)
        {


            if (BallController.GetSelectedCube() != null && BallController.GetBabyOn() /*&& currentPos != transform.position*/)
            {



                if (BallController.GetSelectedCube().name == this.name)
                {
                    if (!isHit)
                    {
                        if (spheres.Count == 0)
                        {
                            leaveTexts = EQG.GetOperationTerms(this.name, ChosenOperation("Leave"), DispLeave.transform.position + new Vector3(0, -2, 12));
                            fractionTexts = EQG.GetOperationTerms(this.name, ChosenOperation("Fraction"), DispFract.transform.position + new Vector3(0, -2, 12));
                            pgITexts = EQG.GetOperationTerms(this.name, ChosenOperation("PGI"), DispPGI.transform.position + new Vector3(0, -2, 12));
                            pgIITexts = EQG.GetOperationTerms(this.name, ChosenOperation("PGII"), DispPGII.transform.position + new Vector3(0, -2, 12));
                            pgIIITexts = EQG.GetOperationTerms(this.name, ChosenOperation("PGIII"), DispPGIII.transform.position + new Vector3(0, -2, 12));
                            potentiationTexts = EQG.GetOperationTerms(this.name, ChosenOperation("Potentiation"), DispSqrt.transform.position + new Vector3(0, -2, 12));
                            liftTexts = EQG.GetOperationTerms(this.name, ChosenOperation("Lift"), DispLift.transform.position + new Vector3(0, -2, 12));

                            nrLeaveOps = leaveTexts.Count;
                            coroutine = ShowDisplayTerms(leaveTexts);
                            StartCoroutine(coroutine);
                            nrFracOps = fractionTexts.Count;
                            coroutine2 = ShowDisplayTerms(fractionTexts);
                            StartCoroutine(coroutine2);
                            nrPGIOps = pgITexts.Count;
                            coroutine3 = ShowDisplayTerms(pgITexts);
                            StartCoroutine(coroutine3);
                            nrPGIIOps = pgIITexts.Count;
                            coroutine4 = ShowDisplayTerms(pgIITexts);
                            StartCoroutine(coroutine4);
                            nrPGIIIOps = pgIIITexts.Count;
                            coroutine5 = ShowDisplayTerms(pgIIITexts);
                            StartCoroutine(coroutine5);
                            nrPotOps = potentiationTexts.Count;
                            coroutine6 = ShowDisplayTerms(potentiationTexts);
                            StartCoroutine(coroutine6);
                            nrLiftOps = liftTexts.Count;
                            coroutine7 = ShowDisplayTerms(liftTexts);
                            StartCoroutine(coroutine7);

                        }


                        CubiSounder.Drive();

                        /*DispLeave.SetActive(false);
                        DispFract.SetActive(false);
                        DispPGI.SetActive(false);
                        DispSqrt.SetActive(false);
                        DispLift.SetActive(false);
                        DispPGII.SetActive(false);
                        DispPGIII.SetActive(false);*/
                        //DispBaby.SetActive(true);
                        isHit = true;
                    }


                }

            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (isHit)
                    {
                        if (Displays.Count > 0)
                        {
                            foreach (GameObject g in Displays)
                            {
                                Destroy(g);
                            }
                        }

                        //StopAllCoroutines();
                        cleanUp();

                        //StopAllCoroutines();
                        /*DispLeave.SetActive(true);
                        DispFract.SetActive(true);
                        DispPGI.SetActive(true);
                        DispSqrt.SetActive(true);
                        DispLift.SetActive(true);
                        DispPGII.SetActive(true);
                        DispPGIII.SetActive(true);*/
                        //DispBaby.SetActive(false);
                        isHit = false;
                    }

                }

            }
            if (isTriggered)
            {

                if (Input.GetMouseButtonUp(0))
                {
                    //sLeave.MinOne();
                    MinOne(this.name);
                    Debug.Log(this.name + " wurde in " + targetName + " abgelegt");
                    int nrOptions = nrOperationChoices(targetName);
                    if (nrOptions <= 1)
                    {
                        if (!nrNames.Contains(targetName)) EQG.SwitchingOrNot(targetName, ChosenOperation(this.name), targetName, PGIList, PGIIList, PGIIIList, PotentiationList, LiftList);
                        else
                        {
                            List<int> opss = new List<int>(); opss.Add(int.Parse(targetName));
                            EQG.SwitchingOrNot(this.name, opss, aktuellerName, PGIList, PGIIList, PGIIIList, PotentiationList, LiftList);
                        }
                        //StopAllCoroutines();
                        cleanUp();


                        //BallController.SwitchingOrNot(this.name + "\t" + targetName);

                        //standard = true;

                    }
                    else
                    {
                        aktuellerName = targetName;
                        //standard = false;
                        if (spheres.Count > 0)
                        {
                            foreach (GameObject s in spheres) Destroy(s);
                            spheres = new List<GameObject>();
                        }
                        List<int> options = EQG.GetOptions(this.name, ChosenOperation(targetName));
                        Vector3 sop = ChosenCalculation(targetName) + 2 * Vector3.down;
                        rowTexts = EQG.GetOperationTerms(this.name, ChosenOperation(targetName), sop);
                        List<(float, float, float, float, float, float, float, float, float)> data = EQG.GetData(this.name, ChosenOperation(targetName));
                        int cc = 0, ca = 1;
                        Vector3 down = Vector3.zero;
                        foreach (int i in options)
                        {
                            GameObject s = Instantiate(sphere);
                            s.name = i.ToString();
                            s.transform.position = sop;
                            //sop += Vector3.down;
                            Debug.Log(data[cc].ToString() + " Gesamthöhe " + data[cc].Item7);

                            foreach (var t in rowTexts[cc]) t.transform.position += down + (data[cc].Item1 + data[cc].Item2 + data[cc].Item3 + data[cc].Item4) / 4 * Vector3.right * ca;//t.transform.position += Vector3.down * (cc)+Vector3.right*ca;
                            sop += Vector3.down * Mathf.Max(4, data[cc].Item7) / 4;
                            down += Vector3.down * Mathf.Max(4, data[cc].Item7) / 4;
                            cc++;
                            ca *= (-1);
                            spheres.Add(s);
                        }



                    }
                    isTriggered = false;

                }


            }

        }



    }

    private void OnDestroy()
    {
        cleanUp();
    }

    private void cleanUp()
    {
        if (leaveTexts != null)
        {
            if (leaveTexts.Count > 0)
            {
                foreach (List<TextMeshPro> txt in leaveTexts) foreach (TextMeshPro t in txt) Destroy(t);
            }
            leaveTexts = new List<List<TextMeshPro>>();


        }
        if (fractionTexts != null)
        {
            if (fractionTexts.Count > 0)
            {
                foreach (List<TextMeshPro> txt in fractionTexts) foreach (TextMeshPro t in txt) Destroy(t);
            }
            fractionTexts = new List<List<TextMeshPro>>();

        }
        if (pgITexts != null)
        {
            if (pgITexts.Count > 0)
            {
                foreach (List<TextMeshPro> txt in pgITexts) foreach (TextMeshPro t in txt) Destroy(t);
            }
            pgITexts = new List<List<TextMeshPro>>();

        }
        if (pgIITexts != null)
        {
            if (pgIITexts.Count > 0)
            {
                foreach (List<TextMeshPro> txt in pgIITexts) foreach (TextMeshPro t in txt) Destroy(t);
            }
            pgIITexts = new List<List<TextMeshPro>>();
        }
        if (pgIIITexts != null)
        {
            if (pgIIITexts.Count > 0)
            {
                foreach (List<TextMeshPro> txt in pgIIITexts) foreach (TextMeshPro t in txt) Destroy(t);
            }
            pgIIITexts = new List<List<TextMeshPro>>();
        }
        if (potentiationTexts != null)
        {
            if (potentiationTexts.Count > 0)
            {
                foreach (List<TextMeshPro> txt in potentiationTexts) foreach (TextMeshPro t in txt) Destroy(t);
            }
            potentiationTexts = new List<List<TextMeshPro>>();
        }
        if (liftTexts != null)
        {
            if (liftTexts.Count > 0)
            {
                foreach (List<TextMeshPro> txt in liftTexts) foreach (TextMeshPro t in txt) Destroy(t);
            }
            liftTexts = new List<List<TextMeshPro>>();
        }
        if (spheres.Count > 0)
        {
            foreach (GameObject s in spheres) Destroy(s);
            spheres = new List<GameObject>();
        }
        if (rowTexts != null)
        {
            if (rowTexts.Count > 0)
            {
                foreach (List<TextMeshPro> txt in rowTexts) foreach (TextMeshPro t in txt) Destroy(t);
            }
            rowTexts = new List<List<TextMeshPro>>();
        }


    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision != null)
        {

            targetName = collision.name;
            //Debug.Log(collision.name + " is Triggered by " + this.name);
            isTriggered = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision != null)
        {
            //Debug.Log(this.name + " left " + collision.name);
            isTriggered = false;
        }
    }

    private void MinOne(string name)
    {
        switch (name)
        {
            case "Leave":
                sLeave.MinOne();break;
            case "Fraction":
                sFraction.MinOne(); break;
            case "PGI":
                sPGI.MinOne(); break;
            case "PGII":
                sPGII.MinOne(); break;
            case "PGIII":
                sPGIII.MinOne(); break;
            case "Potentiation":
                sPot.MinOne(); break;
            case "Lift":
                sLift.MinOne(); break;
            default:
                ;break;
        }

    }
    private List<int> ChosenOperation(string name)
    {
        switch (name)
        {
            case "Leave":
                return LeaveList;
            case "Fraction":
                return FractList;
            case "PGI":
                return PGIList;
            case "PGII":
                return PGIIList;
            case "PGIII":
                return PGIIIList;
            case "Potentiation":
                return PotentiationList;
            case "Lift":
                return LiftList;
            default:
                return new List<int>();
        }
    }



    private int nrOperationChoices(string name)
    {
        switch (name)
        {
            case "Leave":
                return nrLeaveOps;
            case "Fraction":
                return nrFracOps;
            case "PGI":
                return nrPGIOps;
            case "PGII":
                return nrPGIIOps;
            case "PGIII":
                return nrPGIIIOps;
            case "Potentiation":
                return nrPotOps;
            case "Lift":
                return nrLiftOps;
            default:
                return -1;
        }
    }

    private Vector3 ChosenCalculation(int name)
    {
        switch (name)
        {
            case 0:
                {
                    return DispLeave.transform.position;
                }
            case 1:
                {
                    return DispFract.transform.position; ;
                }
            case 2:
                {
                    return DispPGI.transform.position;
                }
            case 3:
                {
                    return DispPGII.transform.position;
                }
            case 4:
                {
                    return DispPGIII.transform.position;
                }
            case 5:
                {
                    return DispSqrt.transform.position;
                }
            case 6:
                {
                    return DispLift.transform.position;
                }
            default:
                {
                    return Vector3.zero;
                }
        }
    }

    private Vector3 ChosenCalculation(string name)
    {
        switch (name)
        {
            case "Leave":
                {
                    return DispLeave.transform.position;
                }
            case "Fraction":
                {
                    return DispFract.transform.position; ;
                }
            case "PGI":
                {
                    return DispPGI.transform.position;
                }
            case "PGII":
                {
                    return DispPGII.transform.position;
                }
            case "PGIII":
                {
                    return DispPGIII.transform.position;
                }
            case "Potentiation":
                {
                    return DispSqrt.transform.position;
                }
            case "Lift":
                {
                    return DispLift.transform.position;
                }
            default:
                {
                    return Vector3.zero;
                }
        }
    }

}
