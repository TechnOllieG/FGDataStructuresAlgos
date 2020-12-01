using FG.Pathfinding;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float enemySpeed;
    public Transform currentTarget;
    
    private Transform _transform;
    private PriorityQueueAStar queue = new PriorityQueueAStar();
    private GameObject pointer;

    private void Awake()
    {
        _transform = transform;
        pointer = new GameObject();
        pointer.name = "Pointer";
        SphereCollider collider = pointer.AddComponent<SphereCollider>();
        pointer.AddComponent<Rigidbody>().useGravity = false;
        
        collider.isTrigger = true;
        collider.radius = 0.1f;
    }

    private void Update()
    {
        
    }

    private bool PointIsOccupied(Vector3 point) => Physics.Raycast(point, Vector3.forward, 0.01f);
}
