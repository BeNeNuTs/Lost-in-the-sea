using UnityEngine;
using System.Collections;

public static class TerrainGenerator
{
    private static TerrainData m_TerrainData;
    private static float[,] m_NoiseMap;
    private static TerrainType[] m_Regions;
    private static Texture2D m_Texture;
    private static int m_Width;
    private static int m_Height;

    public static void Generate(TerrainData tData, float[,] noiseMap, TerrainType[] regions, Texture2D texture, int width, int height)
    {
        m_TerrainData = tData;
        m_NoiseMap = noiseMap;
        m_Regions = regions;
        m_Texture = texture;
        m_Width = width;
        m_Height = height;

        InitTerrain();
        CreateTerrain();
    }

    static void InitTerrain()
    {
        float terrainDepth = 20.0f;

        //Initialisation du Terrain
        m_TerrainData.heightmapResolution = Mathf.FloorToInt(Mathf.Max(m_Width, m_Height)) + 1;
        m_TerrainData.alphamapResolution = m_TerrainData.heightmapResolution - 1;
        m_TerrainData.baseMapResolution = m_TerrainData.alphamapResolution;
        m_TerrainData.size = new Vector3(m_Width, terrainDepth, m_Height);

        //Init main texture of Terrain (baseTexture)
        SplatPrototype[] initTexture = new SplatPrototype[1];
        initTexture[0] = new SplatPrototype();
        initTexture[0].texture = m_Texture;
        initTexture[0].tileSize = new Vector2(m_Width, m_Height);

        m_TerrainData.splatPrototypes = initTexture;

        //Init alphamap with the base texture
        float[,,] map = new float[m_TerrainData.alphamapWidth, m_TerrainData.alphamapHeight, m_TerrainData.alphamapLayers];
        for (int y = 0; y < m_TerrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < m_TerrainData.alphamapWidth; x++)
            {
                map[x, y, 0] = 1f;
            }
        }
        m_TerrainData.SetAlphamaps(0, 0, map);

