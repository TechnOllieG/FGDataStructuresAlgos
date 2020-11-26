using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerOfHanoi : MonoBehaviour
{
    public int amountOfDiscs;

    public bool slowMovement = true;
    public float moveSpeed = 0.2f;
    public float updateFrequency = 0.02f;

    public Transform tower1;
    public Transform tower2;
    public Transform tower3;
    public RectTransform progressBarUI;
    public Text percentageCounter;
    public Text moveCounter;
    
    [Header("Debug")]
    
    public ulong amountOfMoves = 0;
    public ulong totalRequiredMoves = 0;
    public float progressBar = 0;

    private readonly Stack<Transform> _tower1stack = new Stack<Transform>();
    private readonly Stack<Transform> _tower2stack = new Stack<Transform>();
    private readonly Stack<Transform> _tower3stack = new Stack<Transform>();
    
    private Dictionary<int, Stack<Transform>> _stackDictionary = new Dictionary<int, Stack<Transform>>();
    private Dictionary<int, Transform> _towerDictionary = new Dictionary<int, Transform>();

    private IEnumerator Start()
    {
        _stackDictionary.Add(1, _tower1stack);
        _towerDictionary.Add(1, tower1);
        
        _stackDictionary.Add(2, _tower2stack);
        _towerDictionary.Add(2, tower2);
        
        _stackDictionary.Add(3, _tower3stack);
        _towerDictionary.Add(3, tower3);
        
        Vector3 tower1pos = tower1.position;
        for (int i = 0; i < amountOfDiscs; i++)
        {
            Transform currentObject = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            currentObject.localScale = new Vector3(amountOfDiscs - i, 0.5f, amountOfDiscs - i);
            currentObject.position = new Vector3(tower1pos.x, tower1pos.y + i * 0.5f, tower1pos.z);
            _tower1stack.Push(currentObject);
        }

        string suffix = amountOfDiscs > 1 ? "s" : "";
        totalRequiredMoves = AmountOfMoves(amountOfDiscs);
        
        Debug.Log($"Starting tower of hanoi algorithm...\n" +
                  $"Moving {amountOfDiscs} disc{suffix} will require {totalRequiredMoves} move{suffix}");

        yield return StartCoroutine(TOH(amountOfDiscs));
        
        Debug.Log($"Successfully moved {amountOfDiscs} discs from tower 1 to tower 3, performed {amountOfMoves} moves");
    }

    public IEnumerator TOH(int discsAmount, int origin = 1, int via = 2, int destination = 3)
    {
        if (discsAmount == 1)
        {
            amountOfMoves++;
            progressBar = Convert.ToSingle(Convert.ToDecimal(amountOfMoves) / Convert.ToDecimal(totalRequiredMoves));
            progressBarUI.localScale = new Vector3(progressBar, 1f, 1f);
            percentageCounter.text = $"{Convert.ToInt16(progressBar * 100)} %";
            moveCounter.text = $"Amount of moves: {amountOfMoves} / {totalRequiredMoves}";
            _stackDictionary.TryGetValue(origin, out var originStack);
            _stackDictionary.TryGetValue(destination, out var destinationStack);
            _towerDictionary.TryGetValue(destination, out var destinationTower);

            if (originStack != default && destinationTower != default && destinationStack != default)
            {
                Vector3 newPosition = new Vector3(destinationTower.position.x, destinationTower.position.y + destinationStack.Count * 0.5f, destinationTower.position.z);
                
                Transform currentObject = originStack.Pop();

                if (slowMovement)
                {
                    float t = 0;
                
                    while (t < 1f)
                    {
                        currentObject.position = Vector3.Lerp(currentObject.position, newPosition, t);
                        t += moveSpeed;
                        yield return new WaitForSeconds(updateFrequency);
                    }
                }

                currentObject.position = newPosition;
                destinationStack.Push(currentObject);
            }
        }
        else if (discsAmount == 2)
        {
            yield return StartCoroutine(TOH(1, origin, destination, via));
            yield return StartCoroutine(TOH(1, origin, via, destination));
            yield return StartCoroutine(TOH(1, via, origin, destination));
        }
        else
        {
            yield return StartCoroutine(TOH(discsAmount - 1, origin, destination, via));
            yield return StartCoroutine(TOH(1, origin, via, destination));
            yield return StartCoroutine(TOH(discsAmount - 1, via, origin, destination));
        }
    }

    public ulong AmountOfMoves(int discsAmount)
    {
        if (discsAmount == 1)
            return 1;
        return AmountOfMoves(discsAmount - 1) * 2 + 1;
    }
}
