using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class TNHandler : MonoBehaviour
{
    public bool reduce;
    public TextMeshPro text;
    List<Func<Node<string>, string, string, Node<string>>> funcs;
    //TextMeshPro textBefore;
    //TextMeshPro textAfter;
    List<string> networkNodes;
    List<List<string>> networkEdges;
    public void Start()
    {
        funcs  = new List<Func<Node<string>, string, string, Node<string>>>() {LeaveStrictInt,LeaveKuerzen,
        Potentiation,PotentiationSqrt,LeaveSqrt,
        FractionAddition,FractionMult,
        PotenzGesetzI,PotenzGesetzIleftExpOne, PotenzGesetzIrightExpOne,PotenzGesetzIinnerProduct,PotenzGesetzISquare,PotenzGesetzIrueckwaerts,
        PotenzGesetzII,PotenzGesetzIIrueckwaerts,PotenzGesetzIIrueckwaertsDivision,
        PotenzGesetzIII,PotenzGesetzIIIinnerProduct,PotenzGesetzIIIrueckwaerts,
        PotenceToRoot,RootToPotence,
        LiftDownNew,LiftDownFree,LiftUpNew,
        MultiplyPotenceWithNumberOnLeftside,MultiplyPotenceWithNumberOnRightside};
        networkNodes = new List<string>();
        networkEdges = new List<List<string>>();
        //textBefore = Instantiate(text);
        //textAfter = Instantiate(text);
        //textBefore.fontSize /= 2;
        //textAfter.fontSize /= 2;
        //textBefore.transform.position = new Vector3(2, 4, 0);
        //textAfter.transform.position = new Vector3(2, 3, 0);
    }


    public string feedback(Node<string> kind, Node<string> parent, string path, string binType,string parentPaths, List<int> pgIInts, 
        List<int> pgIIInts, List<int> pgIIIInts, List<int> potInts, List<int> liftInts)
    {
        if (!addToNetwork(kind, parent)) return "Neutral!";
        else
        {
            if (binType.StartsWith("PG")) return "Beste Wahl!";
            string feedbak = "Beste Wahl!";
            bool PotOrLift = binType == "Potentiation" || binType == "Lift";
            List<string> ppaths = parentPaths.Trim().Split(" ").ToList();
            int pos = ppaths.IndexOf(path);
            int wurfel = UnityEngine.Random.Range(0, 1);
            if (wurfel == 0)
            {
                for (int i = 0; i < pos; i++)
                {
                    if (Evaluation(parent, ppaths[i], pgIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGI";
                    if (Evaluation(parent, ppaths[i], pgIIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGII";
                    if (Evaluation(parent, ppaths[i], pgIIIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, ppaths[i], potInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, ppaths[i], liftInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf Lift";
                    }
                }

                
                {
                    if (Evaluation(parent, path, pgIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGI";
                    if (Evaluation(parent, path, pgIIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGII";
                    if (Evaluation(parent, path, pgIIIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, path, potInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, path, liftInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf Lift";
                    }
                }

                for (int i = pos + 1; i < ppaths.Count; i++)
                {
                    if (Evaluation(parent, ppaths[i], pgIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGI";
                    if (Evaluation(parent, ppaths[i], pgIIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGII";
                    if (Evaluation(parent, ppaths[i], pgIIIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, ppaths[i], potInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, ppaths[i], liftInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf Lift";
                    }

                }
            }
            else if (wurfel == 1)
            {
                for (int i = pos + 1; i < ppaths.Count; i++)
                {
                    if (Evaluation(parent, ppaths[i], pgIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGI";
                    if (Evaluation(parent, ppaths[i], pgIIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGII";
                    if (Evaluation(parent, ppaths[i], pgIIIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, ppaths[i], potInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, ppaths[i], liftInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf Lift";
                    }

                }
                for (int i = 0; i < pos; i++)
                {
                    if (Evaluation(parent, ppaths[i], pgIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGI";
                    if (Evaluation(parent, ppaths[i], pgIIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGII";
                    if (Evaluation(parent, ppaths[i], pgIIIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, ppaths[i], potInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, ppaths[i], liftInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf Lift";
                    }
                }

                {
                    if (Evaluation(parent, path, pgIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGI";
                    if (Evaluation(parent, path, pgIIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGII";
                    if (Evaluation(parent, path, pgIIIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, path, potInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, path, liftInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf Lift";
                    }
                }

            }
            else
            {
                {
                    if (Evaluation(parent, path, pgIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGI";
                    if (Evaluation(parent, path, pgIIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGII";
                    if (Evaluation(parent, path, pgIIIInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, path, potInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, path, liftInts).Count() > 0) return "Bessere Wahl mit gleichem Operator auf Lift";
                    }
                }

                for (int i = pos + 1; i < ppaths.Count; i++)
                {
                    if (Evaluation(parent, ppaths[i], pgIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGI";
                    if (Evaluation(parent, ppaths[i], pgIIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGII";
                    if (Evaluation(parent, ppaths[i], pgIIIInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, ppaths[i], potInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, ppaths[i], liftInts).Count() > 0) return "Bessere Wahl mit nachherigem Operator auf Lift";
                    }

                }
                for (int i = 0; i < pos; i++)
                {
                    if (Evaluation(parent, ppaths[i], pgIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGI";
                    if (Evaluation(parent, ppaths[i], pgIIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGII";
                    if (Evaluation(parent, ppaths[i], pgIIIInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf PGIII";
                    if (!PotOrLift)
                    {
                        if (Evaluation(parent, ppaths[i], potInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf Ver/Entwurzeln";
                        if (Evaluation(parent, ppaths[i], liftInts).Count() > 0) return "Bessere Wahl mit vorherigem Operator auf Lift";
                    }
                }

            }

                return feedbak;
        }
    }

    public void CreateNewNetwork(Node<string> start)
    {
        if (start != null)
        {
            networkNodes.Clear();
            networkNodes = new List<string>();
            networkNodes.Add(TreeToExpression(start,true));
            networkEdges.Clear();
            networkEdges.Add(new List<string>());
        }
    }

    // returns false if the network already contains the link. returns true otherwise and adds the node and edge to the network
    private bool addToNetwork(Node<string> neu, Node<string> parent)
    {
        
        string Neu = TreeToExpression(neu, true);
        string Parent = TreeToExpression(parent, true);
        if (networkNodes.Contains(Neu))
        {
            int i = networkNodes.IndexOf(Neu);
            if (networkEdges[i].Contains(Parent)) return false;
            else
            {
                //networkEdges[i].Add(Parent);
                networkEdges[networkNodes.IndexOf(Parent)].Add(Neu);
                return true;
            }
        }
        else
        {
            networkNodes.Add(Neu);
            List<string> edges = new List<string>();
            edges.Add(Parent);
            networkEdges.Add(edges);
            //Debug.Log(networkNodes.IndexOf(Parent));
            networkEdges[networkNodes.IndexOf(Parent)].Add(Neu);
            return true;
        }
    }

    public void changeReduce()
    {
        reduce = !reduce;
    }
    public bool GetReduce()
    {
        return reduce;
    }
    public int GetFuncsLenght()
    {
        return funcs.Count;
    }

    //Generates resulting Expressiontrees for a subset of operations defined in funcs
    public List<(Node<string>, int)> Evaluation(Node<string> term, string path, List<int> operations)
    {
        List<(Node<string>, int)> rtn = new List<(Node<string>, int)>();
        string t = TreeToExpression(term, true);
        foreach (int o in operations)
        {
            Node<string> resultat = funcs[o](term, path, "s");

            //textBefore.SetText(ExpressionToLatex(TreeToExpression(resultat, true)));
            if (reduce) resultat = Reduce(ReduceDivisionByOne(ReduceFractLeft(ReduceFractRight(resultat))));
            string r = TreeToExpression(resultat, true);
            //textAfter.SetText(ExpressionToLatex(r));
            if (r != t) rtn.Add((resultat, o));
        }
        return rtn;
    }

    //Umkehrfunktion von LatexToExpression
    public string ExpressionToLatex(String text)
    {
        text = text.Replace("*", @"\cdot");
        while (text.Contains(")/("))//"@"\frac{"))
        {
            int i = text.IndexOf(")/(");//text.IndexOf(@"\frac{");
            int j = ComplementaryBracket(i, text, false);
            int k = ComplementaryBracket(i + 2, text, true);

            text = text.Substring(0, j) + @"\frac{" + text.Substring(j + 1, i - (j + 1)) + "}{" + text.Substring(i + 3, k - (i + 3)) + "}" + text.Substring(k + 1);
            //text = text.Substring(0, i) + "(" + text.Substring(i + 6, j - (i + 6)) + ")/(" + text.Substring(j + 2, k - (j + 2)) + ")" + text.Substring(k + 1);
        }
        while (text.Contains(")^0.5"))
        {
            int i = text.IndexOf(")^0.5");//@"\sqrt{");
            int j = ComplementaryBracket(i, text, false);
            text = text.Substring(0, j) + @"\sqrt{" + text.Substring(j + 1, i - (j + 1)) + "}" + text.Substring(i + 5);
        }
        while (text.Contains(")^01."))
        {

            char[] op = new char[] { '*', '/', '+', '-', '^', ')', '}', '{', '\\' };
            int i = text.IndexOf(")^01.");
            int j = ComplementaryBracket(i, text, false);
            char[] chars = text.ToCharArray();
            int u = i + 4;
            while (u < chars.Length)
            {
                if (op.Contains(chars[u]))
                {
                    //u--;
                    break;
                }
                u++;
            }
            if (u > chars.Length) u = chars.Length - 1;

            text = text.Substring(0, j) + @"\sqrt[" + text.Substring(i + 5, u - (i + 5)) + "]{" + text.Substring(j + 1, i - (j + 1)) + "}" + text.Substring(u);


        }
        while (text.Contains(")^("))
        {
            int i = text.IndexOf(")^(");
            int j = ComplementaryBracket(i, text, false);
            int k = ComplementaryBracket(i + 2, text, true);
            string basis = text.Substring(j + 1, i - (j + 1));
            if (basis.StartsWith("("))
            {
                int p = ComplementaryBracket(1, basis, true);
                if (p == basis.Length - 1)
                {
                    //Console.WriteLine(basis+" bevor");
                    basis = basis.Substring(1, basis.Length - 2);
                    //Console.WriteLine(basis+" nachher");
                }
            }
            if (basis.Contains(@"\frac") || basis.Contains(@"\cdot") || basis.Contains('+') || basis.Contains('-') || basis.Contains('^'))
                text = text.Substring(0, j) + "{{(" + basis/*text.Substring(j + 1, i - (j + 1)) */+ ")}^{" + text.Substring(i + 3, k - (i + 3)) + "}}" + text.Substring(k + 1);
            else
                text = text.Substring(0, j) + "{{" + basis /*text.Substring(j + 1, i - (j + 1))*/ + "}^{" + text.Substring(i + 3, k - (i + 3)) + "}}" + text.Substring(k + 1);
        }
        string[] spl = text.Split(" ");
        text = "";
        foreach (string i in spl) text += i;
        return text;
    }

    public int shortestPath(Node<string> term, int networkBoundary)
    {
        
        int shorti = int.MaxValue;
        List<string> nodes = new List<string>();
        List<Node<string>> sedon = new List<Node<string>>();
        List<List<string>> edges = new List<List<string>>();
        nodes.Add(TreeToExpression(term, true));
        sedon.Add(CopyNode(term));
        edges.Add(new List<string>());
        int endi = 0;
        bool notFound = true;
        Node<string> node = CopyNode(term);
        Node<string> res;
        int runI = 0;
        while (runI < networkBoundary)
        {
            
            //Debug.Log("TreeMarching: " + TreeMarching(node, "s", ""));
            List<string> paths = TreeMarching(node, "s", "").Trim().Split(" ").ToList();
            int count = 0;
            foreach (string path in paths)
            {
                for (int j = 0; j< funcs.Count; j++)
                {
                    List<int> sp = new List<int>() { j };
                    List<(Node<string>,int)> choice = Evaluation(node, path, sp);
                    if (choice.Count > 0)
                    {
                        count++;
                         res = choice[0].Item1;
                        string ser = TreeToExpression(res, true);
                        
                        edges[nodes.IndexOf(TreeToExpression(node, true))].Add(ser);
                        if (!nodes.Contains(ser))
                        {
                            nodes.Add(ser);
                            sedon.Add(CopyNode(res));
                            edges.Add(new List<string>());
                        }
                        
                    }
                }            
            }
            if (count == 0 && notFound)
            {
                endi = nodes.IndexOf(TreeToExpression(node, true));
                notFound = false;
                Debug.Log("Endi = " + endi);
            }
            runI++;
            //Debug.Log("edges " + edges.Count + " sedon " + sedon.Count + " runI " + runI);
            if (runI >= edges.Count) break;
            else
            {
                node = CopyNode(sedon[runI]);
            }
            

        }
        Debug.Log("Es gibt " + edges.Count + " edges");


        for (int i = runI; i < edges.Count; i++)
        {
            node = sedon[i];
            int index = nodes.IndexOf(TreeToExpression(node, true));
                List<string> paths = TreeMarching(node, "s", "").Trim().Split(" ").ToList();
                int count = 0;
                foreach (string path in paths)
                {
                    for (int j = 0; j < funcs.Count; j++)
                    {
                        List<int> sp = new List<int>() { j };
                        List<(Node<string>, int)> choice = Evaluation(node, path, sp);
                        if (choice.Count > 0)
                        {
                            count++;
                            res = choice[0].Item1;
                            string ser = TreeToExpression(res, true);

                            if (nodes.Contains(ser)) edges[index].Add(ser);


                        }
                    }
                }
                if (count == 0 && notFound)
                {
                    endi = index;
                    notFound = false;
                    Debug.Log("Endi = " + endi);
                }
        
        }

        int[,] graph = new int[nodes.Count, nodes.Count];
        int ci = 0;
        foreach (var e in edges)
        {
            foreach (var j in e) graph[ci, nodes.IndexOf(j)] = 1;
            ci++;
        }
        int[] mins = dijkstra(graph, 0, nodes.Count);
        Debug.Log("Endi == "+nodes[endi]);
        shorti = mins[endi];
        return shorti;
        
        
    }

    // Quelle https://www.geeksforgeeks.org/c-sharp/csharp-program-for-dijkstras-shortest-path-algorithm-greedy-algo-7/
    public int[] dijkstra(int[,] graph, int src, int V)
    {
        
        int[] dist = new int[V]; // The output array. dist[i]
                                 // will hold the shortest
                                 // distance from src to i

        // sptSet[i] will true if vertex
        // i is included in shortest path
        // tree or shortest distance from
        // src to i is finalized
        bool[] sptSet = new bool[V];

        // Initialize all distances as
        // INFINITE and stpSet[] as false
        for (int i = 0; i < V; i++)
        {
            dist[i] = int.MaxValue;
            sptSet[i] = false;
        }

        // Distance of source vertex
        // from itself is always 0
        dist[src] = 0;

        // Find shortest path for all vertices
        for (int count = 0; count < V - 1; count++)
        {
            // Pick the minimum distance vertex
            // from the set of vertices not yet
            // processed. u is always equal to
            // src in first iteration.
            int u = minDistance(dist, sptSet,V);

            // Mark the picked vertex as processed
            sptSet[u] = true;

            // Update dist value of the adjacent
            // vertices of the picked vertex.
            for (int v = 0; v < V; v++)

                // Update dist[v] only if is not in
                // sptSet, there is an edge from u
                // to v, and total weight of path
                // from src to v through u is smaller
                // than current value of dist[v]
                if (!sptSet[v] && graph[u, v] != 0 &&
                    dist[u] != int.MaxValue && dist[u] + graph[u, v] < dist[v])
                    dist[v] = dist[u] + graph[u, v];
        }

        // print the constructed distance array
        //printSolution(dist, V);
        return dist;
    }

    int minDistance(int[] dist,
                bool[] sptSet, int V)
    {
        // Initialize min value
        int min = int.MaxValue, min_index = -1;

        for (int v = 0; v < V; v++)
            if (sptSet[v] == false && dist[v] <= min)
            {
                min = dist[v];
                min_index = v;
            }

        return min_index;
    }

    //The variable bruch determines if multiplication with a fraction takes place on the denominator (bruch == true) or the factor precedes the fraction
    // this has to be a major aspect of discussion!!!!!!!!!!!!!!!!
    public string TreeToExpression(Node<string> tree, bool bruch)
    {
        
        string expression;


        switch (tree.key)
        {
            case "+":
                {
                    expression = TreeToExpression(tree.left, bruch) + "+" + TreeToExpression(tree.right, bruch);
                    break;
                }
            case "-":
                {
                    if (tree.right.key == "+" || tree.right.key == "-")
                    {
                        expression = TreeToExpression(tree.left, bruch) + "-(" + TreeToExpression(tree.right, bruch) + ")";
                    }
                    else
                    {
                        expression = TreeToExpression(tree.left, bruch) + "-" + TreeToExpression(tree.right, bruch);
                    }
                    break;
                }
            case "^":
                {
                    string exponent = "";
                    if (tree.right.key == "0.5") exponent = "0.5";
                    else if (tree.right.key.StartsWith("01.")) exponent = tree.right.key;
                    else exponent = "(" + TreeToExpression(tree.right, bruch) + ")";
                    if ((tree.left.key == "^" | tree.left.key == "/") && !exponent.StartsWith("0")) expression = "((" + TreeToExpression(tree.left, bruch) + "))^" + exponent;
                    else expression = "(" + TreeToExpression(tree.left, bruch) + ")^" + exponent;
                    break;
                }
            case "/":
                {
                    if (!bruch)
                    {
                        if (tree.left.key == "*") expression = TreeToExpression(tree.left.left, bruch) + "*(" + TreeToExpression(tree.left.right, bruch) + ")/(" + TreeToExpression(tree.right, bruch) + ")";
                        else expression = "(" + TreeToExpression(tree.left, bruch) + ")/(" + TreeToExpression(tree.right, bruch) + ")";
                    }
                    else
                    {
                        expression = "(" + TreeToExpression(tree.left, bruch) + ")/(" + TreeToExpression(tree.right, bruch) + ")";

                    }
                    break;
                }
            case "*":
                {
                    if (tree.left.key == "+" || tree.left.key == "-")
                    {
                        expression = "(" + TreeToExpression(tree.left, bruch) + ")*";
                    }
                    else expression = TreeToExpression(tree.left, bruch) + "*";
                    if (tree.right.key == "+" || tree.right.key == "-")
                    {
                        expression += "(" + TreeToExpression(tree.right, bruch) + ")";
                    }
                    else expression += TreeToExpression(tree.right, bruch);
                    //PrintTree(tree,0);
                    //Console.WriteLine(expression+" asa");
                    break;
                }
            default:
                {
                    string zahl = mnt2(tree.key);
                    expression = zahl;
                    break;
                }

        }
        return expression;

    }



    public Node<string> Reduce(Node<string> tree)
    {
        bool geht = false;
        Node<string> n;
        if (tree.key == "*")
        {
            if (tree.left.key == "1" || tree.right.key == "1") geht = true;
        }

        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = Reduce(tree.left);
            if (tree.right != null) n.right = Reduce(tree.right);

        }
        else
        {
            if (tree.left.key == "1")
            {
                n = Reduce(tree.right);
            }
            else
            {
                n = Reduce(tree.left);
            }
        }
        return n;
    }

    public Node<string> ReduceDivisionByOne(Node<string> tree)
    {
        bool geht = false;
        Node<string> n;
        if (tree.key == "/")
        {
            if (tree.right.key == "1") geht = true;
        }

        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = ReduceDivisionByOne(tree.left);
            if (tree.right != null) n.right = ReduceDivisionByOne(tree.right);

        }
        else
        {
            n = /*CopyNode*/ReduceDivisionByOne(tree.left);
        }
        return n;
    }


    public Node<string> ReduceFractRight(Node<string> tree)
    {
        bool geht = false;
        Node<string> n;
        if (tree.key == "/")
        {
            if (tree.right.key == "/") geht = true;
        }

        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = ReduceFractRight(tree.left);
            if (tree.right != null) n.right = ReduceFractRight(tree.right);

        }
        else
        {
            n = new Node<string>("/");
            n.left = new Node<string>("*");
            n.left.left = CopyNode(tree.left);
            Node<string> m;
            if (tree.right.right.key == "/") m = ReduceFractRight(tree.right);
            else m = CopyNode(tree.right);
            n.left.right = m.right;
            n.right = m.left;
        }
        return n;

    }

    public Node<string> ReduceFractLeft(Node<string> tree)
    {
        bool geht = false;
        Node<string> n;
        if (tree.key == "/")
        {
            if (tree.left.key == "/") geht = true;
        }

        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = ReduceFractLeft(tree.left);
            if (tree.right != null) n.right = ReduceFractLeft(tree.right);

        }
        else
        {
            n = new Node<string>("/");
            n.right = new Node<string>("*");
            //n.left=CopyNode(tree.left);
            Node<string> m;
            if (tree.left.left.key == "/") m = ReduceFractLeft(tree.left);
            else m = CopyNode(tree.left);
            n.right.left = m.right;
            n.right.right = CopyNode(tree.right);
            n.left = m.left;
        }
        return n;

    }




    public Node<string> PotenceToRoot(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.right.key == "/" && IsLeave(tree.right, ziel + "r", path + "r"))
                {
                    geht = true;
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenceToRoot(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenceToRoot(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            if (tree.right.left.key == "1" && tree.right.right.key == "2")
            {
                n.left = CopyNode(tree.left);
                n.right = new Node<string>("0.5");
            }
            else
            {
                n.right = new Node<string>("01." + tree.right.right.key);
                if (tree.right.left.key == "1")
                {
                    n.left = CopyNode(tree.left);
                }
                else
                {
                    n.left = new Node<string>("^");
                    n.left.left = CopyNode(tree.left);
                    n.left.right = new Node<string>(tree.right.left.key);
                }
            }
        }

        return n;

    }



    public Node<string> RootToPotence(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.right.key == "0.5" || tree.right.key.StartsWith("01."))
                {
                    geht = true;
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = RootToPotence(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = RootToPotence(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = CopyNode(tree.left);
            n.right = new Node<string>("/");
            n.right.left = new Node<string>("1");
            if (tree.right.key == "0.5")
            {
                n.right.right = new Node<string>("2");
            }
            else
            {
                n.right.right = new Node<string>(tree.right.key.Substring(3));
            }
            //if (tree.right.key == "+") n = new Node<string>("*");

        }

        return n;

    }

    public Node<string> LiftDownFree(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (!(op.Contains(tree.right.key) || tree.right.key == "0" || tree.right.key == "0.5" || tree.right.key.StartsWith("01.")))
                {
                    /*if (Calculate(tree.right)<0)*/
                    {
                        if (tree.left.key != "1") geht = true;
                    }

                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LiftDownFree(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LiftDownFree(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("/");
            n.left = new Node<string>("1");
            n.right = new Node<string>("^");
            n.right.left = CopyNode(tree.left);
            Node<string> m = new Node<string>("*");
            m.left = new Node<string>("00001");
            m.right = new Node<string>(tree.right.key);
            float f = Calculate(m);
            string res = f.ToString();
            if (f < 0) res = "(" + res + ")";
            n.right.right = new Node<string>(res);
        }

        return n;

    }


    public Node<string> LiftDown(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (!(op.Contains(tree.right.key) || tree.right.key == "0" || tree.right.key == "0.5" || tree.right.key.StartsWith("01.")))
                {
                    if (Calculate(tree.right) < 0)
                    {
                        geht = true;
                    }

                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LiftDown(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LiftDown(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("/");
            n.left = new Node<string>("1");
            n.right = new Node<string>("^");
            n.right.left = CopyNode(tree.left);
            Node<string> m = new Node<string>("*");
            m.left = new Node<string>("00001");
            m.right = new Node<string>(tree.right.key);
            float f = Calculate(m);
            string res = f.ToString();
            if (f < 0) res = "(" + res + ")";
            n.right.right = new Node<string>(res);
        }

        return n;

    }

    public Node<string> KommutativGesetz(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" || tree.key == "+")
            {
                geht = true;

            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = KommutativGesetz(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = KommutativGesetz(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>(tree.key);
            n.left = CopyNode(tree.right);
            n.right = CopyNode(tree.left);
        }

        return n;

    }

    public Node<string> LiftDownProductKommutativ(Node<string> tree, string ziel, string path)// OPERATIOR -1
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*")
            {
                if (tree.left.key == "^")
                {
                    if (!(op.Contains(tree.left.right.key) || tree.left.right.key == "0" || tree.left.right.key == "0.5" || tree.left.right.key.StartsWith("01.")))
                    {
                        geht = true;
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LiftDownProductKommutativ(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LiftDownProductKommutativ(tree.right, ziel, path + "r");
        }
        else
        {
            Node<string> treeC = KommutativGesetz(tree, ziel, path);
            n = LiftDownProduct(treeC, ziel, path);
        }

        return n;

    }

    public Node<string> LiftDownProductLeft(Node<string> tree, string ziel, string path) // OPERATOR + 1
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*")
            {
                if (tree.left.key == "^")
                {
                    if (!(op.Contains(tree.left.right.key) || tree.left.right.key == "0" || tree.left.right.key == "0.5" || tree.left.right.key.StartsWith("01.")))
                    {
                        if (tree.right.key != "1")
                            geht = true;
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LiftDownProductLeft(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LiftDownProductLeft(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("/");
            n.left = CopyNode(tree.right);
            n.right = new Node<string>("^");
            n.right.left = CopyNode(tree.left.left);
            Node<string> m = new Node<string>("*");
            m.left = new Node<string>("00001");
            m.right = new Node<string>(tree.left.right.key);
            float f = Calculate(m);
            string res = f.ToString();
            if (f < 0) res = "(" + res + ")";
            n.right.right = new Node<string>(res);
        }

        return n;

    }


    public Node<string> LiftDownProduct(Node<string> tree, string ziel, string path) // OPERATOR + 1
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*")
            {
                if (tree.right.key == "^")
                {
                    if (!(op.Contains(tree.right.right.key) || tree.right.right.key == "0" || tree.right.right.key == "0.5" || tree.right.right.key.StartsWith("01.")))
                    {
                        geht = true;
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LiftDownProduct(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LiftDownProduct(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("/");
            n.left = CopyNode(tree.left);
            n.right = new Node<string>("^");
            n.right.left = CopyNode(tree.right.left);
            Node<string> m = new Node<string>("*");
            m.left = new Node<string>("00001");
            m.right = new Node<string>(tree.right.right.key);
            float f = Calculate(m);
            string res = f.ToString();
            if (f < 0) res = "(" + res + ")";
            n.right.right = new Node<string>(res);
        }

        return n;

    }

    public string WhatsOn(Node<string> tree, string path)
    {
        if (path.Length > 0)
        {
            if (path.Substring(0, 1) == "l")
            {
                return WhatsOn(tree.left, path.Substring(1));
            }
            else
            {
                return WhatsOn(tree.right, path.Substring(1));
            }
        }
        else return tree.key;
    }

    public Node<string> CopyOn(Node<string> tree, string path)
    {//returns the subtree from the path onword 
        if (path.Length > 0)
        {
            if (path.Substring(0, 1) == "l")
            {
                return CopyOn(tree.left, path.Substring(1));
            }
            else
            {
                return CopyOn(tree.right, path.Substring(1));
            }
        }
        else return CopyNode(tree);
    }


    public bool DotPath(Node<string> tree, string path)
    {

        if (path.Length == 0) return true;
        else
        {
            if (tree.key == "*")
            {
                if (path.First() == 'l')
                {
                    return DotPath(tree.left, path.Substring(1));
                }
                else
                {
                    return DotPath(tree.right, path.Substring(1));
                }

            }
            else return false;
        }
    }

    public Node<string> AddSubtree(Node<string> tree, string ziel, string path, char lr)
    {
        bool geht = false;
        Node<string> n;
        if (path == ziel)
        {
            geht = true;
        }

        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = AddSubtree(tree.left, ziel, path + "l", lr);
            if (tree.right != null) n.right = AddSubtree(tree.right, ziel, path + "r", lr);

        }
        else
        {
            if (lr == 'l')
            {
                n = CopyNode(tree.right);
            }
            else
            {
                n = CopyNode(tree.left);
            }
        }
        return n;
    }

    public Node<string> LiftDownNew(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (/*path == ziel*/true)
        {
            if (tree.key == "/")
            {
                string tm = TreeMarchingAddKey(tree.left, path + "l");
                string[] mt = tm.Split();
                bool contains = false;
                foreach (string t in mt)
                {
                    if (t == "^" + ziel) contains = true;
                }
                if (contains/*tm.Contains("^"+ziel)*/)
                {
                    if (DotPath(tree.left, ziel.Substring(path.Length).Substring(1)))
                    {

                        string exponent = WhatsOn(tree, ziel.Substring(path.Length) + "r");
                        string basis = WhatsOn(tree, ziel.Substring(path.Length) + "l");
                        if (!(op.Contains(exponent) || exponent == "0" || exponent == "0.5" || exponent.StartsWith("01.")))
                        {
                            if (basis != "1") geht = true;
                        }
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LiftDownNew(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LiftDownNew(tree.right, ziel, path + "r");
        }
        else
        {
            Node<String> nNenner;
            if (tree.right.key == "1")
            {
                nNenner = CopyOn(tree, ziel.Substring(path.Length));//new Node<string>("^");
                                                                    //n.left = CopyNode(tree.right.left);
                Node<string> m = new Node<string>("*");
                m.left = new Node<string>("00001");
                m.right = new Node<string>(nNenner.right.key);
                float f = Calculate(m);
                string res = f.ToString();
                if (f < 0) res = "(" + res + ")";
                nNenner.right = new Node<string>(res);
            }
            else
            {
                nNenner = new Node<string>("*");
                nNenner.left = CopyNode(tree.right);
                nNenner.right = CopyOn(tree, ziel.Substring(path.Length));// new Node<string>("^");

                Node<string> m = new Node<string>("*");
                m.left = new Node<string>("00001");
                m.right = new Node<string>(nNenner.right.right.key);
                float f = Calculate(m);
                string res = f.ToString();
                if (f < 0) res = "(" + res + ")";
                nNenner.right.right = new Node<string>(res);

            }
            Node<string> nZaehler;
            if (ziel.Substring(path.Length).Length == 1)
            {
                //n = nNenner;
                nZaehler = new Node<string>("1");
                n = new Node<string>("/");
                n.left = nZaehler;
                n.right = nNenner;
            }
            else
            {

                string pathBefore = ziel.Substring(path.Length);
                pathBefore = pathBefore.Substring(1);
                string pathBeforeBefore = pathBefore.Remove(pathBefore.Length - 1);
                /*Node<string> nZaehler ;*/


                if (pathBefore.Last() == 'l')
                {
                    nZaehler = AddSubtree(tree.left, pathBeforeBefore, "", 'l');
                }
                else
                {
                    nZaehler = AddSubtree(tree.left, pathBeforeBefore, "", 'r');
                }

                n = new Node<string>("/");
                n.left = CopyNode(nZaehler);
                n.right = CopyNode(nNenner);
            }


        }

        return n;

    }


    public Node<string> LiftUpNew(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (/*path == ziel*/true)
        {
            if (tree.key == "/")
            {
                string tm = TreeMarchingAddKey(tree.right, path + "r");
                string[] mt = tm.Split();
                bool contains = false;
                foreach (string t in mt)
                {
                    if (t == "^" + ziel) contains = true;
                }
                if (contains/*tm.Contains("^"+ziel)*/)
                {
                    if (DotPath(tree.right, ziel.Substring(path.Length).Substring(1)))
                    {

                        string exponent = WhatsOn(tree, ziel.Substring(path.Length) + "r");
                        string basis = WhatsOn(tree, ziel.Substring(path.Length) + "l");
                        if (!(op.Contains(exponent) || exponent == "0" || exponent == "0.5" || exponent.StartsWith("01.")))
                        {
                            if (basis != "1") geht = true;
                        }
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LiftUpNew(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LiftUpNew(tree.right, ziel, path + "r");
        }
        else
        {
            Node<String> nZaehler;
            if (tree.left.key == "1")
            {
                nZaehler = CopyOn(tree, ziel.Substring(path.Length));//new Node<string>("^");
                                                                     //n.left = CopyNode(tree.right.left);
                Node<string> m = new Node<string>("*");
                m.left = new Node<string>("00001");
                m.right = new Node<string>(nZaehler.right.key);
                float f = Calculate(m);
                string res = f.ToString();
                if (f < 0) res = "(" + res + ")";
                nZaehler.right = new Node<string>(res);
            }
            else
            {
                nZaehler = new Node<string>("*");
                nZaehler.left = CopyNode(tree.left);
                nZaehler.right = CopyOn(tree, ziel.Substring(path.Length));// new Node<string>("^");

                Node<string> m = new Node<string>("*");
                m.left = new Node<string>("00001");
                m.right = new Node<string>(nZaehler.right.right.key);
                float f = Calculate(m);
                string res = f.ToString();
                if (f < 0) res = "(" + res + ")";
                nZaehler.right.right = new Node<string>(res);

            }

            if (ziel.Substring(path.Length).Length == 1)
            {
                n = nZaehler;
            }
            else
            {

                string pathBefore = ziel.Substring(path.Length);
                pathBefore = pathBefore.Substring(1);
                string pathBeforeBefore = pathBefore.Remove(pathBefore.Length - 1);
                Node<string> nNenner;


                if (pathBefore.Last() == 'l')
                {
                    nNenner = AddSubtree(tree.right, pathBeforeBefore, "", 'l');
                }
                else
                {
                    nNenner = AddSubtree(tree.right, pathBeforeBefore, "", 'r');
                }

                n = new Node<string>("/");
                n.left = CopyNode(nZaehler);
                n.right = CopyNode(nNenner);
            }


        }

        return n;

    }

    public Node<string> LiftUp(Node<string> tree, string ziel, string path) // OPERATOR + 1
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "/")
            {
                if (tree.right.key == "^")
                {
                    if (!(op.Contains(tree.right.right.key) || tree.right.right.key == "0" || tree.right.right.key == "0.5" || tree.right.right.key.StartsWith("01.")))
                    {
                        geht = true;
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LiftUp(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LiftUp(tree.right, ziel, path + "r");
        }
        else
        {
            if (tree.left.key == "1")
            {
                n = new Node<string>("^");
                n.left = CopyNode(tree.right.left);
                Node<string> m = new Node<string>("*");
                m.left = new Node<string>("00001");
                m.right = new Node<string>(tree.right.right.key);
                float f = Calculate(m);
                string res = f.ToString();
                if (f < 0) res = "(" + res + ")";
                n.right = new Node<string>(res);
            }
            else
            {
                n = new Node<string>("*");
                n.left = CopyNode(tree.left);
                n.right = new Node<string>("^");
                n.right.left = CopyNode(tree.right.left);
                Node<string> m = new Node<string>("*");
                m.left = new Node<string>("00001");
                m.right = new Node<string>(tree.right.right.key);
                float f = Calculate(m);
                string res = f.ToString();
                if (f < 0) res = "(" + res + ")";
                n.right.right = new Node<string>(res);

            }

        }

        return n;

    }

    public Node<string> PotenzGesetzIrueckwaerts(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.right.key == "+" || tree.right.key == "-")
                {
                    geht = true;
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIrueckwaerts(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIrueckwaerts(tree.right, ziel, path + "r");
        }
        else
        {
            if (tree.right.key == "+") n = new Node<string>("*");
            else n = new Node<string>("/");
            n.left = new Node<string>("^");
            n.left.left = CopyNode(tree.left);
            n.left.right = CopyNode(tree.right.left);
            n.right = new Node<string>("^");
            n.right.left = CopyNode(tree.left);
            n.right.right = CopyNode(tree.right.right);




        }

        return n;
    }

    private bool IsLeave(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        if (path == ziel)
        {
            if (!op.Contains(tree.left.key) && !op.Contains(tree.right.key)) return true;
            else return false;
        }
        else
        {
            return IsLeave(tree.left, ziel, path + "l") || IsLeave(tree.right, ziel, path + "r");
        }
    }

    public Node<string> Leave(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (!op.Contains(tree.left.key) && !op.Contains(tree.right.key) && !(tree.right.key.StartsWith("01.") /*|| tree.right.key == "0.5"*/)) geht = true;
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = Leave(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = Leave(tree.right, ziel, path + "r");
        }
        else
        {
            float f = Calculate(tree);
            string res = f.ToString();
            if (f < 0)
                res = "(" + res + ")";
            n = new Node<string>(res);
        }
        return n;
    }

    public Node<string> LeaveStrictInt(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        
        if (path == ziel)
        {
            if (!op.Contains(tree.left.key) && !op.Contains(tree.right.key) && !(tree.right.key.StartsWith("01.") /*|| tree.right.key == "0.5"*/))
            {
                
                float f = Calculate(tree);
                int ff = (int)f;
                float fff = ff;
                if (f == ff)
                {
                    geht = true;
                }


            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LeaveStrictInt(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LeaveStrictInt(tree.right, ziel, path + "r");
        }
        else
        {
            
            float f = Calculate(tree);
            string res = f.ToString();
            if (f < 0)
                res = "(" + res + ")";
            n = new Node<string>(res);
        }
        return n;
    }

    public Node<string> LeaveInnerProductInt(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*")
            {
                if (tree.left.key == "*")
                {
                    if (!op.Contains(tree.left.right.key) && !op.Contains(tree.right.key))
                    {
                        float a = Calculate(tree.left.right);
                        float b = Calculate(tree.right);
                        float f = a * b;
                        int ff = (int)f;
                        float fff = ff;
                        if (f == fff)
                        {
                            geht = true;
                        }


                    }

                }

            }

        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LeaveInnerProductInt(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LeaveInnerProductInt(tree.right, ziel, path + "r");
        }
        else
        {
            float a = Calculate(tree.left.right);
            float b = Calculate(tree.right);
            float f = a * b;
            string res = f.ToString();
            if (f < 0)
                res = "(" + res + ")";
            n = new Node<string>("*");
            n.left = CopyNode(tree.left.left);
            n.right = new Node<string>(res);
        }
        return n;
    }

    public Node<string> LeaveKuerzen(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "/")
            {

                if (!op.Contains(tree.left.key) && !op.Contains(tree.right.key))
                {
                    float a = Calculate(tree.left);
                    float b = Calculate(tree.right);
                    int aInt = (int)a;
                    int bInt = (int)b;
                    float aF = aInt;
                    float bF = bInt;
                    if (a == aF && b == bF) geht = true;

                }



            }

        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LeaveKuerzen(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LeaveKuerzen(tree.right, ziel, path + "r");
        }
        else
        {
            float a = Calculate(tree.left);
            float b = Calculate(tree.right);
            int aInt = (int)a;
            int bInt = (int)b;
            if (aInt < 0 && bInt < 0)
            {
                aInt *= -1;
                bInt *= -1;
            }
            float gcd = GCD(aInt, bInt);
            a = aInt / gcd;
            b = bInt / gcd;

            string aS = a.ToString();
            string bS = b.ToString();
            if (a < 0)
                aS = "(" + aS + ")";
            if (b < 0)
                bS = "(" + bS + ")";
            n = new Node<string>("/");
            n.left = new Node<string>(aS);
            n.right = new Node<string>(bS);
            n = LeaveStrictInt(n, "", "");
        }
        return n;
    }

    public bool SameFractionLine(Node<string> tree, string path, bool start)
    {
        char[] weg = path.ToCharArray();
        string[] op = new string[] { /*"*", "/",*/ "+", "-", "^" };
        bool line = start;
        Node<string> n = CopyNode(tree);
        bool abrechen = false;
        foreach (char step in weg)
        {
            if (step == 'l')
            {
                n = CopyNode(n.left);
                if (op.Contains(n.key)) abrechen = true;

            }
            else
            {
                String parent = n.key;
                n = CopyNode(n.right);
                if (op.Contains(n.key)) abrechen = true;
                if (parent == "/") line = !line;

            }
            if (abrechen)
            {
                line = false;
                break;
            }

        }
        return line;

    }

    public Node<string> AddNode(Node<string> tree, string ziel, string path, string key)
    {
        bool geht = false;
        Node<string> n;
        if (path == ziel)
        {
            geht = true;
        }

        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = AddNode(tree.left, ziel, path + "l", key);
            if (tree.right != null) n.right = AddNode(tree.right, ziel, path + "r", key);

        }
        else
        {
            n = new Node<string>(key);


        }
        return n;
    }


    public Node<string> KuerzenSeparate(Node<string> tree, string ziel, string path)
    {
        if (ziel.StartsWith("s")) ziel = ziel.Substring(1);
        if (path.StartsWith("s")) path = path.Substring(1);
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        int nl = 0, nr = 0;
        string subpathR = "", subpathL = "";
        if (/*path == ziel*/true)
        {
            if (tree.key == "/" || tree.key == "*")
            {
                string tm = TreeMarchingAddKey(tree, path);
                if (tree.key == "/") tm = TreeMarchingAddKey(tree.left, path + "l");
                string[] mt = tm.Split();
                bool contains = false;
                foreach (string t in mt)
                {
                    if (t == "*" + ziel) contains = true;
                }
                if (contains)
                {
                    if (SameFractionLine(tree, ziel.Substring(path.Length), true))
                    {
                        string r = WhatsOn(tree, ziel.Substring(path.Length) + "r");
                        string l = WhatsOn(tree, ziel.Substring(path.Length) + "l");
                        bool rOK = false, lOK = false;
                        if (!op.Contains(r))
                        {
                            nr = int.Parse(r);
                            rOK = true;
                            subpathR = "r";
                        }
                        else if (r == "*")
                        {
                            subpathR = "rl";
                            while (WhatsOn(tree, ziel.Substring(path.Length) + subpathR) == "*")
                            {
                                subpathR = subpathR + "l";
                            }
                            string rl = WhatsOn(tree, ziel.Substring(path.Length) + subpathR);
                            if (!op.Contains(rl))
                            {
                                nr = int.Parse(rl);
                                rOK = true;
                            }
                        }
                        if (!op.Contains(l))
                        {
                            nl = int.Parse(l);
                            lOK = true;
                            subpathL = "l";
                        }
                        else if (l == "*")
                        {
                            subpathL = "lr";
                            while (WhatsOn(tree, ziel.Substring(path.Length) + subpathL) == "*")
                            {
                                subpathL = subpathL + "r";
                            }
                            string rl = WhatsOn(tree, ziel.Substring(path.Length) + subpathL);
                            if (!op.Contains(rl))
                            {
                                nl = int.Parse(rl);
                                lOK = true;
                            }
                        }
                        else if (l == "/")
                        {
                            string ll = WhatsOn(tree, ziel.Substring(path.Length) + "ll");
                            if (!op.Contains(ll))
                            {
                                nl = int.Parse(ll);
                                lOK = true;
                                subpathL = "ll";
                            }
                            else if (ll == "*")
                            {
                                subpathL = "llr";
                                while (WhatsOn(tree, ziel.Substring(path.Length) + subpathL) == "*")
                                {
                                    subpathL = subpathL + "r";
                                }
                                string rl = WhatsOn(tree, ziel.Substring(path.Length) + subpathL);
                                if (!op.Contains(rl))
                                {
                                    nl = int.Parse(rl);
                                    lOK = true;
                                }

                            }
                        }

                        geht = rOK && lOK;

                    }

                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = KuerzenSeparate(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = KuerzenSeparate(tree.right, ziel, path + "r");
        }
        else
        {
            subpathL = ziel + subpathL;
            subpathR = ziel + subpathR;

            n = CopyNode(tree);
            string tm = TreeMarching(tree, "s", "");
            tm = tm.Trim();
            string[] mt = tm.Split();
            List<(int, string)> list = new List<(int, String)>();
            foreach (string p in mt)
            {

                string cc = ""; if (p.Length > 1) cc = p.Substring(1);

                if (SameFractionLine(tree, cc/*p.Substring(1)*/, false) || (WhatsOn(tree, cc) == "/" && !op.Contains(WhatsOn(tree, cc + "r"))))
                {
                    if (!mt.Contains(p + "r"))
                    {
                        string i = WhatsOn(tree, p.Substring(1) + "r");
                        int ii = int.Parse(i);
                        int gcd = GCD(nr, ii);
                        bool added = false;
                        if (gcd != 1)
                        {
                            nr /= gcd;
                            ii /= gcd;
                            added = true;
                        }
                        gcd = GCD(nl, ii);
                        if (gcd != 1)
                        {
                            nl /= gcd;
                            ii /= gcd;
                            added = true;
                        }
                        if (added) list.Add((ii, p.Substring(1) + "r"));
                    }
                    if (!mt.Contains(p + "l") && !(WhatsOn(tree, cc) == "/" && !op.Contains(WhatsOn(tree, cc + "r"))))
                    {
                        string i = WhatsOn(tree, p.Substring(1) + "l");
                        int ii = int.Parse(i);
                        int gcd = GCD(nr, ii);
                        bool added = false;
                        if (gcd != 1)
                        {
                            nr /= gcd;
                            ii /= gcd;
                            added = true;
                        }
                        gcd = GCD(nl, ii);
                        if (gcd != 1)
                        {
                            nl /= gcd;
                            ii /= gcd;
                            added = true;
                        }
                        if (added) list.Add((ii, p.Substring(1) + "l"));
                    }
                }

            }

            //Console.WriteLine(nl+" "+subpathL+" "+nr+" "+subpathR);

            n = AddNode(n, subpathR, "", nr.ToString());
            n = AddNode(n, subpathL, "", nl.ToString());

            if (list.Count > 0)
            {
                foreach ((int, string) l in list)
                {
                    n = AddNode(n, l.Item2, "", l.Item1.ToString());
                    // Console.WriteLine(l.Item1+" d "+l.Item2);
                }
            }
            // n = CopyNode(tree);

        }

        return n;

    }




    public Node<string> Potentiation(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.right.key == "/" && !op.Contains(tree.left.key))
                {
                    if (!op.Contains(tree.right.right.key))
                    {
                        if (!tree.right.right.key.Contains(".") && !op.Contains(tree.right.left.key))
                        {
                            string l = tree.left.key;
                            string r = tree.right.right.key;
                            if (l.StartsWith("(-")) l = l.Substring(1, l.Length - 2);
                            if (r.StartsWith("(-")) r = r.Substring(1, r.Length - 2);
                            //Console.WriteLine(l + " " + r);
                            float f = float.Parse(l/*tree.left.key*/);
                            float g = float.Parse(r/*tree.right.right.key*/);
                            f = MathF.Pow(f, 1.0f / g);
                            int ff = (int)f;
                            float fff = f;
                            if (ff == f) geht = true;
                        }
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = Potentiation(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = Potentiation(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = new Node<string>("^");
            float f = float.Parse(tree.left.key);
            float g = float.Parse(tree.right.right.key);
            f = MathF.Pow(f, 1.0f / g);

            n.left.left = new Node<string>(f.ToString());
            n.left.right = new Node<string>(g.ToString());

            n.right = CopyNode(tree.right);


        }
        return n;
    }

    public Node<string> LeaveSqrt(Node<string> tree, string ziel, string path)//sqrt[a]{{{b}^{a}}} -> b
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.right.key.StartsWith("01.") || tree.right.key == "0.5")
                {
                    if (tree.left.key == "^")
                    {
                        string wurzel = "2";
                        if (tree.right.key.StartsWith("01.")) wurzel = tree.right.key.Substring(3);

                        if (wurzel == tree.left.right.key)
                        {
                            geht = true;
                        }
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = LeaveSqrt(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = LeaveSqrt(tree.right, ziel, path + "r");
        }
        else
        {
            string wurzel = "2";
            if (tree.right.key.StartsWith("01."))
            {
                wurzel = tree.right.key.Substring(3);
            }
            n = CopyNode(tree.left.left);
        }
        return n;
    }

    public Node<string> PotentiationSqrt(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if ((tree.right.key.StartsWith("01.") || tree.right.key == "0.5") && !op.Contains(tree.left.key))
                {
                    string wurzel = "2";
                    if (tree.right.key.StartsWith("01."))
                    {
                        wurzel = tree.right.key.Substring(3);
                    }

                    string l = tree.left.key;
                    if (l.StartsWith("(-")) l = l.Substring(1, l.Length - 2);
                    if (wurzel.StartsWith("(-")) wurzel = wurzel.Substring(1, wurzel.Length - 2);

                    float f = float.Parse(l/*tree.left.key*/);
                    float g = float.Parse(wurzel);//float.Parse(tree.right.right.key);
                    f = MathF.Pow(f, 1.0f / g);
                    int ff = (int)f;
                    float fff = f;
                    if (ff == f) geht = true;

                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotentiationSqrt(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotentiationSqrt(tree.right, ziel, path + "r");
        }
        else
        {
            string wurzel = "2";
            if (tree.right.key.StartsWith("01."))
            {
                wurzel = tree.right.key.Substring(3);
            }

            n = new Node<string>("^");
            n.left = new Node<string>("^");
            float f = float.Parse(tree.left.key);
            float g = float.Parse(wurzel);//float.Parse(tree.right.right.key);
            f = MathF.Pow(f, 1.0f / g);

            n.left.left = new Node<string>(f.ToString());
            n.left.right = new Node<string>(g.ToString());

            n.right = CopyNode(tree.right);


        }
        return n;
    }


    // the following function assumes that ExpressionToTree has bruch set to true!!!!
    public Node<string> FractionMult(Node<string> tree, string ziel, string path)
    {

        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" || tree.key == "/")
            {
                if (tree.left.key == "/" || tree.right.key == "/")
                {
                    geht = true;
                    /*
					if (tree.left.key == "/" && tree.right.key == "/")	
					{
						if (!op.Contains(tree.left.left.key) && !op.Contains(tree.left.right.key) && !op.Contains(tree.right.left.key) && !op.Contains(tree.right.right.key))
						geht = true;
					}
					else if (tree.left.key == "/" && !op.Contains(tree.right.key))
					{
						if (!op.Contains(tree.left.left.key) && !op.Contains(tree.left.right.key))
						geht = true;
					} 
					else if (!op.Contains(tree.left.key) && tree.right.key == "/") 
					{
						if (!op.Contains(tree.right.left.key) && !op.Contains(tree.right.right.key))
						geht = true;
					}
					*/
                }

            }



        }
        if (!geht)
        {
            if (tree.key == "/")
            {
                if (tree.left.key == "*")// andernfalls wäre geht true
                {
                    if (tree.left.left.key == "/")
                    {
                        n = new Node<string>("/");
                        n.left = new Node<string>("*");
                        n.right = new Node<string>("*");
                        n.left.left = CopyNode(tree.left.left.left);
                        n.left.right = CopyNode(tree.left.right);

                        n.right.left = CopyNode(tree.left.left.right);
                        n.right.right = CopyNode(tree.right);

                    }
                    else
                    {
                        n = new Node<string>(tree.key);
                        if (tree.left != null) n.left = FractionMult(tree.left, ziel, path + "l");
                        if (tree.right != null) n.right = FractionMult(tree.right, ziel, path + "r");
                    }
                }
                else
                {
                    n = new Node<string>(tree.key);
                    if (tree.left != null) n.left = FractionMult(tree.left, ziel, path + "l");
                    if (tree.right != null) n.right = FractionMult(tree.right, ziel, path + "r");
                }


            }
            else
            {
                n = new Node<string>(tree.key);
                if (tree.left != null) n.left = FractionMult(tree.left, ziel, path + "l");
                if (tree.right != null) n.right = FractionMult(tree.right, ziel, path + "r");

            }

        }
        else
        {
            n = new Node<string>("/");
            if (tree.left.key == "/" && tree.right.key == "/")
            {
                n.left = new Node<string>("*");
                n.right = new Node<string>("*");
                if (tree.key == "*")
                {

                    n.left.left = CopyNode(tree.left.left);
                    n.left.right = CopyNode(tree.right.left);

                    n.right.left = CopyNode(tree.left.right);
                    n.right.right = CopyNode(tree.right.right);
                }
                else
                {
                    n.left.left = CopyNode(tree.left.left);
                    n.left.right = CopyNode(tree.right.right);
                    n.right.left = CopyNode(tree.left.right);
                    n.right.right = CopyNode(tree.right.left);
                }

            }
            else if (tree.left.key == "/" /*&& !op.Contains(tree.right.key)*/)
            {
                if (tree.key == "*")
                {
                    n.left = new Node<string>("*");
                    n.left.left = CopyNode(tree.left.left);
                    n.left.right = CopyNode(tree.right);
                    n.right = CopyNode(tree.left.right);
                }
                else
                {
                    n.left = CopyNode(tree.left.left);
                    n.right = new Node<string>("*");
                    n.right.left = CopyNode(tree.left.right);
                    n.right.right = CopyNode(tree.right);
                }

            }
            else if (/*!op.Contains(tree.left.key) && */tree.right.key == "/")
            {
                if (tree.key == "*")
                {

                    n.left = new Node<string>("*");
                    n.left.left = CopyNode(tree.left);
                    n.left.right = CopyNode(tree.right.left);
                    n.right = CopyNode(tree.right.right);

                }
                else
                {
                    if (tree.right.left.key == "1")
                    {
                        n = new Node<string>("*");
                        n.left = CopyNode(tree.left);
                        n.right = CopyNode(tree.right.right);
                    }
                    else
                    {
                        n.left = new Node<string>("*");
                        n.left.left = CopyNode(tree.left);
                        n.left.right = CopyNode(tree.right.right);
                        n.right = CopyNode(tree.right.left);
                    }

                }

            }

        }
        return n;

    }

    public Node<string> FractionAddition(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "+" || tree.key == "-")
            {
                if (tree.left.key == "/" || tree.right.key == "/")
                {
                    if (tree.left.key == "/" && tree.right.key == "/")
                    {
                        if (!op.Contains(tree.left.left.key) && !op.Contains(tree.left.right.key) && !op.Contains(tree.right.left.key) && !op.Contains(tree.right.right.key))
                            geht = true;
                    }
                    else if (tree.left.key == "/" && !op.Contains(tree.right.key))
                    {
                        if (!op.Contains(tree.left.left.key) && !op.Contains(tree.left.right.key))
                            geht = true;
                    }
                    else if (!op.Contains(tree.left.key) && tree.right.key == "/")
                    {
                        if (!op.Contains(tree.right.left.key) && !op.Contains(tree.right.right.key))
                            geht = true;
                    }
                }

            }



        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = FractionAddition(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = FractionAddition(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("/");
            if (tree.left.key == "/" && tree.right.key == "/")
            {
                int lDenominator = (int)mnt(tree.left.right.key);
                int rDenominator = (int)mnt(tree.right.right.key);
                int gcd = GCD(lDenominator, rDenominator);
                int lInumerator = (int)mnt(tree.left.left.key);
                int rInumerator = (int)mnt(tree.right.left.key);
                lInumerator *= rDenominator / gcd;
                rInumerator *= lDenominator / gcd;
                int summe = lInumerator + rInumerator;
                if (tree.key == "-") summe = lInumerator - rInumerator;
                rDenominator *= lDenominator / gcd;
                string nl = summe.ToString();
                if (summe < 0) nl = "(" + summe + ")";
                n.left = new Node<string>(nl);
                string nr = rDenominator.ToString();
                if (rDenominator < 0) nr = "(" + nr + ")";
                n.right = new Node<string>(nr);
            }
            else if (tree.left.key == "/" && !op.Contains(tree.right.key))
            {
                int lDenominator = (int)mnt(tree.left.right.key);
                int lInumerator = (int)mnt(tree.left.left.key);
                int r = (int)mnt(tree.right.key);
                int summe = lInumerator + r * lDenominator;
                if (tree.key == "-") summe = lInumerator - r * lDenominator;
                string nl = summe.ToString();
                if (summe < 0) nl = "(" + nl + ")";
                n.left = new Node<string>(nl);
                string nr = lDenominator.ToString();
                if (lDenominator < 0) nr = "(" + nr + ")";
                n.right = new Node<string>(nr);
            }
            else if (!op.Contains(tree.left.key) && tree.right.key == "/")
            {
                int rDenominator = (int)mnt(tree.right.right.key);
                int rInumerator = (int)mnt(tree.right.left.key);
                int l = (int)mnt(tree.left.key);
                int summe = rInumerator + l * rDenominator;
                if (tree.key == "-") summe = l * rDenominator - rInumerator;
                string nl = summe.ToString();
                if (summe < 0) nl = "(" + nl + ")";
                n.left = new Node<string>(nl);
                string nr = rDenominator.ToString();
                if (rDenominator < 0) nr = "(" + nr + ")";
                n.right = new Node<string>(nr);
            }

        }
        return n;

    }

    public int GCD(int a, int b)
    {
        a = (int)MathF.Abs(a);
        b = (int)MathF.Abs(b);

        while (a != b)
        {
            if (a > b) a = a - b;
            else b = b - a;
        }
        return a;
    }



    public Node<string> PotenzGesetzI(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" || tree.key == "/")
            {
                if (tree.left.key == "^" && tree.right.key == "^")
                {
                    if (!tree.left.right.key.StartsWith("01.") && !tree.right.right.key.StartsWith("01."))
                    {
                        if (Math.Abs(Calculate(tree.left.left) - Calculate(tree.right.left)) < 0.0000001f)
                        {//Ueberlegen ob eine striktere Version des PGIII sinnvoll (nur wenn exponenten Baumidentisch und nicht nur rechenidentisch)
                            geht = true;
                        }
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzI(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzI(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = CopyNode(tree.left.left);
            //if (Depth(tree.left.left)>Depth(tree.right.left)) {n.left = CopyNode(tree.right.left);}
            if (tree.key == "*") n.right = new Node<string>("+");
            else n.right = new Node<string>("-");
            n.right.left = CopyNode(tree.left.right);
            n.right.right = CopyNode(tree.right.right);
        }
        return n;
    }

    public Node<string> PotenzGesetzIleftExpOne(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" || tree.key == "/")
            {
                if (/*tree.left.key == "^" &&*/ tree.right.key == "^")
                {
                    if (/*!tree.left.right.key.StartsWith("01.") && */!tree.right.right.key.StartsWith("01."))
                    {
                        if (Math.Abs(Calculate(tree.left) - Calculate(tree.right.left)) < 0.0000001f)
                        {//Ueberlegen ob eine striktere Version des PGIII sinnvoll (nur wenn exponenten Baumidentisch und nicht nur rechenidentisch)
                            if (Math.Abs(Calculate(tree.left)) != 1) geht = true;
                        }
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIleftExpOne(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIleftExpOne(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = CopyNode(tree.left);
            if (tree.key == "*") n.right = new Node<string>("+");
            else n.right = new Node<string>("-");
            n.right.left = new Node<string>("1");
            n.right.right = CopyNode(tree.right.right);
        }
        return n;
    }

    public Node<string> PotenzGesetzIrightExpOne(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" || tree.key == "/")
            {
                if (tree.left.key == "^" /*&& tree.right.key == "^"*/)
                {
                    if (!tree.left.right.key.StartsWith("01.") /*&& !tree.right.right.key.StartsWith("01.")*/)
                    {
                        if (Math.Abs(Calculate(tree.left.left) - Calculate(tree.right)) < 0.0000001f)
                        {//Ueberlegen ob eine striktere Version des PGIII sinnvoll (nur wenn exponenten Baumidentisch und nicht nur rechenidentisch)
                            if (Math.Abs(Calculate(tree.left.left)) != 1) geht = true;
                        }
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIrightExpOne(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIrightExpOne(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = CopyNode(tree.left.left);
            if (tree.key == "*") n.right = new Node<string>("+");
            else n.right = new Node<string>("-");
            n.right.left = CopyNode(tree.left.right);
            n.right.right = new Node<string>("1");
        }
        return n;
    }

    public Node<string> MultiplyPotenceWithNumberOnLeftside(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" || tree.key == "/")
            {
                if (/*tree.left.key == "^" &&*/ tree.right.key == "^")
                {
                    if (/*!tree.left.right.key.StartsWith("01.") && */!op.Contains(tree.left.key))
                    {
                        if (!(tree.key == "/" && tree.left.key == "1")) geht = true;

                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = MultiplyPotenceWithNumberOnLeftside(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = MultiplyPotenceWithNumberOnLeftside(tree.right, ziel, path + "r");
        }
        else
        {
            if (tree.left.key == "1") n = CopyNode(tree.right);
            else
            {
                n = new Node<string>("^");
                n.right = CopyNode(tree.right.right);
                n.left = new Node<string>(tree.key);
                n.left.left = new Node<string>("^");
                n.left.left.left = CopyNode(tree.left);

                if (tree.right.right.key.StartsWith("01."))
                {
                    n.left.left.right = new Node<string>(tree.right.right.key.Substring(3));
                }
                else if (tree.right.right.key == "0.5")
                {
                    n.left.left.right = new Node<string>("2");
                }
                else
                {
                    n.left.left.right = new Node<string>("/");
                    if (tree.right.right.key != "/")
                    {
                        n.left.left.right.left = new Node<string>("1");
                        n.left.left.right.right = CopyNode(tree.right.right);
                    }
                    else
                    {
                        n.left.left.right.left = CopyNode(tree.right.right.right);
                        n.left.left.right.right = CopyNode(tree.right.right.left);
                    }
                }
                n.left.right = CopyNode(tree.right.left);
            }
        }
        return n;
    }

    public Node<string> MultiplyPotenceWithNumberOnRightside(Node<string> tree, string ziel, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" || tree.key == "/")
            {
                if (/*tree.left.key == "^" &&*/ tree.left.key == "^")
                {
                    if (/*!tree.left.right.key.StartsWith("01.") && */!op.Contains(tree.right.key))
                    {
                        geht = true;

                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = MultiplyPotenceWithNumberOnRightside(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = MultiplyPotenceWithNumberOnRightside(tree.right, ziel, path + "r");
        }
        else
        {
            if (tree.right.key == "1") n = CopyNode(tree.left);
            else
            {
                n = new Node<string>("^");
                n.right = CopyNode(tree.left.right);
                n.left = new Node<string>(tree.key);
                n.left.right = new Node<string>("^");
                n.left.right.left = CopyNode(tree.right);

                if (tree.left.right.key.StartsWith("01."))
                {
                    n.left.right.right = new Node<string>(tree.left.right.key.Substring(3));
                }
                else if (tree.left.right.key == "0.5")
                {
                    n.left.right.right = new Node<string>("2");
                }
                else
                {
                    n.left.right.right = new Node<string>("/");
                    if (tree.left.right.key != "/")
                    {
                        n.left.right.right.left = new Node<string>("1");
                        n.left.right.right.right = CopyNode(tree.left.right);
                    }
                    else
                    {
                        n.left.right.right.left = CopyNode(tree.left.right.right);
                        n.left.right.right.right = CopyNode(tree.left.right.left);
                    }
                }
                n.left.left = CopyNode(tree.left.left);
            }
        }
        return n;
    }

    public Node<string> PotenzGesetzISquare(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*")
            {

                if (Math.Abs(Calculate(tree.left) - Calculate(tree.right)) < 0.0000001f)
                {//Ueberlegen ob eine striktere Version des PGIII sinnvoll (nur wenn exponenten Baumidentisch und nicht nur rechenidentisch)
                    if (Math.Abs(Calculate(tree.left)) != 1) geht = true;
                }


            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzISquare(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzISquare(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = CopyNode(tree.left);
            n.right = new Node<string>("+");
            n.right.left = new Node<string>("1");
            n.right.right = new Node<string>("1");
        }
        return n;
    }

    public Node<string> PotenzGesetzIinnerProduct(Node<string> tree, string ziel, string path)//@"\frac{1}{81}\cdot{{3}^{2}}\cdot{{3}^{4}}"
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" /*|| tree.key == "/"*/)
            {
                if (tree.left.key == "*" && tree.right.key == "^")
                {
                    if (tree.left.right.key == "^")
                    {
                        if (!tree.left.right.right.key.StartsWith("01.") && !tree.right.right.key.StartsWith("01."))
                        {
                            if (Math.Abs(Calculate(tree.left.right.left) - Calculate(tree.right.left)) < 0.0000001f)
                            {//Ueberlegen ob eine striktere Version des PGIII sinnvoll (nur wenn exponenten Baumidentisch und nicht nur rechenidentisch)
                                geht = true;
                            }
                        }
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIinnerProduct(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIinnerProduct(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("*");
            n.left = CopyNode(tree.left.left);
            /*if (tree.key == "*")*/
            n.right = new Node<string>("^");
            //else n.right = new Node<string>("-");

            n.right.left = CopyNode(tree.left.right.left);
            n.right.right = new Node<string>("+");
            n.right.right.left = CopyNode(tree.left.right.right);
            n.right.right.right = CopyNode(tree.right.right);
        }
        return n;
    }



    public Node<string> PotenzGesetzII(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.left.key == "^")
                {
                    if (!tree.left.right.key.StartsWith("01.") && !tree.right.key.StartsWith("01."))
                    {
                        geht = true;
                    }

                }

            }



        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzII(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzII(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = CopyNode(tree.left.left);
            n.right = new Node<string>("*");
            n.right.left = CopyNode(tree.left.right);
            n.right.right = CopyNode(tree.right);
        }
        return n;
    }

    public Node<string> PotenzGesetzIII(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" || tree.key == "/")
            {
                if (tree.left.key == "^" && tree.right.key == "^")
                {
                    if (Math.Abs(Calculate(tree.left.right) - Calculate(tree.right.right)) < 0.0000001f)
                    {//Ueberlegen ob eine striktere Version des PGIII sinnvoll (nur wenn exponenten Baumidentisch und nicht nur rechenidentisch)
                        geht = true;
                    }

                }

            }



        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIII(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIII(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = new Node<string>(tree.key);//CopyNode(tree.left.left);
            n.left.left = CopyNode(tree.left.left);
            n.left.right = CopyNode(tree.right.left);
            //if (tree.key == "*") n.right = new Node<string>("+");
            //else n.right = new Node<string>("-");
            n.right = CopyNode(tree.left.right);
            //n.right.left = CopyNode(tree.left.right);
            //n.right.right = CopyNode(tree.right.right);
        }
        return n;
    }


    public Node<string> PotenzGesetzIIIinnerProduct(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "*" /*|| tree.key == "/"*/)
            {
                if (tree.left.key == "*" && tree.right.key == "^")
                {
                    if (tree.left.right.key == "^")
                    {
                        if (Math.Abs(Calculate(tree.left.right.right) - Calculate(tree.right.right)) < 0.0000001f)
                        {//Ueberlegen ob eine striktere Version des PGIII sinnvoll (nur wenn exponenten Baumidentisch und nicht nur rechenidentisch)
                            geht = true;
                        }
                    }

                }

            }



        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIIIinnerProduct(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIIIinnerProduct(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("*");
            n.left = CopyNode(tree.left.left);// new Node<string>(tree.key);//CopyNode(tree.left.left);
            n.right = new Node<string>("^");
            n.right.left = new Node<string>("*");
            n.right.left.left = CopyNode(tree.left.right.left);
            n.right.left.right = CopyNode(tree.right.left);
            n.right.right = CopyNode(tree.left.right.right);
        }
        return n;
    }



    public Node<string> PotenzGesetzIIrueckwaerts(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.right.key == "*")
                {
                    geht = true;
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIIrueckwaerts(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIIrueckwaerts(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = new Node<string>("^");
            n.left.left = CopyNode(tree.left);
            n.left.right = CopyNode(tree.right.left);
            n.right = CopyNode(tree.right.right);
        }

        return n;
    }



    public Node<string> PotenzGesetzIIrueckwaertsDivision(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.right.key == "/")
                {
                    if (tree.right.left.key != "1")
                    {
                        geht = true;
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIIrueckwaertsDivision(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIIrueckwaertsDivision(tree.right, ziel, path + "r");
        }
        else
        {
            n = new Node<string>("^");
            n.left = new Node<string>("^");
            n.left.left = CopyNode(tree.left);
            n.right = new Node<string>("/");
            n.right.right = CopyNode(tree.right.right);
            if (tree.right.left.key == "*")
            {
                n.left.right = CopyNode(tree.right.left.left);
                n.right.left = CopyNode(tree.right.left.right);
            }
            else
            {
                n.left.right = CopyNode(tree.right.left);
                n.right.left = new Node<string>("1");
            }

        }

        return n;
    }

    public Node<string> PotenzGesetzIIIrueckwaerts(Node<string> tree, string ziel, string path)
    {
        Node<string> n;
        bool geht = false;
        if (path == ziel)
        {
            if (tree.key == "^")
            {
                if (tree.left.key == "*" || tree.left.key == "/")
                {
                    //if (tree.left.left.key != "1" && tree.left.right.key != "1")
                    {
                        geht = true;
                    }
                }
            }
        }
        if (!geht)
        {
            n = new Node<string>(tree.key);
            if (tree.left != null) n.left = PotenzGesetzIIIrueckwaerts(tree.left, ziel, path + "l");
            if (tree.right != null) n.right = PotenzGesetzIIIrueckwaerts(tree.right, ziel, path + "r");
        }
        else
        {

            n = new Node<string>(tree.left.key);
            if (tree.left.left.key != "1")
            {
                n.left = new Node<string>("^");
                n.left.left = CopyNode(tree.left.left);
                n.left.right = CopyNode(tree.right);

            }
            else n.left = new Node<string>("1");
            if (tree.left.right.key != "1")
            {
                n.right = new Node<string>("^");
                n.right.left = CopyNode(tree.left.right);
                n.right.right = CopyNode(tree.right);
            }
            else n.right = new Node<string>("1");




        }

        return n;
    }

    public float Calculate(Node<string> tree)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        if (!op.Contains(tree.key))
        {
            string value = tree.key;
            while (value != RemoveBrackets(value)) value = RemoveBrackets(value);
            return mnt(value);
        }
        switch (tree.key)
        {
            case "+":
                {
                    return Calculate(tree.left) + Calculate(tree.right);
                }
            case "-":
                {
                    return Calculate(tree.left) - Calculate(tree.right);
                }
            case "*":
                {
                    return Calculate(tree.left) * Calculate(tree.right);
                }
            case "/":
                {
                    return Calculate(tree.left) / Calculate(tree.right);
                }
            case "^":
                {
                    if (tree.right.key.StartsWith("01."))
                    {
                        string value = tree.right.key.Substring(3);
                        while (value != RemoveBrackets(value)) value = RemoveBrackets(value);
                        float n = float.Parse(value);
                        return MathF.Pow(Calculate(tree.left), 1.0f / n);

                    }
                    else return MathF.Pow(Calculate(tree.left), Calculate(tree.right));
                }
        }
        return 0.0f;
    }

    private string RemoveBrackets(string text)
    {
        if (text.StartsWith("(") && text.EndsWith(")"))
        {
            int i = ComplementaryBracket(0, text, true);
            if (i == text.Length - 1) return text.Substring(1, text.Length - 2);
        }
        return text;
    }

    public Node<string> CopyNode(Node<string> tree)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        Node<string> n;
        n = new Node<string>(tree.key);
        if (tree.left != null) { n.left = CopyNode(tree.left); }
        if (tree.right != null) { n.right = CopyNode(tree.right); }
        return n;
    }

    public string TreeMarchingAddKey(Node<string> tree, string path)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        if (op.Contains(tree.key))
        {
            return TreeMarchingAddKey(tree.left, path + "l") + tree.key + path + " " + TreeMarchingAddKey(tree.right, path + "r");
        }
        else return "";

    }

    public string TreeMarchingContainsSqrt(Node<string> tree)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        if (op.Contains(tree.key))
        {
            if (tree.key == "^" && (tree.right.key.StartsWith("01.") || tree.right.key == "0.5"))
                return TreeMarchingContainsSqrt(tree.left) + "s " + TreeMarchingContainsSqrt(tree.right);
            else return TreeMarchingContainsSqrt(tree.left) + "n " + TreeMarchingContainsSqrt(tree.right);
        }
        else return "";

    }

    public string TreeMarching(Node<string> tree, string path, string weg)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        if (op.Contains(tree.key))
        {
            return TreeMarching(tree.left, path + "l", weg) + path + " " + TreeMarching(tree.right, path + "r", weg);
        }
        else return "";

    }

    public int Depth(Node<string> tree)
    {
        string[] op = new string[] { "*", "/", "+", "-", "^" };
        if (op.Contains(tree.key))
        {
            return 1 + Math.Max(Depth(tree.left), Depth(tree.right));
        }
        else return 0;

    }

    //Hilfsfunktion die dabei hilft, negative Vorzeichen von Subtraktion zu unterscheiden.
    private float mnt(String zahl)
    {
        zahl = zahl.Replace("(-", "0000");
        zahl = zahl.Replace(")", "");
        if (zahl.StartsWith("0000"))// Wir setzen in der Funktion NextLatexExpression 0000 anstatt (-, hier setzen wir das zurück
        {
            zahl = "-" + zahl.Substring(4);
        }
        //zahl = zahl.Replace(".", ",");
        return float.Parse(zahl);
    }

    private string mnt2(string zahl)
    {
        if (zahl.StartsWith("0000"))// Wir setzen in der Funktion NextLatexExpression 0000 anstatt (-, hier setzen wir das zurück
        {
            zahl = "(-" + zahl.Substring(4) + ")";
        }
        //zahl = zahl.Replace(".", ",");
        return zahl;

    }

    public int ComplementaryBracket(int i, String text, bool upOrDown)
    {

        int j = 1; int upDown = 1; char b1 = '('; char b2 = ')';
        if (!upOrDown)
        {
            upDown = -1;
            b1 = ')';
            b2 = '(';


        }
        char[] chars = text.ToCharArray();
        bool test = true;
        while (test)
        {
            i += upDown;
            if (chars[i] == b1)
            {
                j++;
            }
            if (chars[i] == b2)
            {
                j--;
                if (j == 0) test = false;
            }
        }
        return i;
    }


}
