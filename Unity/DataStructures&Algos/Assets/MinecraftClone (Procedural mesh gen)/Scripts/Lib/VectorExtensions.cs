using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Returns a new Vector3 with some components changed or altered relative to the original
    /// </summary>
    /// <param name="t">This Vector3</param>
    /// <param name="x">x replacement value</param>
    /// <param name="y">y replacement value</param>
    /// <param name="z">z replacement value</param>
    /// <param name="relX">Value relative to x</param>
    /// <param name="relY">Value relative to y</param>
    /// <param name="relZ">Value relative to z</param>
    /// <returns></returns>
    public static Vector3 Where(this Vector3 t, 
        float? x = null, float? y = null, float? z = null, 
        float? relX = null, float? relY = null, float? relZ = null)
    {
        return new Vector3((x ?? t.x) + (relX ?? 0), 
            (y ?? t.y) + (relY ?? 0), 
            (z ?? t.z) + (relZ ?? 0));
    }
    
    /// <summary>
    /// Returns a new Vector3Int with some components changed or altered relative to the original
    /// </summary>
    /// <param name="t">This Vector3Int</param>
    /// <param name="x">x replacement value</param>
    /// <param name="y">y replacement value</param>
    /// <param name="z">z replacement value</param>
    /// <param name="relX">Value relative to x</param>
    /// <param name="relY">Value relative to y</param>
    /// <param name="relZ">Value relative to z</param>
    /// <returns></returns>
    public static Vector3Int Where(this Vector3Int t, 
        int? x = null, int? y = null, int? z = null, 
        int? relX = null, int? relY = null, int? relZ = null)
    {
        return new Vector3Int((x ?? t.x) + (relX ?? 0), 
            (y ?? t.y) + (relY ?? 0), 
            (z ?? t.z) + (relZ ?? 0));
    }
}
