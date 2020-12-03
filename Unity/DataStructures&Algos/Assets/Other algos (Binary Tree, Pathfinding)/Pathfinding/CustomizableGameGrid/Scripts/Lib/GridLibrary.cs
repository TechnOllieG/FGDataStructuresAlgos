using System;
using UnityEngine;
using Axes = TechnOllieG.CustomizableGameGrid.Grid2D.Axes;

namespace TechnOllieG.CustomizableGameGrid
{
    public struct OccupiedPointData
    {
        public object occupier; // Reference to the object that occupies the point, can contain a reference to whatever, a class, a transform, a GameObject etc.
        public int occupiedPointIndex; // The index of the occupied point

        public OccupiedPointData(object occupier, int occupiedPointIndex)
        {
            this.occupier = occupier;
            this.occupiedPointIndex = occupiedPointIndex;
        }
    }
    
    public enum Direction
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ
    }

    public static class GridLibrary
    {
        #region Grid2D specifc extension methods
        // Depending on the axes selected, the length/width will mean different things. Therefore these Width-/Length-Component extension methods will help.
        // XY = X: Width, Y: Length
        // XZ = X: Width, Z:Length
        // ZY = Z: Width, Y: Length
        
        /// <summary>
        /// Will return a copy of this Vector with the specified component(s) changed, depending on the 2 selected axes.
        /// </summary>
        public static Vector3 WhereComponent(this Vector3 t, Axes axes,
            float? length = null,
            float? depth = null,
            float? width = null)
        {
            Vector3 returnVector = t;
            returnVector.LengthComponent(axes) = length ?? t.LengthComponentVal(axes);
            returnVector.DepthComponent(axes) = depth ?? t.DepthComponentVal(axes);
            returnVector.WidthComponent(axes) = width ?? t.WidthComponentVal(axes);
            
            return returnVector;
        }
        
        /// <summary>
        /// <para>Will return the reference to the Vector3 component symbolizing width depending on the 2 selected axes.</para>
        /// <para>if axes = XY or XZ will return x,</para>
        /// if axes = ZY will return z
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ref float WidthComponent(this ref Vector3 t, Axes axes)
        {
            switch(axes)
            {
                case Axes.XY:
                    return ref t.x;
                case Axes.XZ:
                    return ref t.x;
                case Axes.ZY:
                    return ref t.z;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axes), axes, null);
            }
        }
        
        /// <summary>
        /// <para>Same as WidthComponent() but will return the value of the Vector3 component symbolizing width depending on the 2 selected axes.</para>
        /// <para>if axes = XY or XZ will return x,</para>
        /// if axes = ZY will return z
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float WidthComponentVal(this Vector3 t, Axes axes)
        {
            switch(axes)
            {
                case Axes.XY:
                    return t.x;
                case Axes.XZ:
                    return t.x;
                case Axes.ZY:
                    return t.z;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axes), axes, null);
            }
        }
        
        /// <summary>
        /// <para>Will return the reference to the Vector3 component symbolizing depth (remains constant for all points) depending on the 2 selected axes.</para>
        /// <para>if axes = XY will return z,</para>
        /// if axes = XZ will return y,
        /// <para>if axes = ZY will return x</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ref float DepthComponent(this ref Vector3 t, Axes axes2D)
        {
            switch(axes2D)
            {
                case Axes.XY:
                    return ref t.z;
                case Axes.XZ:
                    return ref t.y;
                case Axes.ZY:
                    return ref t.x;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axes2D), axes2D, null);
            }
        } 
        
        /// <summary>
        /// <para>Same as ConstantComponent() but will not return a value instead of a reference.</para>
        /// <para>if axes = XY will return z,</para>
        /// if axes = XZ will return y,
        /// <para>if axes = ZY will return x</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float DepthComponentVal(this Vector3 t, Axes axes)
        {
            switch(axes)
            {
                case Axes.XY:
                    return t.z;
                case Axes.XZ:
                    return t.y;
                case Axes.ZY:
                    return t.x;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axes), axes, null);
            }
        }

        /// <summary>
        /// <para>Will return the reference to the Vector3 component symbolizing length depending on the 2 selected axes.</para>
        /// <para>if axes = XY or ZY will return y,</para>
        /// if axes = XZ will return z
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ref float LengthComponent(this ref Vector3 t, Axes axes)
        {
            switch(axes)
            {
                case Axes.XY:
                    return ref t.y;
                case Axes.XZ:
                    return ref t.z;
                case Axes.ZY:
                    return ref t.y;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axes), axes, null);
            }
        }
        
        /// <summary>
        /// <para>Same as LengthComponent() but will return the value of the Vector3 component symbolizing length depending on the 2 selected axes.</para>
        /// <para>if axes = XY or ZY will return y,</para>
        /// if axes = XZ will return z
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float LengthComponentVal(this Vector3 t, Axes axes)
        {
            switch(axes)
            {
                case Axes.XY:
                    return t.y;
                case Axes.XZ:
                    return t.z;
                case Axes.ZY:
                    return t.y;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axes), axes, null);
            }
        }
        #endregion
    }
}