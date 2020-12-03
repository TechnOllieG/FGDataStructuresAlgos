using UnityEngine;

namespace TechnOllieG.CustomizableGameGrid
{
    [System.Serializable]
    public struct ObjectSpawnModule
    {
        public GameObject objectToSpawn;
        public int[] indicesToSpawnOn;
    }

    public enum TargetGrid
    {
        Grid2DSingleton,
        Grid2D
    }
    
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private TargetGrid targetGrid = TargetGrid.Grid2DSingleton;
        [SerializeField] private Grid2D gridInstance = default;
        [SerializeField] private ObjectSpawnModule[] objectsToSpawn = new ObjectSpawnModule[0];

        private void Awake()
        {
            if(targetGrid == TargetGrid.Grid2DSingleton)
                gridInstance = Grid2DSingleton.Instance;

            foreach (ObjectSpawnModule module in objectsToSpawn)
            {
                for (int i = 0; i < module.indicesToSpawnOn.Length; i++)
                {
                    GameObject currentObject = Instantiate(module.objectToSpawn,
                        CalculateOffset(gridInstance.gridPoints[module.indicesToSpawnOn[i]], module.objectToSpawn),
                        gridInstance.CalculateObjectRotation(),
                        transform);
                    gridInstance.SetPointOccupied(currentObject, i);
                }
            }
        }

        private Vector3 CalculateOffset(Vector3 position, GameObject objectToOffset)
        {
            Grid2D.Axes ax = gridInstance.gridAxes;
            Vector3 bounds = objectToOffset.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            
            switch(gridInstance.gridPointObjectSnapping)
            {
                case Grid2D.ObjectSnapping.MeshBoundsTopToGrid:
                    return position.WhereComponent(ax, depth: position.DepthComponentVal(ax) + bounds.y / 2f);
                case Grid2D.ObjectSnapping.Centered:
                    return position;
                case Grid2D.ObjectSnapping.MeshBoundsBottomToGrid:
                    return position.WhereComponent(ax, depth: position.DepthComponentVal(ax) - bounds.y / 2f);
                default:
                    Debug.LogWarning("There is no case in the switch statement to handle this variation");
                    return Vector3.zero;
            }
        }
    }
}
