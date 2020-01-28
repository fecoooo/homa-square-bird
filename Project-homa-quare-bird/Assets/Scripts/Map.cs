﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map:MonoBehaviourSingleton<Map>
{
	readonly Color Gray = new Color(0.5019608f, 0.5019608f, 0.5019608f);
	const string MapDataPath = "Levels/level_";
	const float timeBetweenCannonShots = .3f;

	List<GameObject> allBlocks = new List<GameObject>();

	Transform endPlatform;

	public int Width { get; private set; }
	public int Height { get; private set; }
	public bool Ready { get; private set; }

	BlockType[,] mapData;

	List<GameObject> cannons = new List<GameObject>();

	void Start()
	{
		endPlatform = transform.Find("EndPlatform");
		GameHandler.instance.GameStateChanged += OnGameStateChanged;

		foreach(Transform child in transform.Find("EndPlatform/Props"))
		{
			if(child.name.Contains("Cannon"))
				cannons.Add(child.gameObject);
		}
	}

	void OnGameStateChanged(GameState state)
	{
		switch (state)
		{
			case GameState.BeforeGame:
				ClearAllBlock();
				Generate(GameHandler.instance.CurrentLevel);
				Ready = true;
				break;
			case GameState.InGame:
				break;
			case GameState.GameWon:
				Ready = false;
				StartCoroutine(ShootCannons());
				break;
			case GameState.GameLost:
				Ready = false;
				break;
			default:
				break;
		}
	}

	IEnumerator ShootCannons()
	{
		foreach(GameObject c in cannons)
		{
			c.gameObject.SetActive(true);
			yield return new WaitForSeconds(timeBetweenCannonShots);
			Taptic.Success();
			c.GetComponent<ParticleSystem>().Play();
		}
	}

	public void Generate(int level)
	{
		ReadMapDataFromTexture(level);
		for (int i = 0; i < Width; ++i)
		{
			for (int j = 0; j < Height; ++j)
				InstantiateBlock(i, j);
		}

		endPlatform.transform.position = new Vector3(Width, 0, 0);
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
			case BlockType.TopDirt:
				block = Instantiate(GamePreferences.instance.TopDirtBlock);
				break;
			case BlockType.GrassyDirt:
				block = Instantiate(GamePreferences.instance.GrassyDirtBlock);
				break;
			case BlockType.GrassyDirtWithScore:
				block = Instantiate(GamePreferences.instance.GrassyDirtBlockWithScore);
				break;
			case BlockType.Stone:
				block = Instantiate(GamePreferences.instance.StoneBlock);
				break;
			case BlockType.GrassyStoneWithScore:
				block = Instantiate(GamePreferences.instance.GrassyStoneBlockWithScore);
				break;
			default:
				throw new Exception("No such blocktype defined");
		}

		block.transform.position = new Vector3(i, j, 0);
		block.transform.parent = transform;
		allBlocks.Add(block);
	}

	void ClearAllBlock()
	{
		foreach (GameObject go in allBlocks)
			Destroy(go);
		allBlocks.Clear();
	}

	public void DestroyBlocks(RaycastHit[] infos)
	{
		foreach(RaycastHit i in infos)
		{
			Block block = i.collider.GetComponent<Block>();
			if (block != null && (block.tag != "Unbreakable" || block.tag == "UnbreakableScore"))
				block.DestroyBlock();
		}
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

		for (int i = 0; i < Width; ++i)
		{
			for (int j = 0; j < Height; ++j)
				mapData[i, j] = GetBlockTypeByColor(mapDataTex.GetPixel(i, j));
		}
	}

	private BlockType GetBlockTypeByColor(Color color)
	{
		

		if (color == Color.black)
			return BlockType.Dirt;
		if (color == Color.blue)
			return BlockType.TopDirt;
		if (color == Color.green)
			return BlockType.GrassyDirt;
		else if (color == Color.white)
			return BlockType.None;
		else if (color == Color.red)
			return BlockType.GrassyDirtWithScore;
		else if (color == Gray)
			return BlockType.Stone;
		else if (color == Color.magenta)
			return BlockType.GrassyStoneWithScore;
		else
			throw new Exception("There's no blockytype associated for this color: " + color);
	}

	public enum BlockType
	{
		None,
		Dirt,
		TopDirt,
		GrassyDirt,
		GrassyDirtWithScore,
		Stone,
		GrassyStoneWithScore
	}
}