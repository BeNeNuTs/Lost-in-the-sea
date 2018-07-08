using UnityEngine;
using System.Collections;
using System;

public class MapDisplay : MonoBehaviour {

	public Renderer textureRender;
    public Terrain terrainRender;

	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

    public void DrawTerrain(float[,] noiseMap, TerrainType[] regions, Texture2D texture, int width, int height)
    {
        TerrainGenerator.Generate(terrainRender.terrainData, noiseMap, regions, texture, width, height);
    }

    public void UpdateDrawMode(MapGenerator.DrawMode drawMode)
    {
        textureRender.gameObject.SetActive(false);
        terrainRender.gameObject.SetActive(false);

        switch (drawMode)
        {
            case MapGenerator.DrawMode.NoiseMap:
            case MapGenerator.DrawMode.ColourMap:
            case MapGenerator.DrawMode.FallofMap:
                textureRender.gameObject.SetActive(true);
                break;
            case MapGenerator.DrawMode.Terrain:
                terrainRender.gameObject.SetActive(true);
                break;
        }
    }
}
