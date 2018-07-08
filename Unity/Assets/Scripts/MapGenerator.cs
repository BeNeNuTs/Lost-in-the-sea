using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, ColourMap, FallofMap, Terrain};
	public DrawMode drawMode;

	public int mapWidth;
	public int mapHeight;
	public float noiseScale;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

    public bool useFallofMap;
    public float fallofA = 3.0f;
    public float fallofB = 2.2f;

	public bool autoUpdate;

	public TerrainType[] regions;

    float[,] fallofMap;

    public void Awake()
    {
        fallofMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight, fallofA, fallofB);
    }

    public void GenerateMap() {
		float[,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

		Color[] colourMap = new Color[mapWidth * mapHeight];
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
                if(useFallofMap)
                {
                    noiseMap[x,y] -= Mathf.Clamp01(fallofMap[x,y]);
                }
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						colourMap [y * mapWidth + x] = regions [i].colour;
						break;
					}
				}
			}
		}

		MapDisplay display = FindObjectOfType<MapDisplay> ();
        display.UpdateDrawMode(drawMode);
        switch(drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                break;
            case DrawMode.ColourMap:
                display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
                break;
            case DrawMode.FallofMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight, fallofA, fallofB)));
                break;
            case DrawMode.Terrain:
                display.DrawTerrain(noiseMap, regions, TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight), mapWidth, mapHeight);
                break;
        }
	}

	void OnValidate() {
        mapWidth = Mathf.Clamp(mapWidth, 1, 1000);
        mapHeight = Mathf.Clamp(mapHeight, 1, 1000);
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}

        fallofMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight, fallofA, fallofB);
    }
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}