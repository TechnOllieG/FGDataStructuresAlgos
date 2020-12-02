using System.Collections.Generic;
using FG.Pathfinding;
using UnityEngine;
using Point = FG.Pathfinding.Point;

public class EnemyMovement : MonoBehaviour
{
    public float enemySpeed;
    public Transform target;
    public float trackDistance;
    public float minDistanceToPlayer = 1f;
    public bool visualizePath = false;
    public GameObject prefabUsedToVisualize;
    
    private Transform _transform;
    private PriorityQueueAStar priorityQueue = new PriorityQueueAStar();
    private Rigidbody _rigidbody;
    private Collider _collider;
    private List<Vector3> _path = new List<Vector3>();

    private const int UnitMultiplierForHeuristics = 10;
    private const float LinearMove = 1f;
    private const float DiagonalMove = 1.4f;
    private const int LinearMoveHeuristic = (int) (LinearMove * LinearMove) * UnitMultiplierForHeuristics;
    private const int DiagonalMoveHeuristic = (int) (DiagonalMove * DiagonalMove) * UnitMultiplierForHeuristics;

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        while (DistanceBetweenEnemyAndTarget() < MaxTrackingDistance() && DistanceBetweenEnemyAndTarget() > MinDistanceToPlayer())
        {
            if (priorityQueue.Count == 0)
            {
                priorityQueue.Enqueue(new Point(FeetPosition(), (int) HeuristicBetweenPointAndTarget(_transform.position), null));
            }
            
            if (DistanceBetweenPointAndTarget(priorityQueue.GetFirstElement().position) <= MinDistanceToPlayer())
            {
                Point point = priorityQueue.DequeueLast();
                if (visualizePath)
                {
                    Instantiate(prefabUsedToVisualize, point.position, Quaternion.identity, _transform);
                }

                continue;
            }
            Point currentPoint = priorityQueue.DequeueFirst();
            CheckNeighbours(currentPoint);
        }
        priorityQueue = new PriorityQueueAStar();
        if (_transform.childCount > 0)
        {
            for (int i = 0; i < _transform.childCount; i++)
            {
                Destroy(_transform.GetChild(i).gameObject);
            }
        }
    }

    private void CheckNeighbours(Point point)
    {
        Vector3 pos = point.position;
        List<Point> points = new List<Point>();
        
        // ----- Linear -----
        
        pos.Where(x: pos.x + 1); // Right of point
        if (CheckLinear())
            return;
        
        pos.Where(x: pos.x - 2); // Left of point
        if (CheckLinear())
            return;
        
        pos.Where(x: pos.x + 1, z: pos.z + 1); // In front of the point
        if (CheckLinear())
            return;
        
        pos.Where(z: pos.z - 2); // Behind the point
        if (CheckLinear())
            return;

        // ----- Diagonal -----
        
        pos.Where(x: pos.x + 1); // Behind to the right
        if (CheckDiagonal())
            return;

        pos.Where(x: pos.x - 2); // Behind to the left
        if (CheckDiagonal())
            return;

        pos.Where(z: pos.z + 2); // In front to the left
        if (CheckDiagonal())
            return;

        pos.Where(x: pos.x + 2); // In front to the right
        CheckDiagonal();
        EnqueueAll();

        bool CheckLinear()
        {
            if (DistanceBetweenPointAndTarget(pos) <= MinDistanceToPlayer())
            {
                return true;
            }
            if (priorityQueue.queue.FindIndex(a => a.position == pos) == -1 && !PointIsOccupied(pos))
            {
                points.Add(new Point(pos, (int) HeuristicBetweenPointAndTarget(pos) + LinearMoveHeuristic, point));
            }
            return false;
        }

        bool CheckDiagonal()
        {
            if (DistanceBetweenPointAndTarget(pos) <= MinDistanceToPlayer())
            {
                return true;
            }
            if (priorityQueue.queue.FindIndex(a => a.position == pos) == -1 && !PointIsOccupied(pos))
            {
                points.Add(new Point(pos, (int) HeuristicBetweenPointAndTarget(pos) + DiagonalMoveHeuristic, point));
            }
            return false;
        }

        void EnqueueAll()
        {
            foreach (Point tempPoint in points)
            {
                priorityQueue.Enqueue(tempPoint);
            }
        }
    }

    private bool PointIsOccupied(Vector3 point) => Physics.Raycast(point.Where(x: point.x + 1), Vector3.left, 1f);
    private float DistanceBetweenEnemyAndTarget() => (target.position - _transform.position).sqrMagnitude;
    private float HeuristicBetweenPointAndTarget(Vector3 pos) => (pos - target.position).sqrMagnitude * UnitMultiplierForHeuristics;
    private float DistanceBetweenPointAndTarget(Vector3 pos) => (pos - target.position).sqrMagnitude;
    private float MaxTrackingDistance() => trackDistance * trackDistance;
    private float MinDistanceToPlayer() => minDistanceToPlayer * minDistanceToPlayer;
    private Vector3 FeetPosition()
    {
        Vector3 pos = _transform.position;
        return pos.Where(y: pos.y - _collider.bounds.extents.y - 0.5f);
    }
}