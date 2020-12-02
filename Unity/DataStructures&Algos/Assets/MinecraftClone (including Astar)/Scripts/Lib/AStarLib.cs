using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FG.Pathfinding
{
    public class PriorityQueueAStar
    {
        public int Count => queue.Count;
        public List<Point> queue = new List<Point>();

        public Point DequeueFirst()
        {
            if (queue.Count == 0)
            {
                Console.WriteLine("There are no elements in the queue");
                return default;
            }
            Point point = queue.First();
            queue.Remove(point);
            return point;
        }

        public Point GetFirstElement()
        {
            if (queue.Count == 0)
            {
                Console.WriteLine("There are no elements in the queue");
                return default;
            }
            return queue.First();
        }
        
        public Point DequeueLast()
        {
            if (queue.Count == 0)
            {
                Console.WriteLine("There are no elements in the queue");
                return default;
            }
            Point point = queue.Last();
            queue.Remove(point);
            return point;
        }

        public Point GetLastElement()
        {
            if (queue.Count == 0)
            {
                Console.WriteLine("There are no elements in the queue");
                return default;
            }
            return queue.Last();
        }

        public void Enqueue(Point point)
        {
            if (queue.Count == 0)
            {
                queue.Add(point);
            }
            else
            {
                float smallestCombinedHeuristics = queue[0].heuristic;
                for (int i = 0; i < queue.Count; i++)
                {
                    if (queue[i].heuristic < smallestCombinedHeuristics)
                    {
                        queue.Insert(i, point);
                    }
                }
            }
        }
    }

    public class Point
    {
        public Vector3 position;
        public int heuristic;
        public Point previousPoint;

        public Point(Vector3 position, int heuristic, Point previousPoint)
        {
            this.position = position;
            this.heuristic = heuristic;
            this.previousPoint = previousPoint;
        }
    }
}
