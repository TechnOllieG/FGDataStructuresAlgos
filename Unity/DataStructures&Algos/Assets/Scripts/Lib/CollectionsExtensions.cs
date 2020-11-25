using UnityEngine;

namespace FutureGames.Lib
{
    public static class CollectionsExtensions
    {
        public static Color[] ToGrayscaleArray(this float[,] t)
        {
            Color[] r = new Color[t.GetLength(0) * t.GetLength(1)];

            int c = -1;
            for (int y = 0; y < t.GetLength(1); y++)
            {
                for (int x = 0; x < t.GetLength(0); x++)
                {
                    c++;
                    r[c] = new Color(t[x, y], t[x, y], t[x, y], 1f);
                }
            }

            return r;
        }

        public static float[,] Amplify(this float[,] t, float amplitude)
        {
            float[,] r = new float[t.GetLength(0), t.GetLength(1)];

            for (int y = 0; y < t.GetLength(1); y++)
            {
                for (int x = 0; x < t.GetLength(0); x++)
                {
                    r[x, y] = t[x, y] * amplitude;
                }
            }

            return r;
        }
    }
}