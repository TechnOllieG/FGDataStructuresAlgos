using UnityEngine;

namespace FutureGames.Lib
{
    public class TestPerlinNoise : MonoBehaviour
    {
        [SerializeField]
        Material outputMaterial = null;
        
        [SerializeField]
        int width = 512;

        [SerializeField]
        int height = 512;

        [SerializeField]
        int widthOffset = 0;
        
        [SerializeField]
        int heightOffset = 0;

        [SerializeField]
        float scale = 1f;

        [SerializeField]
        int octaves = 3;

        [SerializeField]
        float persistance = 1.2f;

        [SerializeField]
        float lacunarity = 2f;
        
        private Texture2D _noiseTexture;

        void OnValidate()
        {
            if (_noiseTexture == null || _noiseTexture.width != width || _noiseTexture.height != height)
            {
                _noiseTexture = new Texture2D(width, height);
            }
            Color[] pixels = PerlinNoise.GenerateHeightmap(width, height, scale, octaves, persistance, lacunarity, widthOffset, heightOffset).GetPixels();
            _noiseTexture.SetPixels(pixels);
            _noiseTexture.Apply();
            outputMaterial.mainTexture = _noiseTexture;
        }
    }
}