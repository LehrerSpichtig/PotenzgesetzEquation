using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T>
{
    public T key;
    public Node<T> left, right;

    public Node(T item)
    {
        key = item;
        left = null;//new("");
        right = null;//new("");
    }

}
