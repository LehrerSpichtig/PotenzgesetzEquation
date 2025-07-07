using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
//using Unity.VisualScripting.Antlr3.Runtime.Tree;
//using UnityEditor.ShaderKeywordFilter;

//using Unity.Burst.CompilerServices;
//using Unity.VisualScripting;
//using Unity.VisualScripting.Antlr3.Runtime.Tree;
//using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UIElements;
//using static UnityEditor.PlayerSettings;
public class EquationGenerator : MonoBehaviour
{
    
    public float bPG, bPotLift, b, c;
    private float payoff, totalPayoff;
    public List<string> terms;
    [SerializeField] public TextMeshPro text;
    public TMP_Text fbtext;
    public TMP_Text pText;
    public TMP_Text rText;
    public TMP_Text payoffText;
    List<TextMeshPro> texts;
    public bool feedback;
    //TextMeshPro feedbacktxt;
    //TextMeshPro shortestPathText;
    //TextMeshPro roundText;
    int round;
    public MouseHandlerScript mhs;
    public TNHandler TNH;
    public CubeSounder cb;
    //private List<string> cubeNamer;

    Node<string> current;
    List<int> rnge;
    List<int> PlayedGames;
    float Shift=1;
    bool suche;
    int sp = 0;
    void Start()
    {
        Shift = 1.0f;
        PlayedGames = new List<int>();
        rnge = new List<int>();
        for (int i = 0; i < TNH.GetFuncsLenght(); i++) rnge.Add(i);
        texts = new List<TextMeshPro>();
        //feedbacktxt = Instantiate(text);
        //feedbacktxt.fontSize /= 4;
        //feedbacktxt.transform.position = new Vector3(-4, 4, -2);
        //shortestPathText = Instantiate(text);
        //shortestPathText.fontSize /= 4;
        //shortestPathText.transform.position = new Vector3(1, 6, -2);
        //roundText = Instantiate(text);
        //roundText.fontSize /= 4;
        //roundText.transform.position = new Vector3(1, 5, -2);
        payoff = 0;
        totalPayoff = 0;
        int z = (int)Mathf.Floor(terms.Count * UnityEngine.Random.value);
        if (z == terms.Count) z--;
        while (PlayedGames.Contains(z))
        {
            z = (int)Mathf.Floor(terms.Count * UnityEngine.Random.value);
            if (z == terms.Count) z--;
        }
        PlayedGames.Add(z);
        if (z == 12) suche = false; else suche = true;
            string texto = terms[z];//@"\sqrt[4]{\sqrt[2]{256}}";@"\sqrt[3]{{{2}^{2}}}\cdot\sqrt[3]{2}";

        GenerateNewTerm(texto);
        payoffText.SetText("Payoff:\n" + ((int)payoff) + "\nTotal:\n" + ((int)totalPayoff));
    }

    void GenerateNewTerm(string texto)
    {
        Debug.Log("new Term is "+texto); 
        Node<string> t = CleanTree(LatexToExpressiontree2(texto));

        TNH.CreateNewNetwork(t);
        string paths = TreeMarching(t, "s", "");
        string[] p = (paths.Trim()).Split(" ");
        List<string> p2 = p.ToList();
        //cubeNamer = p2;
        current = t;
        int z = 100;
        Debug.Log("z = " + z);
        int shortestPath = 5;
        if (suche)
        {
            shortestPath = TNH.shortestPath(t, z);
            while (shortestPath == 0)
            {
                Debug.Log("z = " + z);
                z += 100;
                shortestPath = TNH.shortestPath(t, z);
            }
        }
        sp = shortestPath;    

        //shortestPathText.SetText("Kürzester Weg: "+shortestPath);
        pText.SetText("Kürzester Weg: " + shortestPath);
        //GenerateEquation3(t, Shift, text, returnDataList2(EquationData3(t)), p2, "s", Vector3.zero);
        GenerateEquation2(t, Shift, text, returnDataList2(EquationData2(t)), p2, "s", Vector3.zero);
        round = 0;
        //roundText.SetText("Anzahl Schritte: " + round);
        rText.SetText("Anzahl Schritte: " + round);
    }



    public List<TextMeshPro> GetTerm(Node<string> term,  Vector3 pos)
    {
        string paths = TreeMarching(term, "s", "");
        string[] p = (paths.Trim()).Split(" ");
        List<string> p2 = p.ToList();
        TextMeshPro t = Instantiate(text);
        t.fontSize /= 4;
        return GenerateEquation2Return(term, Shift/4, t, returnDataList2(EquationData2(term)), p2, "s", pos);
    }

    public List<(float,float,float,float,float,float,float,float,float)> GetEquationData(Node<string> term)
    {
        return returnDataList2(EquationData2(term));
    }
    
    public List<(float, float, float, float, float, float, float, float, float)> GetData(string path, List<int> op)
    {
        List<(float, float, float, float, float, float, float, float, float)> rnt = new List<(float, float, float, float, float, float, float, float, float)>();
        List<string> opp = new List<string>() { "*", "/", "+", "-", "^" };
        if (opp.Contains(current.key))
        {
            List<(Node<string>, int)> choice = TNH.Evaluation(current, path, op);
            foreach ((Node<string>,int) n in choice)
            {
                List<string> tree = (TreeMarching(n.Item1, "s", "").Trim()).Split(" ").ToList();
                int pi = tree.LastIndexOf("s");
                rnt.Add(GetEquationData(n.Item1)[pi]);
            }
        }

        return rnt;
    }

    public List<List<TextMeshPro>> GetOperationTerms(string name, List<int> op, Vector3 pos)
    {
        return OperationTerms(current, name, op, pos);
    }
    private List<List<TextMeshPro>> OperationTerms(Node<string> term,string path, List<int> op, Vector3 pos)
    {
        List<List<TextMeshPro>> list = new List<List<TextMeshPro>>();
        List<string> opp = new List<string>() { "*", "/", "+", "-", "^" };


        if (opp.Contains(term.key)){
            List<(Node<string>, int)> choice = TNH.Evaluation(current, path, op);

            TextMeshPro t = Instantiate(text);
            t.fontSize /= 4;

            List<string> terms = new List<string>();
            foreach ((Node<string>, int) node in choice)
            {
                string trm = TNH.TreeToExpression(node.Item1, true);
                if (!terms.Contains(trm))
                 {
                    terms.Add(trm);
                    List<TextMeshPro> txt = GenerateEquation2Return(node.Item1, Shift / 4, t, returnDataList2(EquationData2(node.Item1)), ((TreeMarching(node.Item1, "s", "").Trim()).Split(" ")).ToList(), "s", pos);
                    list.Add(txt);

                }

            }

        }



        return list;
    }

    public List<int> GetOptions(string path, List<int> op)
    {
        return Options(current, path, op);
    }
    private List<int> Options(Node<string> term, string path, List<int> op)
    {
        List<int> list = new List<int>();
        List<string> opp = new List<string>() { "*", "/", "+", "-", "^" };


        if (opp.Contains(term.key))
        {
            List<(Node<string>, int)> choice = TNH.Evaluation(current, path, op);

            foreach ((Node<string>, int) node in choice)
            {
                list.Add(node.Item2);
            }

        }
                return list;
    }

