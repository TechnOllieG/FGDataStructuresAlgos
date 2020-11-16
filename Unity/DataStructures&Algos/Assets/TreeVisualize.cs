﻿using System.Collections.Generic;
using UnityEngine;
using FGDataStructuresAlgos;
using UnityEngine.UI;

public class TreeVisualize : MonoBehaviour
{
    public Vector2 offset = new Vector2(15f, 10f);
    public GameObject nodePrefab;
    public int[] input;
    public float lineAttachmentOffset = 1.7f;
    
    private Canvas _canvas;
    private List<Vector2> _linePoints = new List<Vector2>();

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        BinaryTree<int> tree = new BinaryTree<int>();
        foreach (int integer in input)
        {
            tree.Add(integer);
        }
        Visualize(tree.Root, Vector2.zero);
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < _linePoints.Count; i += 2)
        {
            Gizmos.DrawLine(_linePoints[i], _linePoints[i + 1]);
        }
    }

    private void Visualize(BinaryTreeNode<int> node, Vector2 pos)
    {
        Text text = Instantiate(nodePrefab.gameObject, pos, Quaternion.identity, _canvas.transform).GetComponent<Text>();

        text.text = node.Value.ToString();

        if (node.Left != null)
        {
            Vector2 newPos = pos + new Vector2(-offset.x, -offset.y);
            _linePoints.Add(pos - new Vector2(0f, lineAttachmentOffset));
            _linePoints.Add(newPos + new Vector2(0f, lineAttachmentOffset));
            Visualize(node.Left, newPos);
        }

        if (node.Right != null)
        {
            Vector2 newPos = pos + new Vector2(offset.x, -offset.y);
            _linePoints.Add(pos - new Vector2(0f, lineAttachmentOffset));
            _linePoints.Add(newPos + new Vector2(0f, lineAttachmentOffset));
            Visualize(node.Right, newPos);
        }
    }
}
