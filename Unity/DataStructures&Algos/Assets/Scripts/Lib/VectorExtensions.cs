using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 Where(this Vector3 t, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? t.x, y ?? t.y, z ?? t.z);
    }
    
    public static Vector3Int Where(this Vector3Int t, int? x = null, int? y = null, int? z = null)
    {
        return new Vector3Int(x ?? t.x, y ?? t.y, z ?? t.z);
    }
}