    public void SwitchingOrNot(string path, List<int> op, string binType, List<int> pgInts, List<int> pgIInts, List<int> pgIIInts, List<int> potInts, List<int> liftInts)
    {
        round++;
        //roundText.SetText("Anzahl Schritte: " + round);
        rText.SetText("Anzahl Schritte: " + round);
       
        List<string> opp = new List<string>() { "*", "/", "+", "-", "^" };

        List<(Node<string>, int)> choice;
        if (opp.Contains(current.key)) choice = TNH.Evaluation(current, path, op);
        else choice = new List<(Node<string>, int)>();

        if (choice.Count > 0)
        {// neuer Term
            cb.Win();
            if (binType.StartsWith("PG"))
            {
                payoff += bPG;
            }
            else if (binType.StartsWith("Pot") || binType.StartsWith("Lift"))
            {
                payoff += bPotLift;
            }
            else payoff += b;
                Node<string> parent = TNH.CopyNode(current);
            int r = UnityEngine.Random.Range(0, choice.Count);
            current = CleanTree(choice[r].Item1);
            Node<string> kind = TNH.CopyNode(current);
            string ss = TNH.feedback(kind, parent, path, binType, TreeMarchingStrict(parent, "s", ""), pgInts, pgIInts, pgIIInts, potInts, liftInts);
            if (feedback) { /*feedbacktxt.SetText(ss);*/ fbtext.SetText(ss); }
            foreach (var t in texts) { Destroy(t); }
            mhs.DestroyCubes();


            string paths = TreeMarching(current, "s", "");

            string[] p = (paths.Trim()).Split(" ");
            List<string> p2 = p.ToList();
            int pi = p2.LastIndexOf("s");
            //float hUnten = returnDataList2(EquationData2(current))[pi].Item7;
            Vector3 pos = Vector3.zero;
            (float, float, float) height = TermHeights(current);
            if (height.Item1 > 2.5f) pos += (height.Item1 -2.5f)*Vector3.up+Vector3.up;
            Debug.Log("Pos ist " + pos.ToString());
            if (current.right != null)
            {
                //GenerateEquation3(current, Shift, text, returnDataList2(EquationData3(current)), p2, "s", pos/*Vector3.zero*/);
                GenerateEquation2(current, Shift, text, returnDataList2(EquationData2(current)), p2, "s", pos/*Vector3.zero*/);

            }
            else
            {
                //GenerateEquation3(current, Shift, text, returnDataList2(EquationData3(current)), p2, "s", pos /*Vector3.zero*/);
                GenerateEquation2(current, Shift, text, returnDataList2(EquationData2(current)), p2, "s", pos /*Vector3.zero*/);
                mhs.GenerateCube(p[0], Vector3.zero);
            }


        }
        else
        {

            //string data = EquationData(current);
            string paths = TreeMarching(current, "s", "");


            string[] p = (paths.Trim()).Split(" ");
            List<string> p2 = p.ToList();

            string pathsStrict = TreeMarchingStrict(current, "s", "");
            List<string> p3 = ((pathsStrict.Trim()).Split(" ")).ToList();
            int count = 0;
            if (p3.Count > 0)
            {
                foreach (string pp in p3)
                {
                    
                    count += TNH.Evaluation(current, pp, rnge).Count;
                }

            }
            


            foreach (var t in texts) { Destroy(t); }
            mhs.DestroyCubes();

            if (count > 0)
            {// alter Term
                cb.Lose();
                payoff -= c;
                if (feedback) { /*feedbacktxt.SetText("Leider falsch!");*/ fbtext.SetText("Leider falsch!"); }
                Vector3 pos = Vector3.zero;
                (float, float, float) height = TermHeights(current);
                if (height.Item1 > 2.5f) pos += (height.Item1 - 2.5f) * Vector3.up + Vector3.up;
                GenerateEquation2(current, Shift, text, returnDataList2(EquationData2(current)), p2, "s", pos/* Vector3.zero*/);

            }
            else
            {// neues Game
                cb.Win();

                totalPayoff += payoff / sp;
                payoff = 0;

                if (feedback) { /*feedbacktxt.SetText("Neues Spiel");*/ fbtext.SetText("Neues Spiel"); }
                if (PlayedGames.Count == terms.Count) PlayedGames = new List<int>();
                int z = (int)Mathf.Floor(terms.Count * UnityEngine.Random.value);
                if (z == terms.Count) z--;
                while (PlayedGames.Contains(z))
                {
                    z = (int)Mathf.Floor(terms.Count * UnityEngine.Random.value);
                    if (z == terms.Count) z--;
                }
                PlayedGames.Add(z);
                if (z == 12) suche = false; else suche = true;
                Debug.Log("neues Spiel: " + z);
                Debug.Log("neues Spiel: " + terms[z]);
                GenerateNewTerm(terms[z]);

            }


        }
        payoffText.SetText("Payoff:\n" + ((int)payoff) + "\nTotal:\n" + ((int)totalPayoff));
        
    }

    public void ChangePosition(bool turn)
    {
        int sign = 1;
        if (turn) sign = -1;
        foreach (TextMeshPro t in texts) { if (t != null) t.transform.position += sign * new Vector3(0, 0, 2); }
    }
 

    private List<(float, float, float, float, float, float, float, float,float)> returnDataList2(string equationData)
    {
        List<(float, float, float, float, float, float, float, float, float)> rtn = new List<(float, float, float, float, float, float, float, float, float)>();

        string[] d = (equationData.Trim()).Split(" ");
        foreach (string s in d) 
        {
            rtn.Add(returnData2(s));
        }
        return rtn;
    }

