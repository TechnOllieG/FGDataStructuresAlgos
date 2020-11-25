using UnityEngine;

namespace FutureGames.Lib
{
    public static class PerlinNoise
    {
        public static Texture2D GenerateHeightmap(int width, int height, float scale,
            int octaves, float persistance, float lacunarity, int widthOffset = 0, int heightOffset = 0)
        {
            Texture2D r = new Texture2D(width, height);

            r.SetPixels(Generate(width, height, scale, octaves, persistance, lacunarity, widthOffset, heightOffset).ToGrayscaleArray());
            r.Apply();

            return r;
        }

        public static float[,] Generate(int width, int height, float scale,
            int octaves, float persistance, float lacunarity, int widthOffset = 0, int heightOffset = 0)
        {
            float[,] r = new float[width, height];

            scale = Mathf.Max(0.0001f, scale);

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for (int o = 0; o < octaves; o++)
                    {
                        float sampleX = (x + widthOffset) / scale * frequency;
                        float sampleY = (y + heightOffset) / scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    r[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    r[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, r[x, y]);
                }
            }

            return r;
        }
    }
}