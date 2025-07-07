using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Button2 : MonoBehaviour
{
    public MouseHandlerScript mhs;
    public EquationGenerator EQG;
    public GameObject DispLeave, DispFract, DispPGI, DispPGII, DispPGIII, DispSqrt, DispLift/*, DispBaby*/;
    [SerializeField] private TMP_Text text;
    string a = "Schwerer?";
    string b = "Einfacher?";
    List<List<TextMeshPro>> fixedLeaveTexts;
    List<List<TextMeshPro>> fixedFractTexts;
    List<List<TextMeshPro>> fixedPGITexts;
    List<List<TextMeshPro>> fixedPGIITexts;
    List<List<TextMeshPro>> fixedPGIIITexts;
    List<List<TextMeshPro>> fixedPotTexts;
    List<List<TextMeshPro>> fixedLiftTexts;
    private IEnumerator coroutine,coroutineF,coroutinePGI, coroutinePGII, coroutinePGIII,coroutinePot,coroutineLift;
    // Start is called before the first frame update
    void Start()
    {

        GenerateFixedTexts();
        

        if (mhs.GetBabyOn()) { text.SetText(a); }
        else
        {
            text.SetText(b);
            coroutine = ShowDisplayTermsLoop(fixedLeaveTexts);
            StartCoroutine(coroutine);
            coroutineF = ShowDisplayTermsLoop(fixedFractTexts);
            StartCoroutine(coroutineF);
            coroutinePGI = ShowDisplayTermsLoop(fixedPGITexts);
            StartCoroutine(coroutinePGI);
            coroutinePGII = ShowDisplayTermsLoop(fixedPGIITexts);
            StartCoroutine(coroutinePGII);
            coroutinePGIII = ShowDisplayTermsLoop(fixedPGIIITexts);
            StartCoroutine(coroutinePGIII);
            coroutinePot = ShowDisplayTermsLoop(fixedPotTexts);
            StartCoroutine(coroutinePot);
            coroutineLift = ShowDisplayTermsLoop(fixedLiftTexts);
            StartCoroutine(coroutineLift);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShowDisplayTermsLoop(List<List<TextMeshPro>> txts)
    {
        int i = 0;
        while (i<2)
        {
            foreach (List<TextMeshPro> txt in txts)
            {
                foreach (TextMeshPro t in txt) { t.transform.position -= new Vector3(0, 0, 12); }
                yield return new WaitForSeconds(1);
                foreach (TextMeshPro t in txt) t.transform.position += new Vector3(0, 0, 12);
            }
            i++;
        }

    }

    void sendTextBehindBackground()
    {
        foreach (List<TextMeshPro> txt in fixedLeaveTexts)
        {
            foreach (TextMeshPro t in txt) { Destroy(t); }

        }
        foreach (List<TextMeshPro> txt in fixedFractTexts)
        {
            foreach (TextMeshPro t in txt) { Destroy(t); }
        }
        foreach (List<TextMeshPro> txt in fixedPGITexts)
        {
            foreach (TextMeshPro t in txt) { Destroy(t); }
        }
        foreach (List<TextMeshPro> txt in fixedPGIITexts)
        {
            foreach (TextMeshPro t in txt) { Destroy(t); }
        }
        foreach (List<TextMeshPro> txt in fixedPGIIITexts)
        {
            foreach (TextMeshPro t in txt) { Destroy(t); }
        }
        foreach (List<TextMeshPro> txt in fixedPotTexts)
        {
            foreach (TextMeshPro t in txt) { Destroy(t); }
        }
        foreach (List<TextMeshPro> txt in fixedLiftTexts)
        {
            foreach (TextMeshPro t in txt) { Destroy(t); }
        }
        GenerateFixedTexts();
        
    }

    public void Pressed()
    {

        mhs.changeBabyOn();
        if (mhs.GetBabyOn())
        {
            text.SetText(a);
            StopAllCoroutines();
            sendTextBehindBackground();
        }
        else 
        { 
            text.SetText(b);
            coroutine = ShowDisplayTermsLoop(fixedLeaveTexts);
            StartCoroutine(coroutine);
            coroutineF = ShowDisplayTermsLoop(fixedFractTexts);
            StartCoroutine(coroutineF);
            coroutinePGI = ShowDisplayTermsLoop(fixedPGITexts);
            StartCoroutine(coroutinePGI);
            coroutinePGII = ShowDisplayTermsLoop(fixedPGIITexts);
            StartCoroutine(coroutinePGII);
            coroutinePGIII = ShowDisplayTermsLoop(fixedPGIIITexts);
            StartCoroutine(coroutinePGIII);
            coroutinePot = ShowDisplayTermsLoop(fixedPotTexts);
            StartCoroutine(coroutinePot);
            coroutineLift = ShowDisplayTermsLoop(fixedLiftTexts);
            StartCoroutine(coroutineLift);
        }

        
    }

    void GenerateFixedTexts()
    {
        fixedLeaveTexts = new List<List<TextMeshPro>>();
        Node<string> addition = new Node<string>("+");
        addition.left = new Node<string>("a");
        addition.right = new Node<string>("b");
        fixedLeaveTexts.Add(EQG.GetTerm(addition, DispLeave.transform.position + new Vector3(0, -2, 12)));
        Node<string> substr = new Node<string>("-");
        substr.left = new Node<string>("a");
        substr.right = new Node<string>("b");
        fixedLeaveTexts.Add(EQG.GetTerm(substr, DispLeave.transform.position + new Vector3(0, -2, 12)));
        Node<string> mult = new Node<string>("*");
        mult.left = new Node<string>("a");
        mult.right = new Node<string>("b");
        fixedLeaveTexts.Add(EQG.GetTerm(mult, DispLeave.transform.position + new Vector3(0, -2, 12)));
        Node<string> div = new Node<string>("/");
        div.left = new Node<string>("a");
        div.right = new Node<string>("b");
        fixedLeaveTexts.Add(EQG.GetTerm(div, DispLeave.transform.position + new Vector3(0, -2, 12)));

        fixedFractTexts = new List<List<TextMeshPro>>();
        Node<string> BAddition = new Node<string>("+");
        BAddition.left = new Node<string>("/");
        BAddition.right = new Node<string>("/");
        BAddition.left.left = new Node<string>("a");
        BAddition.left.right = new Node<string>("b");
        BAddition.right.left = new Node<string>("c");
        BAddition.right.right = new Node<string>("d");
        fixedFractTexts.Add(EQG.GetTerm(BAddition, DispFract.transform.position + new Vector3(0, -2.5f, 12)));
        Node<string> BAdded = new Node<string>("/");
        BAdded.left = new Node<string>("+");
        
        BAdded.left.left = new Node<string>("*");
        BAdded.left.left.left = new Node<string>("a");
        BAdded.left.left.right = new Node<string>("d");
        BAdded.left.right = new Node<string>("*");
        BAdded.left.right.left = new Node<string>("c");
        BAdded.left.right.right = new Node<string>("b");
   
        BAdded.right = new Node<string>("*");
        BAdded.right.left = new Node<string>("b");
        BAdded.right.right = new Node<string>("d");
        fixedFractTexts.Add(EQG.GetTerm(BAdded, DispFract.transform.position + new Vector3(0, -2.5f, 12)));

        Node<string> BMult = new Node<string>("*");
        BMult.left = new Node<string>("/");
        BMult.right = new Node<string>("/");
        BMult.left.left = new Node<string>("a");
        BMult.left.right = new Node<string>("b");
        BMult.right.left = new Node<string>("c");
        BMult.right.right = new Node<string>("d");
        fixedFractTexts.Add(EQG.GetTerm(BMult, DispFract.transform.position + new Vector3(0, -2.5f, 12)));

        Node<string> BMulted = new Node<string>("/");
        BMulted.left = new Node<string>("*");
        BMulted.right = new Node<string>("*");
        BMulted.left.left = new Node<string>("a");
        BMulted.left.right = new Node<string>("c");
        BMulted.right.left = new Node<string>("b");
        BMulted.right.right = new Node<string>("d");
        fixedFractTexts.Add(EQG.GetTerm(BMulted, DispFract.transform.position + new Vector3(0, -2.5f, 12)));


        fixedPGITexts = new List<List<TextMeshPro>>();
        Node<string> PotMul = new Node<string>("*");
        PotMul.left = new Node<string>("^");
        PotMul.right = new Node<string>("^");
        PotMul.left.left = new Node<string>("a");
        PotMul.left.right = new Node<string>("b");
        PotMul.right.left = new Node<string>("a");
        PotMul.right.right = new Node<string>("c");
        fixedPGITexts.Add(EQG.GetTerm(PotMul, DispPGI.transform.position + new Vector3(0, -1.5f, 12)));

        Node<string> PotMuled = new Node<string>("^");
        PotMuled.left = new Node<string>("a");
        PotMuled.right = new Node<string>("+");

        PotMuled.right.left = new Node<string>("b");
        PotMuled.right.right = new Node<string>("c");
        fixedPGITexts.Add(EQG.GetTerm(PotMuled, DispPGI.transform.position + new Vector3(0, -1.5f, 12)));

        Node<string> PotDiv = new Node<string>("/");
        PotDiv.left = new Node<string>("^");
        PotDiv.right = new Node<string>("^");
        PotDiv.left.left = new Node<string>("a");
        PotDiv.left.right = new Node<string>("b");
        PotDiv.right.left = new Node<string>("a");
        PotDiv.right.right = new Node<string>("c");
        fixedPGITexts.Add(EQG.GetTerm(PotDiv, DispPGI.transform.position + new Vector3(0, -1.5f, 12)));

        Node<string> PotDived = new Node<string>("^");
        PotDived.left = new Node<string>("a");
        PotDived.right = new Node<string>("-");

        PotDived.right.left = new Node<string>("b");
        PotDived.right.right = new Node<string>("c");
        fixedPGITexts.Add(EQG.GetTerm(PotDived, DispPGI.transform.position + new Vector3(0, -1.5f, 12)));

        fixedPGIITexts = new List<List<TextMeshPro>>();
        Node<string> Pot = new Node<string>("^");
        Pot.left = new Node<string>("^");
        Pot.right = new Node<string>("c");
        Pot.left.left = new Node<string>("a");
        Pot.left.right = new Node<string>("b");

        fixedPGIITexts.Add(EQG.GetTerm(Pot, DispPGII.transform.position + new Vector3(0, -2.5f, 12)));

        Node<string> PotM = new Node<string>("^");
        PotM.left = new Node<string>("a");
        PotM.right = new Node<string>("*");

        PotM.right.left = new Node<string>("b");
        PotM.right.right = new Node<string>("c");
        fixedPGIITexts.Add(EQG.GetTerm(PotM, DispPGII.transform.position + new Vector3(0, -2.5f, 12)));

        fixedPGIIITexts = new List<List<TextMeshPro>>();
        Node<string> PotMul2 = new Node<string>("*");
        PotMul2.left = new Node<string>("^");
        PotMul2.right = new Node<string>("^");
        PotMul2.left.left = new Node<string>("a");
        PotMul2.left.right = new Node<string>("c");
        PotMul2.right.left = new Node<string>("b");
        PotMul2.right.right = new Node<string>("c");
        fixedPGIIITexts.Add(EQG.GetTerm(PotMul2, DispPGIII.transform.position + new Vector3(0, -1.5f, 12)));

        Node<string> PotMuled2 = new Node<string>("^");
        PotMuled2.left = new Node<string>("*");
        PotMuled2.right = new Node<string>("c");

        PotMuled2.left.left = new Node<string>("a");
        PotMuled2.left.right = new Node<string>("b");
        fixedPGIIITexts.Add(EQG.GetTerm(PotMuled2, DispPGIII.transform.position + new Vector3(0, -1.5f, 12)));

        Node<string> PotDiv2 = new Node<string>("/");
        PotDiv2.left = new Node<string>("^");
        PotDiv2.right = new Node<string>("^");
        PotDiv2.left.left = new Node<string>("a");
        PotDiv2.left.right = new Node<string>("c");
        PotDiv2.right.left = new Node<string>("b");
        PotDiv2.right.right = new Node<string>("c");
        fixedPGIIITexts.Add(EQG.GetTerm(PotDiv2, DispPGIII.transform.position + new Vector3(0, -1.5f, 12)));

        Node<string> PotDived2 = new Node<string>("^");
        PotDived2.left = new Node<string>("/");
        PotDived2.right = new Node<string>("c");

        PotDived2.left.left = new Node<string>("a");
        PotDived2.left.right = new Node<string>("b");
        fixedPGIIITexts.Add(EQG.GetTerm(PotDived2, DispPGIII.transform.position + new Vector3(0, -1.5f, 12)));

        fixedPotTexts = new List<List<TextMeshPro>>();
        Node<string> Potent = new Node<string>("^");
        Potent.left = new Node<string>("a");
        Potent.right = new Node<string>("01.c");
        fixedPotTexts.Add(EQG.GetTerm(Potent, DispSqrt.transform.position + new Vector3(0, -3f, 12)));

        Node<string> Potent2 = new Node<string>("^");
        Potent2.left = new Node<string>("^");
        Potent2.left.left = new Node<string>("b");
        Potent2.left.right = new Node<string>("/");
        Potent2.left.right.left = new Node<string>("1");
        Potent2.left.right.right = new Node<string>("c");
        Potent2.right = new Node<string>("01.c");
        fixedPotTexts.Add(EQG.GetTerm(Potent2, DispSqrt.transform.position + new Vector3(0, -3f, 12)));
        fixedPotTexts.Add(EQG.GetTerm(Potent, DispSqrt.transform.position + new Vector3(0, -3f, 12)));
        Potent2 = new Node<string>("^");
        Potent2.left = new Node<string>("a");
        Potent2.right = new Node<string>("/");
        Potent2.right.left = new Node<string>("1");
        Potent2.right.right = new Node<string>("c");
        fixedPotTexts.Add(EQG.GetTerm(Potent2, DispSqrt.transform.position + new Vector3(0, -3f, 12)));

        fixedLiftTexts = new List<List<TextMeshPro>>();
        Node<string> l = new Node<string>("^");
        l.left = new Node<string>("a");
        l.right = new Node<string>("b");
        fixedLiftTexts.Add(EQG.GetTerm(l, DispLift.transform.position + new Vector3(0, -2f, 12)));
        l = new Node<string>("/");
        l.left = new Node<string>("1");
        l.right = new Node<string>("^");
        l.right.left = new Node<string>("a");
        l.right.right = new Node<string>("(-b)");
        fixedLiftTexts.Add(EQG.GetTerm(l, DispLift.transform.position + new Vector3(0, -2f, 12)));
        l = new Node<string>("/");
        l.left = new Node<string>("c");
        l.right = new Node<string>("^");
        l.right.left = new Node<string>("a");
        l.right.right = new Node<string>("b");
        fixedLiftTexts.Add(EQG.GetTerm(l, DispLift.transform.position + new Vector3(0, -2f, 12)));

        l = new Node<string>("*");
        l.left = new Node<string>("c");
        l.right = new Node<string>("^");
        l.right.left = new Node<string>("a");
        l.right.right = new Node<string>("(-b)");

        fixedLiftTexts.Add(EQG.GetTerm(l, DispLift.transform.position + new Vector3(0, -2f, 12)));
    }
}