    private (float, float, float, float, float, float, float,float,float) returnData2(string data)
    {
        string[] strings = data.Split("q");
        (float, float, float, float, float, float, float,float,float) d;
        d.Item1 = float.Parse(strings[0]);
        d.Item2 = float.Parse(strings[1]);
        d.Item3 = float.Parse(strings[2]);
        d.Item4 = float.Parse(strings[3]);
        d.Item5 = float.Parse(strings[4]);
        d.Item6 = float.Parse(strings[5]);
        d.Item7 = float.Parse(strings[6]);
        d.Item8 = float.Parse(strings[7]);
        d.Item9 = float.Parse(strings[8]);

        return d;
    }
    public string EquationData2(Node<string> term)
    {
        string rtn = "";
        List<string> op = new List<string>() { "*", "/", "+", "-", "^" };
        if (op.Contains(term.key))
        {
            string lft = EquationData2(term.left).Trim();
            string rgt = EquationData2(term.right).Trim();
            string[] ls = lft.Split(" ");
            string[] rs = rgt.Split(" ");
            string tl = TreeMarching(term.left, "s", "").Trim();
            string tr = TreeMarching(term.right, "s", "").Trim();
            List<string> ltl = tl.Split(" ").ToList();
            List<string> ltr = tr.Split(" ").ToList();
            var lData = returnData2(ls[ltl.LastIndexOf("s")]);
            var rData = returnData2(rs[ltr.LastIndexOf("s")]);

            switch (term.key)
            {
                case "+":
                    {
                        rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + 0 + "q" + (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4) + "q" + 0 + "q"+ 0 + "q" +0 + "q" + Mathf.Max(lData.Item7,rData.Item7)+"q"+1+"q"+0/*Mathf.Max(lData.Item9,rData.Item9)*/;
                        break;
                    }
                case "-":
                    {
                        int z = 0;
                        if (term.right.key == "+" || term.right.key == "-") z = 2;
        
                        rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + 0 + "q" + (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4) + "q" + z + "q" + 0 + "q" + 0 + "q" + Mathf.Max(lData.Item7, rData.Item7) + "q" + 1 + "q" + 0/*Mathf.Max(lData.Item9, rData.Item9)*/;
                        break;
                    }
                case "*":
                    {
                        int zl = 0;
                        int zr = 0;
                        if (term.left.key == "+" || term.left.key == "-") zl = 2;
                        if (term.right.key == "+" || term.right.key == "-") zr = 2;
                        rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + zl + "q" + (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4) + "q" + zr + "q" + 0 + "q" + 0 + "q" + Mathf.Max(lData.Item7, rData.Item7) + "q" + 1 + "q" + 0/*Mathf.Max(lData.Item9, rData.Item9)*/;
                        break;
                    }
                case "/":
                    {
                        rtn = (Mathf.Max((lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4), (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4))+ 2*(Mathf.Max(lData.Item9+1, rData.Item9+1)-1)) + "q" + 0 + "q" + 0 + "q" + 0 + "q" + lData.Item7 + "q" + rData.Item7 + "q" + (lData.Item7 + 1 + rData.Item7) + "q" + 0 + "q" + (Mathf.Max(lData.Item9, rData.Item9)+1);
                        break;
                    }
                case "^":
                    {
                        if (term.right.key == "0.5" || term.right.key.StartsWith("01."))
                        {
                            rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + 3 + "q" + 0 + "q" + 0 + "q" + 0 + "q" + 0 + "q" + lData.Item7+ "q" + 0 + "q" + 0/*lData.Item9*/;
                        }
                        else
                        {
                            int z = 0;
                            if (op.Contains(term.left.key)) z = 2;

                            rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + z + "q" + (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4) + "q" + 0 + "q" + lData.Item7 + "q" + rData.Item7 + "q" + (lData.Item7+ rData.Item7) + "q" + 0 + "q" + 0/*Mathf.Max(lData.Item9, rData.Item9)*/;
                            break;
                        }
                        break;
                    }
            }
            return lft + " " + rtn + " " + rgt;



        }
        else
        {       //längeLinks, Klammer, längeRechts, Klammer, HöheLinks, Höherechts, Gesamthöhe,Operationsize, #UnterBrüche
                rtn = term.key.Length + "q" + 0f + "q" + 0f + "q" + 0 + "q" + 0 + "q" + 0 + "q" + 1+"q"+0 + "q" + 0;
                return rtn;
        }
            
    }
    

