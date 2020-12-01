using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FG.Pathfinding
{
    public class PriorityQueueAStar
    {
        public List<(Vector3, float)> queue = new List<(Vector3, float)>();

        public Vector3 Dequeue()
        {
            if (queue.Count == 0)
            {
                Console.WriteLine("There are no elements in the queue");
                return default;
            }
            return queue.First().Item1;
        }

        public void Enqueue(Vector3 element, float combinedHeuristic)
        {
            if (queue.Count == 0)
            {
                queue.Add((element, combinedHeuristic));
            }
            else
            {
                float smallestCombinedHeuristics = queue[0].Item2;
                for (int i = 0; i < queue.Count; i++)
                {
                    if (queue[i].Item2 < smallestCombinedHeuristics)
                    {
                        queue.Insert(i, (element, combinedHeuristic));
                    }
                }
            }
        }
    }
}
