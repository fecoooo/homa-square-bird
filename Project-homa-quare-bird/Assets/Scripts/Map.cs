using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviourSingleton<Map>
{
	const string MapDataPath = "Levels/level_";

	public int Width { get; private set; }
	public int Height { get; private set; }

	BlockType[,] mapData;

    void Start()
    {
		Generate(1);
    }

    void Update()
    {
			
    }

	public void Generate(int level)
	{
		ReadMapDataFromTexture(level);
		for (int i = 0; i < Width; ++i)
		{
			for (int j = 0; j < Height; ++j)
				InstantiateBlock(i, j);
		}
	}

	void InstantiateBlock(int i, int j)
	{
		GameObject block;
		switch (mapData[i, j])
		{
			case BlockType.None:
				return;
			case BlockType.Dirt:
				block = Instantiate(GamePreferences.instance.DirtBlock);
				break;
			case BlockType.GrassyDirt:
				block = Instantiate(GamePreferences.instance.GrassyDirtBlock);
				break;
			case BlockType.TopDirt:
				throw new NotImplementedException();
			default:
				throw new Exception("No such blocktype defined");
		}

		block.transform.position = new Vector3(i, .5f + j, 0);
		block.transform.parent = transform;
	}

	/*
	Information in mapData stored differently! Stored by columns, not rows.
	
	[2 5]
	[1 4]
	[0 3]
	*/

	void ReadMapDataFromTexture(int level)
	{
		Texture2D mapDataTex = Resources.Load<Texture2D>(MapDataPath + level);

		Width = mapDataTex.width;
		Height = mapDataTex.height;

		mapData = new BlockType[Width, Height];

		Color pixelColor;
		for (int i = 0; i < Width; ++i)
		{
			for (int j = 0; j < Height; ++j)
			{
				pixelColor = mapDataTex.GetPixel(i, j);

				bool isBlackButNothingAbove = mapDataTex.GetPixel(i, j) == Color.black && mapDataTex.GetPixel(i, j + 1) == Color.white;
				bool topBlock = j == Height - 1;
				if (isBlackButNothingAbove && !topBlock)
					pixelColor = Color.green;

				mapData[i, j] = GetBlockTypeByColor(pixelColor);
			}
		}
	}

	private BlockType GetBlockTypeByColor(Color color)
	{
		if (color == Color.black)
			return BlockType.Dirt;
		if (color == Color.green)
			return BlockType.GrassyDirt;
		else if (color == Color.white)
			return BlockType.None;
		else
			throw new Exception("There's no blockytype associated for this color.");
	}

	public enum BlockType
	{
		None,
		Dirt,
		GrassyDirt,
		TopDirt
	}
}