    public void GenerateEquation2(Node<string> term, float shift, TextMeshPro text, List<(float,float,float, float, float, float,float,float,float)> data, List<string> tree, string path, Vector3 pos)
    {
        List<string> op = new List<string>() { "*", "/", "+", "-", "^" };
        TextMeshPro t = TextMeshPro.Instantiate(text);
        t.transform.position = pos;
        if (!op.Contains(term.key))
        {
            t.SetText(term.key);
            texts.Add(t);
            //Debug.Log(term.key + " at " + path);
        }
        else
        {
            int pi = tree.LastIndexOf(path);
            var d = data[pi];
            //Debug.Log(d.ToString() + " at " + path + " with " + term.key);
            if ((term.right.key == "0.5" && term.key == "^") || term.right.key.StartsWith("01."))
            {
                t.SetText("√");
                float size = d.Item8 + d.Item1 + d.Item2 + d.Item3 + d.Item4;
                t.transform.position = pos- new Vector3((size / 2 - 0.5f) * shift, 0, 0);

                if (term.right.key.StartsWith("01."))
                {
                    TextMeshPro srt = Instantiate(text);
                    srt.SetText(term.right.key.Substring(3));
                    srt.transform.position = t.transform.position + new Vector3(-0.15f * shift /** d.Item2*/, 0.6f * shift /** d.Item2*/, 0);
                    srt.fontSize = t.fontSize / 2;
                    texts.Add(srt);

                }
                TextMeshPro lBracket = Instantiate(text);
                lBracket.SetText("(");
                lBracket.transform.position = t.transform.position + Vector3.right * shift;
                texts.Add(lBracket);
                TextMeshPro rBracket = Instantiate(text);
                rBracket.SetText(")");
                rBracket.transform.position = pos + new Vector3((size / 2 - 0.5f) * shift, 0, 0);
                texts.Add(rBracket);
                GenerateEquation2(term.left, shift,text, data, tree, path + "l", pos + new Vector3(shift / 2, 0, 0));
            }
            else if (term.key == "/")
            {
                t.transform.position = pos;
                float size = d.Item1;
                string bruch = "─";
                //bruch = "-";
                string strich = "";
                for (int i = 0; i < 3 * size / 4; i++) strich = strich + bruch;
                t.SetText(strich);
                float sizeUp = Mathf.Max(1,d.Item5)*shift/2+shift/2;
                float sizeDown = Mathf.Max(1,d.Item6)*shift/2+shift/2;
                if (term.left.key == "^")
                {
                    if (!(term.left.right.key == "0.5" || term.left.right.key.StartsWith("01.")))
                    {
                        int pil = tree.LastIndexOf(path+"l");
                        var dl = data[pil];
                        //Halbe Höhe Basis
                        sizeUp = Mathf.Max(1, dl.Item5) * shift / 2 + shift / 2;
                    }
                }
                else if (term.left.key == "/")
                {
                    int pir = tree.LastIndexOf(path + "l");
                    var dr = data[pir];
                    //Bruchstrich Über Nenner und über aktuellen Brüchstrich
                    sizeUp = shift/2 + Mathf.Max(1, dr.Item6) * shift + shift / 2;
                }
                if (term.right.key == "^")
                {
                    if (!(term.right.right.key == "0.5" || term.right.right.key.StartsWith("01.")))
                    {
                        int pir = tree.LastIndexOf(path + "r");
                        var dr = data[pir];
                        //Ganze Höhe der Potenz runter und Halbe Höhe Basis rauf
                        sizeDown = Mathf.Max(2, dr.Item7) * shift + shift / 2-Mathf.Max(1,dr.Item5)*shift/2;
                    }
                }
                else if (term.right.key == "/")
                {
                    int pil = tree.LastIndexOf(path + "r");
                    var dl = data[pil];
                    //Bruchstrich unter Zähler und unter aktuellen Brüchstrich
                    sizeDown = shift / 2 + Mathf.Max(1, dl.Item5) * shift + shift / 2;
                }
                GenerateEquation2(term.left, shift,text, data, tree, path + "l", pos + new Vector3(0, sizeUp, 0));
                GenerateEquation2(term.right, shift,text, data, tree, path + "r", pos - new Vector3(0, sizeDown, 0));

            }
            else
            {
                Vector3 newPos = pos + new Vector3(((d.Item1+d.Item2) - (d.Item3+d.Item4))*shift / 2, 0, 0);
                t.transform.position = newPos;
                float sizeL = d.Item1 + d.Item2;
                float sizeR = d.Item3 + d.Item4;
                //Debug.Log(sizeL + " " + sizeR);
                Vector3 posL = newPos - new Vector3(sizeL / 2 * shift + d.Item8 * shift / 2, 0, 0);
                Vector3 posR = newPos + new Vector3(sizeR / 2 * shift + d.Item8 * shift / 2, 0, 0);
                switch (term.key)
                {
                    case "+":
                        {
                            t.SetText("+"); break;
                        }
                    case "-":
                        {
                            t.SetText("–"); break;
                        }
                    case "*":
                        {
                            t.SetText("∙"); break;
                        }
                    case "^":
                        {
                            t.SetText("");
                            if (term.left.key == "/")
                            {
                                int pil = tree.LastIndexOf(path + "l");
                                var dl = data[pil];
                                //Bruchstrich unter Zähler und unter aktuellen Brüchstrich
                                posR += new Vector3(0, (dl.Item5 + d.Item6) * shift / 2, 0);
                            }
                            else posR += new Vector3(0,(d.Item5+d.Item6)*shift/2, 0);
                            break;
                        }
                }
                if (d.Item2 == 2)
                {
                    TextMeshPro lBracket = Instantiate(text);
                    lBracket.SetText("(");
                    lBracket.transform.position = newPos - new Vector3(d.Item8 * shift / 2 + d.Item1 * shift + 1.5f*shift, 0, 0);//posL- new Vector3(sizeL*shift/2,0,0);
                    texts.Add(lBracket);
                    TextMeshPro rBracket = Instantiate(text);
                    rBracket.SetText(")");
                    rBracket.transform.position = newPos - new Vector3(d.Item8*shift / 2, 0, 0);//posL + new Vector3(sizeL*shift / 2, 0, 0);
                    texts.Add(rBracket);
                }
                if (d.Item4 == 2)
                {
                    TextMeshPro lBracket = Instantiate(text);
                    lBracket.SetText("(");
                    lBracket.transform.position = posR - new Vector3(sizeR*shift / 2, 0, 0);
                    texts.Add(lBracket);
                    TextMeshPro rBracket = Instantiate(text);
                    rBracket.SetText(")");
                    rBracket.transform.position = posR + new Vector3(sizeR*shift / 2, 0, 0);
                    texts.Add(rBracket);
                }
                GenerateEquation2(term.left, shift,text, data, tree, path + "l", posL);
                GenerateEquation2(term.right, shift,text, data, tree, path + "r", posR);
            }
            texts.Add(t);
            mhs.GenerateCube(path, t.transform.position);
        }

    }
    // return is (deepest distance down, deepest distance up, overall height)
    public (float,float,float) TermHeights(Node<string> term)
    {
        List<string> op = new List<string>() { "*", "/", "+", "-", "^" };
        if (!op.Contains(term.key))
        {
            return (0, 0, 1);
        }
        else 
        {
            (float, float, float) d = TermHeights(term.left);
            (float, float, float) e = TermHeights(term.right);
            ;
            if (term.key == "/") return (e.Item3, d.Item3, d.Item3 + 1 + e.Item3);
            else if (term.key == "^")
            {
                float height = d.Item3;
                Debug.Log(d.Item3 + " " + e.Item3 + " " + e.Item3 / 2);
                if ((d.Item3 + e.Item3/2)  > d.Item3) height = (d.Item3 + e.Item3/2)  ;
                return (d.Item1, e.Item2, height);
            }
            else return (Mathf.Max(d.Item1,e.Item1), Mathf.Max(d.Item2, e.Item2), Mathf.Max(d.Item3, e.Item3));
        }
    }

    public (float,float,float) TermHeights()
    {
        return TermHeights(current);
    }

