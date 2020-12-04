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
            return queue[0];
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
                for (int i = 0; i < queue.Count; i++)
                {
                    if (point.heuristic < queue[i].heuristic)
                    {
                        queue.Insert(i, point);
                        return;
                    }
                }

                queue.Add(point);
            }
        }
    }
    
    [Serializable]
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
