using System.Collections.Generic;
using UnityEngine;
using FGDataStructuresAlgos;
using UnityEngine.UI;

public enum SearchMode
{
    Depth,
    Breadth
}

public class TreeVisualize : MonoBehaviour
{
    public Vector2 offset = new Vector2(15f, 10f);
    public GameObject nodePrefab;
    public bool search = false;
    public SearchMode searchMode = SearchMode.Depth;
    public int numberToSearchFor = 0;
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
        Visualize(tree.Root, Vector2.zero, offset);
        if (search)
        {
            switch (searchMode)
            {
                case SearchMode.Depth:
                    Debug.Log(tree.DepthContains(numberToSearchFor));
                    break;
                case SearchMode.Breadth:
                    Debug.Log(tree.BreadthContains(numberToSearchFor));
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < _linePoints.Count; i += 2)
        {
            Gizmos.DrawLine(_linePoints[i], _linePoints[i + 1]);
        }
    }

    private void Visualize(BinaryTreeNode<int> node, Vector2 pos, Vector2 offsetTemp)
    {
        Text text = Instantiate(nodePrefab.gameObject, pos, Quaternion.identity, _canvas.transform).GetComponent<Text>();

        text.text = node.Value.ToString();

        if (node.Left != null)
        {
            Vector2 newPos = pos + new Vector2(-offsetTemp.x, -offset.y);
            _linePoints.Add(pos - new Vector2(0f, lineAttachmentOffset));
            _linePoints.Add(newPos + new Vector2(0f, lineAttachmentOffset));
            Visualize(node.Left, newPos, offsetTemp / 2f);
        }

        if (node.Right != null)
        {
            Vector2 newPos = pos + new Vector2(offsetTemp.x, -offset.y);
            _linePoints.Add(pos - new Vector2(0f, lineAttachmentOffset));
            _linePoints.Add(newPos + new Vector2(0f, lineAttachmentOffset));
            Visualize(node.Right, newPos, offsetTemp / 2f);
        }
    }
}