    public List<TextMeshPro> GenerateEquation2Return(Node<string> term, float shift, TextMeshPro text, List<(float, float, float, float, float, float, float, float, float)> data, List<string> tree, string path, Vector3 pos)
    {
        List<TextMeshPro> rtn = new List<TextMeshPro>();
        List<string> op = new List<string>() { "*", "/", "+", "-", "^" };
        TextMeshPro t = TextMeshPro.Instantiate(text);
        t.transform.position = pos;
        if (!op.Contains(term.key))
        {
            t.SetText(term.key);
            //texts.Add(t);
            rtn.Add(t);
            //Debug.Log(term.key + " at " + path);
        }
        else
        {
            int pi = tree.LastIndexOf(path);
            var d = data[pi];
            //Debug.Log(d.ToString() + " at " + path + " with " + term.key);
            if ((term.right.key == "0.5" && term.key == "^") || term.right.key.StartsWith("01."))
            {
                t.SetText("√");
                float size = d.Item8 + d.Item1 + d.Item2 + d.Item3 + d.Item4;
                t.transform.position = pos - new Vector3((size / 2 - 0.5f) * shift, 0, 0);

                if (term.right.key.StartsWith("01."))
                {
                    TextMeshPro srt = Instantiate(text);
                    srt.SetText(term.right.key.Substring(3));
                    srt.transform.position = t.transform.position + new Vector3(-0.15f * shift /** d.Item2*/, 0.6f * shift /** d.Item2*/, 0);
                    srt.fontSize = t.fontSize / 2;
                    //texts.Add(srt);
                    rtn.Add(srt);

                }
                TextMeshPro lBracket = Instantiate(text);
                lBracket.SetText("(");
                lBracket.transform.position = t.transform.position + Vector3.right * shift;
                //texts.Add(lBracket);
                rtn.Add(lBracket);
                TextMeshPro rBracket = Instantiate(text);
                rBracket.SetText(")");
                rBracket.transform.position = pos + new Vector3((size / 2 - 0.5f) * shift, 0, 0);
                //texts.Add(rBracket);
                rtn.Add(rBracket);
                List<TextMeshPro> r = GenerateEquation2Return(term.left, shift, text, data, tree, path + "l", pos + new Vector3(shift / 2, 0, 0));
                foreach (var tz in r) rtn.Add(tz);
            }
            else if (term.key == "/")
            {
                t.transform.position = pos;
                float size = d.Item1;
                string bruch = "─";
                //bruch = "-";
                string strich = "";
                for (int i = 0; i < 3 * size / 4; i++) strich = strich + bruch;
                t.SetText(strich);
                float sizeUp = Mathf.Max(1, d.Item5) * shift / 2 + shift / 2;
                float sizeDown = Mathf.Max(1, d.Item6) * shift / 2 + shift / 2;
                if (term.left.key == "^")
                {
                    if (!(term.left.right.key == "0.5" || term.left.right.key.StartsWith("01.")))
                    {
                        int pil = tree.LastIndexOf(path + "l");
                        var dl = data[pil];
                        //Halbe Höhe Basis
                        sizeUp = Mathf.Max(1, dl.Item5) * shift / 2 + shift / 2;
                    }
                }
                else if (term.left.key == "/")
                {
                    int pir = tree.LastIndexOf(path + "l");
                    var dr = data[pir];
                    //Bruchstrich Über Nenner und über aktuellen Brüchstrich
                    sizeUp = shift / 2 + Mathf.Max(1, dr.Item6) * shift + shift / 2;
                }
                if (term.right.key == "^")
                {
                    if (!(term.right.right.key == "0.5" || term.right.right.key.StartsWith("01.")))
                    {
                        int pir = tree.LastIndexOf(path + "r");
                        var dr = data[pir];
                        //Ganze Höhe der Potenz runter und Halbe Höhe Basis rauf
                        sizeDown = Mathf.Max(2, dr.Item7) * shift + shift / 2 - Mathf.Max(1, dr.Item5) * shift / 2;
                    }
                }
                else if (term.right.key == "/")
                {
                    int pil = tree.LastIndexOf(path + "r");
                    var dl = data[pil];
                    //Bruchstrich unter Zähler und unter aktuellen Brüchstrich
                    sizeDown = shift / 2 + Mathf.Max(1, dl.Item5) * shift + shift / 2;
                }
                List<TextMeshPro> rt = GenerateEquation2Return(term.left, shift, text, data, tree, path + "l", pos + new Vector3(0, sizeUp, 0));
                foreach (var item in rt) rtn.Add(item);
                rt = GenerateEquation2Return(term.right, shift, text, data, tree, path + "r", pos - new Vector3(0, sizeDown, 0));
                foreach (var item in rt) rtn.Add(item);

            }
            else
            {
                Vector3 newPos = pos + new Vector3(((d.Item1 + d.Item2) - (d.Item3 + d.Item4)) * shift / 2, 0, 0);
                t.transform.position = newPos;
                float sizeL = d.Item1 + d.Item2;
                float sizeR = d.Item3 + d.Item4;
                //Debug.Log(sizeL + " " + sizeR);
                Vector3 posL = newPos - new Vector3(sizeL / 2 * shift + d.Item8 * shift / 2, 0, 0);
                Vector3 posR = newPos + new Vector3(sizeR / 2 * shift + d.Item8 * shift / 2, 0, 0);
                switch (term.key)
                {
                    case "+":
                        {
                            t.SetText("+"); break;
                        }
                    case "-":
                        {
                            t.SetText("–"); break;
                        }
                    case "*":
                        {
                            t.SetText("∙"); break;
                        }
                    case "^":
                        {
                            t.SetText("");
                            if (term.left.key == "/")
                            {
                                int pil = tree.LastIndexOf(path + "l");
                                var dl = data[pil];
                                //Bruchstrich unter Zähler und unter aktuellen Brüchstrich
                                posR += new Vector3(0, (dl.Item5 + d.Item6) * shift / 2, 0);
                            }
                            else posR += new Vector3(0, (d.Item5 + d.Item6) * shift / 2, 0);
                            break;
                        }
                }
                if (d.Item2 == 2)
                {
                    TextMeshPro lBracket = Instantiate(text);
                    lBracket.SetText("(");
                    lBracket.transform.position = newPos - new Vector3(d.Item8 * shift / 2 + d.Item1 * shift + 1.5f * shift, 0, 0);//posL- new Vector3(sizeL*shift/2,0,0);
                    //texts.Add(lBracket);
                    rtn.Add(lBracket);
                    TextMeshPro rBracket = Instantiate(text);
                    rBracket.SetText(")");
                    rBracket.transform.position = newPos - new Vector3(d.Item8 * shift / 2, 0, 0);//posL + new Vector3(sizeL*shift / 2, 0, 0);
                    //texts.Add(rBracket);
                    rtn.Add(rBracket);
                }
                if (d.Item4 == 2)
                {
                    TextMeshPro lBracket = Instantiate(text);
                    lBracket.SetText("(");
                    lBracket.transform.position = posR - new Vector3(sizeR * shift / 2, 0, 0);
                    //texts.Add(lBracket);
                    rtn.Add(lBracket);
                    TextMeshPro rBracket = Instantiate(text);
                    rBracket.SetText(")");
                    rBracket.transform.position = posR + new Vector3(sizeR * shift / 2, 0, 0);
                    //texts.Add(rBracket);
                    rtn.Add(rBracket);
                }
                List<TextMeshPro> rt  =GenerateEquation2Return(term.left, shift, text, data, tree, path + "l", posL);
                foreach (var item in rt) rtn.Add(item);
                rt = GenerateEquation2Return(term.right, shift, text, data, tree, path + "r", posR);
                foreach (var item in rt) rtn.Add(item);
            }
            //texts.Add(t);
            rtn.Add(t);
            //mhs.GenerateCube(path, t.transform.position);
        }
        return rtn;

    }