        /*treeInstances = new TreeInstance[0];
        m_TerrainData.treeInstances = treeInstances;
        m_TerrainData.treePrototypes = new TreePrototype[0];

        water.waterGameobject.SetActive(false);
        water.waterGameobject.transform.position = new Vector3(water.waterGameobject.transform.position.x, water.minPosY, water.waterGameobject.transform.position.z);*/
    }

    static void CreateTerrain()
    {
        //Création du Terrain en fonction du noiseMap

        float[,] correctedNoise = new float[m_Width, m_Height];
        for (int i = 0; i < m_Width; i++)
        {
            for (int j = 0; j < m_Height; j++)
            {
                correctedNoise[i, j] = m_NoiseMap[j, i];
            }
        }
        m_TerrainData.SetHeights(0, 0, correctedNoise);

        //AddTextures();

        //RefreshTerrainCollider();

        //AddTrees();
        //AddWater();
    }

    static void RefreshTerrainCollider()
    {
        float[,] terrainHeights = m_TerrainData.GetHeights(0, 0, m_TerrainData.heightmapWidth, m_TerrainData.heightmapHeight);
        m_TerrainData.SetHeights(0, 0, terrainHeights);
    }

    static void AddTextures()
    {
        Texture2D[] textures = new Texture2D[m_Regions.Length];

        for (int i = 0; i < m_Regions.Length; i++)
        {
            Color[] colorMap = new Color[m_Width * m_Height];
            for(int j = 0; j < m_Width * m_Height; j++)
            {
                colorMap[i] = m_Regions[i].colour;
            }

            textures[i] = TextureGenerator.TextureFromColourMap(colorMap, m_Width, m_Height);
        }

        // Ajout des textures au Terrain
        SplatPrototype[] terrainTexture = new SplatPrototype[textures.Length];
        for (int i = 0; i < textures.Length; i++)
        {
            terrainTexture[i] = new SplatPrototype();
            terrainTexture[i].texture = textures[i];
            terrainTexture[i].tileSize = new Vector2(2, 2);
        }
        m_TerrainData.splatPrototypes = terrainTexture;

        float[,,] map = new float[m_TerrainData.alphamapWidth, m_TerrainData.alphamapHeight, m_TerrainData.alphamapLayers];

        for (int y = 0; y < m_TerrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < m_TerrainData.alphamapWidth; x++)
            {
                int nbCollisionMap = 0;

                for (int i = 0; i < m_Regions.Length; i++)
                {
                    if (m_NoiseMap[x,y] > m_Regions[i].height)
                    {
                        map[x, y, i] = 1;
                        nbCollisionMap++;
                    }
                    else
                    {
                        map[x, y, i] = 0;
                    }
                }

                if (nbCollisionMap > 1)
                {
                    for (int i = 0; i < m_Regions.Length; i++)
                    {
                        if (map[x, y, i] == 1)
                        {
                            map[x, y, i] /= nbCollisionMap;
                        }
                    }
                }
            }
        }

        //Assignation de la nouvelle alphamap
        m_TerrainData.SetAlphamaps(0, 0, map);
    }

    /*void AddTrees()
    {

        // Ajout des arbres au Terrain
        TreePrototype[] treesProto = new TreePrototype[trees.treesGamobject.Length];

        for (int i = 0; i < trees.treesGamobject.Length; i++)
        {
            treesProto[i] = new TreePrototype();
            treesProto[i].prefab = trees.treesGamobject[i];
        }

        m_TerrainData.treePrototypes = treesProto;

        int nbTrees = Random.Range(trees.minTree, trees.maxTree);

        treeInstances = new TreeInstance[nbTrees];

        for (int i = 0; i < nbTrees; i++)
        {

            int xPos = 0;
            int zPos = 0;
            int index = 0;
            do
            {
                xPos = Random.Range(0, hMap.width);
                zPos = Random.Range(0, hMap.height);

                index++;
            } while ((m_TerrainData.GetHeight(zPos, xPos) < textures[1].minHeight || m_TerrainData.GetHeight(zPos, xPos) > textures[1].maxHeight) && index < 1000);

            //float yPos = hMap.GetPixel(xPos + offset, zPos + offset).grayscale * m_TerrainData.size.y - offset;
            float yPos = m_TerrainData.GetHeight(zPos, xPos);
            Vector3 position = new Vector3(zPos, yPos, xPos);

            position = new Vector3(position.x / hMap.width, position.y / m_TerrainData.size.y, position.z / hMap.height);

            treeInstances[i].position = position;
            treeInstances[i].widthScale = 0;
            treeInstances[i].heightScale = 0;
            treeInstances[i].color = Color.white;
            treeInstances[i].lightmapColor = Color.white;
            treeInstances[i].prototypeIndex = Random.Range(0, trees.treesGamobject.Length - 1);
        }

        m_TerrainData.treeInstances = treeInstances; ;

        StartCoroutine(GrowTrees());
    }

    IEnumerator GrowTrees()
    {
        for (int j = 0; j < 100; j++)
        {
            for (int i = 0; i < treeInstances.Length; i++)
            {
                treeInstances[i].widthScale += 0.01f;
                treeInstances[i].heightScale += 0.01f;
            }

            m_TerrainData.treeInstances = treeInstances;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        RefreshTerrainCollider();
    }

    void AddWater()
    {

        //Check if water is necessary
        int cpt = 0;
        for (int y = 0; y < m_TerrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < m_TerrainData.alphamapWidth; x++)
            {
                if (m_TerrainData.GetHeight(y, x) < textures[0].maxHeight)
                {
                    cpt++;
                }
            }
        }

        float percentage = ((float)cpt / (float)(m_TerrainData.alphamapHeight * m_TerrainData.alphamapWidth));
        Debug.Log("Pourcentage sable : " + percentage);
        if (percentage < water.waterProbability)
        {
            return;
        }

        water.waterGameobject.SetActive(true);

        StartCoroutine(RaiseWater());
    }

    IEnumerator RaiseWater()
    {
        float oldPosY = water.waterGameobject.transform.position.y;
        float newPosY = textures[0].maxHeight + water.offsetY;

        float posY;
        float time = 100 * Time.deltaTime;
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            posY = Mathf.Lerp(oldPosY, newPosY, elapsedTime / time);
            water.waterGameobject.transform.position = new Vector3(water.waterGameobject.transform.position.x, posY, water.waterGameobject.transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        water.waterGameobject.transform.position = new Vector3(water.waterGameobject.transform.position.x, newPosY, water.waterGameobject.transform.position.z);
    }*/

}
