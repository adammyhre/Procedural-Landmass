using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode{NoiseMap, ColorMap, Mesh};
	public DrawMode drawMode;

	public int mapWidth = 100;
	public int mapHeight = 100;
	public int seed = 0;
	public float mapScale = 30f;
	public int octaves = 4;
	[Range(0,1)]
	public float persistence = 0.5f;
	public float lacunarity = 2f;
	public Vector2 offset;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public TerrainType[] regions;

	public bool autoUpdate = false;

	public void GenerateMap(){
		float[,] noiseMap = Noise.GenerateNoise (mapWidth, mapHeight, seed, mapScale, octaves, persistence, lacunarity, offset);

		Color[] colorMap = new Color[mapWidth * mapHeight];

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						colorMap [y * mapWidth + x] = regions [i].color;
						break;
					}
				}
			}
		}

		MapDisplay display = FindObjectOfType<MapDisplay> ();
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap(noiseMap));
		}
		else if (drawMode == DrawMode.ColorMap) {
			display.DrawTexture (TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
		}
		else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
		}

	}

	void OnValidate(){
		if (mapWidth < 1)
			mapWidth = 1;
		if (mapHeight < 1)
			mapHeight = 1;
		if (lacunarity < 1)
			lacunarity = 1;
		if (octaves < 0)
			octaves = 0;
	}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color color;
}