    public string EquationData3(Node<string> term)
    {
        string rtn = "";
        List<string> op = new List<string>() { "*", "/", "+", "-", "^" };
        if (op.Contains(term.key))
        {
            string lft = EquationData3(term.left).Trim();
            string rgt = EquationData3(term.right).Trim();
            string[] ls = lft.Split(" ");
            string[] rs = rgt.Split(" ");
            string tl = TreeMarching(term.left, "s", "").Trim();
            string tr = TreeMarching(term.right, "s", "").Trim();
            List<string> ltl = tl.Split(" ").ToList();
            List<string> ltr = tr.Split(" ").ToList();
            var lData = returnData2(ls[ltl.LastIndexOf("s")]);
            var rData = returnData2(rs[ltr.LastIndexOf("s")]);

            switch (term.key)
            {
                case "+":
                    {
                        rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + 0 + "q" + (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4) + "q" + 0 + "q" + 0 + "q" + 0 + "q" + Mathf.Max(lData.Item7, rData.Item7) + "q" + 1 + "q" + 0/*Mathf.Max(lData.Item9,rData.Item9)*/;
                        break;
                    }
                case "-":
                    {
                        float z = 0;
                        if (term.right.key == "+" || term.right.key == "-") z = 2* Mathf.Max(1,rData.Item7);

                        rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + 0 + "q" + (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4) + "q" + z + "q" + 0 + "q" + 0 + "q" + Mathf.Max(lData.Item7, rData.Item7) + "q" + 1 + "q" + 0/*Mathf.Max(lData.Item9, rData.Item9)*/;
                        break;
                    }
                case "*":
                    {
                        float zl = 0;
                        float zr = 0;
                        if (term.left.key == "+" || term.left.key == "-") zl = 2*Mathf.Max(1,lData.Item7);
                        if (term.right.key == "+" || term.right.key == "-") zr = 2*Mathf.Max(1,rData.Item7);
                        rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + zl + "q" + (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4) + "q" + zr + "q" + 0 + "q" + 0 + "q" + Mathf.Max(lData.Item7, rData.Item7) + "q" + 1 + "q" + 0/*Mathf.Max(lData.Item9, rData.Item9)*/;
                        break;
                    }
                case "/":
                    {
                        rtn = (Mathf.Max((lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4), (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4)) + 2 * (Mathf.Max(lData.Item9 + 1, rData.Item9 + 1) - 1)) + "q" + 0 + "q" + 0 + "q" + 0 + "q" + lData.Item7 + "q" + rData.Item7 + "q" + (lData.Item7 + 1 + rData.Item7) + "q" + 0 + "q" + (Mathf.Max(lData.Item9, rData.Item9) + 1);
                        break;
                    }
                case "^":
                    {
                        if (term.right.key == "0.5" || term.right.key.StartsWith("01."))
                        {
                            rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + Mathf.Max(1,lData.Item7) + "q" + 0 + "q" + 0 + "q" + 0 + "q" + 0 + "q" + lData.Item7 + "q" + 0 + "q" + 0/*lData.Item9*/;
                        }
                        else
                        {
                            float z = 0;
                            if (op.Contains(term.left.key)) z = 2*lData.Item7;

                            rtn = (lData.Item8 + lData.Item1 + lData.Item2 + lData.Item3 + lData.Item4) + "q" + z + "q" + (rData.Item8 + rData.Item1 + rData.Item2 + rData.Item3 + rData.Item4) + "q" + 0 + "q" + lData.Item7 + "q" + rData.Item7 + "q" + (lData.Item7 + rData.Item7) + "q" + 0 + "q" + 0/*Mathf.Max(lData.Item9, rData.Item9)*/;
                            break;
                        }
                        break;
                    }
            }
            return lft + " " + rtn + " " + rgt;



        }
        else
        {       //längeLinks, Klammer, längeRechts, Klammer, HöheLinks, Höherechts, Gesamthöhe,Operationsize, #UnterBrüche
            rtn = term.key.Length + "q" + 0f + "q" + 0f + "q" + 0 + "q" + 0 + "q" + 0 + "q" + 1 + "q" + 0 + "q" + 0;
            return rtn;
        }

    }

    public void GenerateEquation3(Node<string> term, float shift, TextMeshPro text, List<(float, float, float, float, float, float, float, float, float)> data, List<string> tree, string path, Vector3 pos)
    {
        List<string> op = new List<string>() { "*", "/", "+", "-", "^" };
        TextMeshPro t = TextMeshPro.Instantiate(text);
        t.transform.position = pos;
        if (!op.Contains(term.key))
        {
            t.SetText(term.key);
            texts.Add(t);
            //Debug.Log(term.key + " at " + path);
        }
        else
        {
            int pi = tree.LastIndexOf(path);
            var d = data[pi];
            //Debug.Log(d.ToString() + " at " + path + " with " + term.key);
            if ((term.right.key == "0.5" && term.key == "^") || term.right.key.StartsWith("01."))
            {
                t.SetText("√");
                t.fontSize *= d.Item2;
                Debug.Log("Fontsize = " + t.fontSize+" "+d.Item2);
                float size = d.Item8 + d.Item1 + d.Item2 + d.Item3 + d.Item4;
                t.transform.position = pos - new Vector3((size / 2 - 0.5f) * shift, 0, 0);

                if (term.right.key.StartsWith("01."))
                {
                    TextMeshPro srt = Instantiate(text);
                    srt.SetText(term.right.key.Substring(3));
                    srt.transform.position = t.transform.position + new Vector3(-0.15f * shift /** d.Item2*/, 0.6f * shift /** d.Item2*/, 0);
                    srt.fontSize = t.fontSize / 2;
                    texts.Add(srt);

                }
                /*TextMeshPro lBracket = Instantiate(text);
                lBracket.SetText("(");
                lBracket.transform.position = t.transform.position + Vector3.right * shift;
                texts.Add(lBracket);
                TextMeshPro rBracket = Instantiate(text);
                rBracket.SetText(")");
                rBracket.transform.position = pos + new Vector3((size / 2 - 0.5f) * shift, 0, 0);
                texts.Add(rBracket);*/
                GenerateEquation3(term.left, shift, text, data, tree, path + "l", pos + new Vector3(shift / 2, 0, 0));
            }
            else if (term.key == "/")
            {
                t.transform.position = pos;
                float size = d.Item1;
                string bruch = "─";
                //bruch = "-";
                string strich = "";
                for (int i = 0; i < 3 * size / 4; i++) strich = strich + bruch;
                t.SetText(strich);
                float sizeUp = Mathf.Max(1, d.Item5) * shift / 2 + shift / 2;
                float sizeDown = Mathf.Max(1, d.Item6) * shift / 2 + shift / 2;
                if (term.left.key == "^")
                {
                    if (!(term.left.right.key == "0.5" || term.left.right.key.StartsWith("01.")))
                    {
                        int pil = tree.LastIndexOf(path + "l");
                        var dl = data[pil];
                        //Halbe Höhe Basis
                        sizeUp = Mathf.Max(1, dl.Item5) * shift / 2 + shift / 2;
                    }
                }
                else if (term.left.key == "/")
                {
                    int pir = tree.LastIndexOf(path + "l");
                    var dr = data[pir];
                    //Bruchstrich Über Nenner und über aktuellen Brüchstrich
                    sizeUp = shift / 2 + Mathf.Max(1, dr.Item6) * shift + shift / 2;
                }
                if (term.right.key == "^")
                {
                    if (!(term.right.right.key == "0.5" || term.right.right.key.StartsWith("01.")))
                    {
                        int pir = tree.LastIndexOf(path + "r");
                        var dr = data[pir];
                        //Ganze Höhe der Potenz runter und Halbe Höhe Basis rauf
                        sizeDown = Mathf.Max(2, dr.Item7) * shift + shift / 2 - Mathf.Max(1, dr.Item5) * shift / 2;
                    }
                }
                else if (term.right.key == "/")
                {
                    int pil = tree.LastIndexOf(path + "r");
                    var dl = data[pil];
                    //Bruchstrich unter Zähler und unter aktuellen Brüchstrich
                    sizeDown = shift / 2 + Mathf.Max(1, dl.Item5) * shift + shift / 2;
                }
                GenerateEquation3(term.left, shift, text, data, tree, path + "l", pos + new Vector3(0, sizeUp, 0));
                GenerateEquation3(term.right, shift, text, data, tree, path + "r", pos - new Vector3(0, sizeDown, 0));

            }
            else
            {
                Vector3 newPos = pos + new Vector3(((d.Item1 + d.Item2) - (d.Item3 + d.Item4)) * shift / 2, 0, 0);
                t.transform.position = newPos;
                float sizeL = d.Item1 + d.Item2;
                float sizeR = d.Item3 + d.Item4;
                //Debug.Log(sizeL + " " + sizeR);
                Vector3 posL = newPos - new Vector3(sizeL / 2 * shift + d.Item8 * shift / 2, 0, 0);
                Vector3 posR = newPos + new Vector3(sizeR / 2 * shift + d.Item8 * shift / 2, 0, 0);
                switch (term.key)
                {
                    case "+":
                        {
                            t.SetText("+"); break;
                        }
                    case "-":
                        {
                            t.SetText("–"); break;
                        }
                    case "*":
                        {
                            t.SetText("∙"); break;
                        }
                    case "^":
                        {
                            t.SetText("");
                            if (term.left.key == "/")
                            {
                                int pil = tree.LastIndexOf(path + "l");
                                var dl = data[pil];
                                //Bruchstrich unter Zähler und unter aktuellen Brüchstrich
                                posR += new Vector3(0, (dl.Item5 + d.Item6) * shift / 2, 0);
                            }
                            else posR += new Vector3(0, (d.Item5 + d.Item6) * shift / 2, 0);
                            break;
                        }
                }
                if (d.Item2 >= 2)
                {
                    TextMeshPro lBracket = Instantiate(text);
                    (float, float, float) h = TermHeights(term.left);
                    lBracket.fontSize *= h.Item3;//d.Item2 / 2;
                    Debug.Log("Bracket Size = " + lBracket.fontSize+" "+h.Item3);
                    lBracket.SetText("(");
                    lBracket.transform.position = newPos - new Vector3(d.Item8 * shift / 2 + d.Item1 * shift + 1.5f * shift*h.Item3, 0, 0);//posL- new Vector3(sizeL*shift/2,0,0);
                    texts.Add(lBracket);
                    TextMeshPro rBracket = Instantiate(text);
                    rBracket.fontSize *= h.Item3;//d.Item2 / 2;
                    rBracket.SetText(")");
                    rBracket.transform.position = newPos - new Vector3(d.Item8 * shift / 2, 0, 0);//posL + new Vector3(sizeL*shift / 2, 0, 0);
                    texts.Add(rBracket);
                }
                if (d.Item4 >= 2)
                {
                    TextMeshPro lBracket = Instantiate(text);
                    lBracket.fontSize *= d.Item4 / 2;
                    lBracket.SetText("(");
                    lBracket.transform.position = posR - new Vector3(sizeR * shift / 2, 0, 0);
                    texts.Add(lBracket);
                    TextMeshPro rBracket = Instantiate(text);
                    rBracket.fontSize *= d.Item4 / 2;
                    rBracket.SetText(")");
                    rBracket.transform.position = posR + new Vector3(sizeR * shift / 2, 0, 0);
                    texts.Add(rBracket);
                }
                GenerateEquation3(term.left, shift, text, data, tree, path + "l", posL);
                GenerateEquation3(term.right, shift, text, data, tree, path + "r", posR);
            }
            texts.Add(t);
            mhs.GenerateCube(path, t.transform.position);
        }

    }

