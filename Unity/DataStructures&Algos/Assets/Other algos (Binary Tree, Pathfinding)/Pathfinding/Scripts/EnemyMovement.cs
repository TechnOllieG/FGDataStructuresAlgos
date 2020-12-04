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

	[Header("Debug")]
	
	public List<Point> queue = new List<Point>();
	
	private PriorityQueueAStar _priorityQueue = new PriorityQueueAStar();
	private List<Point> done = new List<Point>();
	private Transform _transform;
	private Rigidbody _rigidbody;
	private Collider _collider;
	private Stack<Vector3> _path = new Stack<Vector3>();
	private bool _pathDone = false;

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

	private void Start()
	{
		target = GameObject.FindWithTag("Player").transform;
		queue = _priorityQueue.queue;
	}

	private void Update()
	{
		if (_priorityQueue.Count == 0)
		{
			Vector3 pos = _transform.position;
			_priorityQueue.Enqueue(new Point(pos, (int) HeuristicBetweenPointAndTarget(pos), null));
		}
		
		if (DistanceBetweenEnemyAndTarget() < MaxTrackingDistance())
		{
			if (DistanceBetweenEnemyAndTarget() > MinDistanceToPlayer())
			{
				if (DistanceBetweenPointAndTarget(_priorityQueue.GetFirstElement().position) <= MinDistanceToPlayer()) 
				{
					Point point = _priorityQueue.GetFirstElement();
					while (point != null && !_pathDone)
					{
						var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.transform.parent = _transform;
						cube.transform.position = point.position.Where(relY: 1);
						cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
						cube.GetComponent<MeshRenderer>().material.color = Color.yellow;
					
						point = point.previousPoint;
					}
					_pathDone = true;
					return;
				}
				if (_pathDone)
				{
					ClearPath();
					return;
				}
			
				Point currentPoint = _priorityQueue.DequeueFirst();
				done.Add(currentPoint);
				CheckNeighbours(currentPoint);
			}
		}
		else
		{
			ClearPath();
		}
	}

	private void CheckNeighbours(Point point)
	{
		// Debug.Log("Checking neighbours");
		Vector3 pos = point.position;
		List<Point> points = new List<Point>();

		// ----- Linear -----
        
		pos = pos.Where(relX: 1); // Right of point
		Check(LinearMoveHeuristic);
        
		pos = pos.Where(relX: -2); // Left of point
		Check(LinearMoveHeuristic);
        
		pos = pos.Where(relX: 1, relZ: 1); // In front of the point
		Check(LinearMoveHeuristic);
        
		pos = pos.Where(relZ: -2); // Behind the point
		Check(LinearMoveHeuristic);

		// ----- Diagonal -----

		pos = pos.Where(relX: 1); // Behind to the right
		Check(DiagonalMoveHeuristic);

		pos = pos.Where(relX: -2); // Behind to the left
		Check(DiagonalMoveHeuristic);

		pos = pos.Where(relZ: 2); // In front to the left
		Check(DiagonalMoveHeuristic);

		pos = pos.Where(relX: 2); // In front to the right
		Check(DiagonalMoveHeuristic);
		EnqueueAll();

		void Check(int moveHeuristic)
		{
			if ((!PointIsOccupied(pos) || pos == target.position) && done.FindIndex(a => a.position == pos) == -1)
			{
				int heuristic = (int) HeuristicBetweenPointAndTarget(pos) + moveHeuristic;
				int index = 0;
				Point tempPoint;
				
				if ((index = _priorityQueue.queue.FindIndex(a => a.position == pos)) == -1)
				{
					points.Add(new Point(pos, heuristic, point));
				}
				else if(heuristic < (tempPoint = _priorityQueue.queue[index]).heuristic)
				{
					tempPoint.heuristic = heuristic;
					tempPoint.previousPoint = point;
				}
			}
		}

		void EnqueueAll()
		{
			foreach (Point tempPoint in points)
			{
				_priorityQueue.Enqueue(tempPoint);
				if (PointIsOccupied(tempPoint.position))
					return;
				var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				cube.transform.position = tempPoint.position;
				cube.transform.parent = _transform;
				cube.GetComponent<MeshRenderer>().material.color = Color.red;
			}
		}
	}

	private void ClearPath()
	{
		Debug.Log("Clearing path");
		_priorityQueue.queue.Clear();
		done.Clear();
		_pathDone = false;

		for (int i = 0; i < _transform.childCount; i++)
		{
			Destroy(_transform.GetChild(i).gameObject);
		}
	}

	private bool PointIsOccupied(Vector3 point) => Physics.Raycast(point.Where(relX: 1), Vector3.left, 1f);
	private float DistanceBetweenEnemyAndTarget() => (target.position - _transform.position).sqrMagnitude;
	private float HeuristicBetweenPointAndTarget(Vector3 pos) => (pos - target.position).sqrMagnitude * UnitMultiplierForHeuristics;
	private float DistanceBetweenPointAndTarget(Vector3 pos) => (pos - target.position).sqrMagnitude;
	private float MaxTrackingDistance() => trackDistance * trackDistance;
	private float MinDistanceToPlayer() => minDistanceToPlayer * minDistanceToPlayer;
	private Vector3 FeetPosition()
	{
		Vector3 pos = _transform.position;
		return pos.Where(relY: -_collider.bounds.extents.y - 0.5f);
	}
}