    private string TreeMarching(Node<string> tree, string path, string weg)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        if (op.Contains(tree.key))
        {
            return TreeMarching(tree.left, path + "l", weg) + path + " " + TreeMarching(tree.right, path + "r", weg);
        }
        else return path+" ";

    }

    private string TreeMarchingStrict(Node<string> tree, string path, string weg)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        if (op.Contains(tree.key))
        {
            return TreeMarchingStrict(tree.left, path + "l", weg)+ " " + path + " " + TreeMarchingStrict(tree.right, path + "r", weg);
        }
        else return "";

    }



    private Node<string> CleanTree(Node<string> tree)
    {
        string s = tree.key;
        if (s.StartsWith("0000")) { s = s.Replace("0000", "(-"); s = s + ")"; }
        Node<string> node = new Node<string>(s);
        if (tree.left != null) { node.left = CleanTree(tree.left); }
        if (tree.right != null) { node.right = CleanTree(tree.right); }
        return node;
    }

    public Node<string> LatexToExpressiontree2(string latexExpr)
    {
        string[] to = LatexToExpression2(latexExpr).Split(" ");


        List<string> tokens = new List<string>();
        bool negative = false;
        foreach (string t in to)
        {
            if (t.StartsWith("ç"))
            {
                string s = t.Replace("ç", "0000");
                negative = true;
                //s = s.Remove(s.Length - 1);
                tokens.Add(s);
            }
            else
            {
                if (negative)
                {
                    negative = false;
                }
                else
                {
                    tokens.Add(t);
                }
            }
        }

        List<string> infixToPostfix = InfixToPostfix(tokens);
        string rp = "";
        foreach (string token in infixToPostfix)
        {
            rp += token + " ";
        }
        //Console.WriteLine(rp);
        Node<string> tree = ConstructTree(infixToPostfix);


        return tree;


    }

    // transformiert einen Latexterm in einen Term von der Form (1 + 2 * 3)/((2)^2 - 3)
    public string LatexToExpression2(String text)
    {
        text = text.Replace("(-", "ç");
        text = text.Replace(@"\cdot", " * ");
        text = text.Replace("+", " + ");
        text = text.Replace("-", " - ");
        text = text.Replace("(", " ( ");
        text = text.Replace(")", " ) ");

        while (text.Contains(@"\frac{"))
        {
            int i = text.IndexOf(@"\frac{");
            int j = ComplementaryCurlyBracket(i + 5, text);
            int k = ComplementaryCurlyBracket(j + 1, text);
            text = text.Substring(0, i) + " ( " + text.Substring(i + 6, j - (i + 6)) + " ) / ( " + text.Substring(j + 2, k - (j + 2)) + " ) " + text.Substring(k + 1);
        }
        while (text.Contains(@"\sqrt{"))
        {
            int i = text.IndexOf(@"\sqrt{");
            int j = ComplementaryCurlyBracket(i + 5, text);
            text = text.Substring(0, i) + " ( " + text.Substring(i + 6, j - (i + 6)) + " ) ^ 0.5 " + text.Substring(j + 1);
        }
        while (text.Contains(@"\sqrt["))
        {
            int i = text.IndexOf(@"\sqrt[");
            string follow = text.Substring(i + 6);
            int u = follow.IndexOf("]");
            string nTe = follow.Substring(0, u);
            int j = ComplementaryCurlyBracket(i + 6 + u + 1, text);
            text = text.Substring(0, i) + " ( " + text.Substring(i + 6 + u + 2, j - (i + 6 + u + 2)) + " ) ^ 01." + nTe + " " + text.Substring(j + 1);
        }
        while (text.Contains("{{"))
        {
            int i = text.IndexOf("{{");
            int j = ComplementaryCurlyBracket(i + 1, text);
            int k = ComplementaryCurlyBracket(j + 2, text);
            text = text.Substring(0, i) + " ( " + text.Substring(i + 2, j - (i + 2)) + " ) ^ ( " + text.Substring(j + 3, k - (j + 3)) + " ) " + text.Substring(k + 2);
        }
        text = text.Replace("  ", " ");
        text = text.Trim();
        return text;
    }

    // Finkdet in einem Latexterm die jeweils zugehörige geschwungene Klammer
    public int ComplementaryCurlyBracket(int i, String text)
    {
        int j = 1;
        char[] chars = text.ToCharArray();
        bool test = true;
        while (test)
        {
            i++;
            if (chars[i] == '{')
            {
                j++;
            }
            if (chars[i] == '}')
            {
                j--;
                if (j == 0) test = false;
            }
        }
        return i;
    }


    // transforms the infix notation into a postfix notation (reverse Polish notation) by means of the shunting yard algorithm
    private List<string> InfixToPostfix(List<string> tokens)
    {
        List<string> stack = new List<string>();
        List<string> postfix = new List<string>();

        Regex regex = new Regex(@"^\d+(\.\d+)?$");

        foreach (string token in tokens)
        {
            if (regex.IsMatch(token))
            {// ATTENTION: Variables are not included yet
                postfix.Add(token);
            }
            else if (token.Equals("("))
            {
                stack.Add(token);
            }
            else if (token.Equals(")"))
            {
                while (stack.Count > 0 && !stack[stack.Count - 1].Equals("("))
                {
                    postfix.Add(stack[stack.Count - 1]);
                    stack.RemoveAt(stack.Count - 1);
                }
                if (stack.Count > 0 && stack[stack.Count - 1].Equals("("))
                {
                    stack.RemoveAt(stack.Count - 1);
                }
            }
            else
            {
                while (stack.Count > 0 && Precedence(stack[stack.Count - 1]) >= Precedence(token))
                {
                    postfix.Add(stack[stack.Count - 1]);
                    stack.RemoveAt(stack.Count - 1);
                }
                stack.Add(token);
            }
        }
        while (stack.Count > 0)
        {
            postfix.Add(stack[stack.Count - 1]);
            stack.RemoveAt(stack.Count - 1);
        }
        return postfix;
    }


    //Constrution of the binary algebraic tree from the postfix notation
    private Node<string> ConstructTree(List<string> postfix_tokens)
    {
        List<Node<string>> stack = new(); // List<Node<string>>();
        Regex regex = new Regex(@"^\d+(\.\d+)?$");
        foreach (string token in postfix_tokens)
        {
            if (regex.IsMatch(token))
            {
                stack.Add(new Node<string>(token));
            }
            else
            {
                if (stack.Count < 2)
                {
                    Console.WriteLine("Mathematisch inkorrekte Eingabe! Noch sicherstellen");
                    return stack[stack.Count - 1];
                }
                Node<string> r = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                Node<string> l = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                Node<string> node = new Node<string>(token);
                node.left = l;
                node.right = r;
                stack.Add(node);
            }
        }
        if (!(stack.Count == 1))
        {
            Console.WriteLine("Unbalanced expression");
            return stack[0];
        }

        return stack[stack.Count - 1];
    }

    // transforms the the latex expression into a more general mathematical notation
    // e.g. \frac{2+3*4}{6+7}+2*3  => (2 + 3 * 4) / (6 + 7) + 2 * 3   
    private static List<string> Tokenize(string latex_expr)
    {
        //https://regex101.com/
        string regexPattern = @"\\frac{[^{}]+}{[^{}]+}|\\sqrt{[^{}]+}|{{.*}\^{.*}}|\\[a-zA-Z]|\(-[0-9]+(\.[0-9]+)?\)|[0-9]+(\.[0-9]+)?|[+\-*/^(){}]"; //@"\\frac{[^{}]+}{[^{}]+}|\\sqrt{[^{}]+}|{{[^{}]+}\^{[^{}]+}}|\\[a-zA-Z]+|[0-9]+(\.[0-9]+)?|[+\-*/^(){}]";
        Regex regex = new(regexPattern);

        Regex regexHelp = new(@"{{.*}\^{[^{}]+}");//new(@"{{[^{}]+}\^{[^{}]+}}");
                                                  //string inputString //@"\sqrt{2.4+3 * 5.412} * {(10.1-x)}^{3.32}";

        MatchCollection tokens = regex.Matches(latex_expr);
        var processed_tokens = new List<string> { };

        foreach (Match match in tokens)
        {
            string token = match.ToString();
            //Console.WriteLine(token);¨
            if (token.StartsWith("(-"))
            {
                token = token.Replace("(-", "0000");
                token = token.Replace(")", "");
                processed_tokens.Add(token);
            }


            else if (token.StartsWith("\\frac"))
            {
                processed_tokens.Add("(");
                //ACHTUNG BEI SQRT IST ES 6 und HIER IST ES 5
                var numerator = Tokenize(token.Substring(6, token.IndexOf("}") - 6));
                processed_tokens.AddRange(numerator);
                processed_tokens.Add(")");
                processed_tokens.Add("/");
                processed_tokens.Add("(");
                var denominator = Tokenize(token.Substring(token.IndexOf("}") + 2, token.Length - token.IndexOf("}") - 3));
                processed_tokens.AddRange(denominator);

                processed_tokens.Add(")");

            }
            else if (token.StartsWith("\\sqrt"))
            {
                processed_tokens.Add("(");

                var inner_expr = Tokenize(token.Substring(6, token.IndexOf("}") - 6));
                processed_tokens.AddRange(inner_expr);

                processed_tokens.Add(")");
                processed_tokens.Add("^");
                processed_tokens.Add("0.5");

            }
            else if (regexHelp.IsMatch(token))
            {
                processed_tokens.Add("(");


                string ie = token.Substring(2, token.LastIndexOf("}^{") - 2);
                if (ie.Contains("}^{"))
                {
                    ie = "{" + ie + "}";

                }

                var inner_expr = Tokenize(ie);

                processed_tokens.AddRange(inner_expr);

                processed_tokens.Add(")");
                processed_tokens.Add("^");
                string exponent = token.Substring(token.LastIndexOf("}^{") + 3);
                string exponentClear = exponent.Remove(exponent.IndexOf("}"));
                processed_tokens.Add(exponentClear);
            }
            else
            {
                processed_tokens.Add(token);
            }
        }
        return processed_tokens;
    }


    private static int Precedence(string op)
    {
        if (op == "+" || op == "-") return 1;
        if (op == "*" || op == "/") return 2;
        if (op == "^") return 3;
        return 0;
    }

    public bool GetFeedbackValue()
    {
        return feedback;
    }
    public void changeFeedbackValue()
    {
        feedback = !feedback;
    }

    public void SetFeedbackToZero()
    {
        /*feedbacktxt.SetText("");*/
        fbtext.SetText("");
    }
}

