﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : PhysicalObject
{
	public int jumpFrames;
	public float jumpHeight = 1f;
	int currentJumpFrame = int.MaxValue;
	float jumpStep;
	Vector3 eggSpawnPositionYZ;

	Vector3 spawnPosition;

	List<GameObject> allEggs = new List<GameObject>();

	protected Vector3 ForwardTopPoint
	{
		get => BottomFrontPoint + new Vector3(0, colldier.size.y, 0);
	}
	public bool Ready { get; private set; }

	protected override void OnStart()
	{
		base.OnStart();

		spawnPosition = transform.position;

		jumpStep = jumpHeight / jumpFrames;

		GameHandler.instance.GameStateChanged += OnGameStateChanged;
	}

	private void Update()
	{
		if (GameHandler.instance.CurrentState != GameState.InGame)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			eggSpawnPositionYZ = transform.position;
			currentJumpFrame = 0;
		}
	}

	protected override void OnFixedUpdate()
	{
		if (GameHandler.instance.CurrentState != GameState.InGame)
			return;

		moveVector = Vector3.zero;

		if (!frontCollision)
		{
			RaycastHit hitInfo;

			frontCollision = Physics.Raycast(ForwardTopPoint, new Vector3(1, 0, 0), out hitInfo, GamePreferences.instance.bottomCheckDistance);
			if(!frontCollision)
				frontCollision = Physics.Raycast(ForwardPoint, new Vector3(1, 0, 0), out hitInfo, GamePreferences.instance.bottomCheckDistance);

			if (frontCollision)
			{
				if(hitInfo.collider.name == "WinCollider")
					GameHandler.instance.TriggerGameWon();
				else
					GameHandler.instance.TriggerGameLost();
			}
		}

		moveVector.x = frontCollision ? 0 : GamePreferences.instance.speed;

		grounded = Physics.Raycast(BottomFrontPoint, Vector3.down, GamePreferences.instance.bottomCheckDistance) ||
			Physics.Raycast(BottomRearPoint, Vector3.down, GamePreferences.instance.bottomCheckDistance);

		if (!grounded)
			moveVector.y = GamePreferences.instance.gravityPerFrame;

		if (currentJumpFrame <= jumpFrames)
		{
			moveVector.y = jumpStep;
			if (currentJumpFrame == jumpFrames)
				SpawnEgg();
			currentJumpFrame++;
		}

		transform.position += moveVector;
	}

	void ResetPosition()
	{
		transform.position = spawnPosition;
	}

	void OnGameStateChanged(GameState state)
	{
		switch (state)
		{
			case GameState.BeforeGame:
				ClearEggs();
				ResetPosition();
				ResetProperies();
				Ready = true;
				break;
			case GameState.InGame:
				break;
			case GameState.GameWon:
				Ready = false;
				break;
			case GameState.GameLost:
				Ready = false;
				break;
			default:
				break;
		}
	}

	public void SpawnEgg()
	{
		GameObject egg = Instantiate(GamePreferences.instance.egg);
		float spawnX = transform.position.x + GamePreferences.instance.speed;
		egg.transform.position = new Vector3(spawnX, eggSpawnPositionYZ.y, eggSpawnPositionYZ.z);

		allEggs.Add(egg);
	}

	void ClearEggs()
	{
		foreach (GameObject go in allEggs)
			Destroy(go);
		allEggs.Clear();
	}

	protected override void DebugDraw()
	{
		base.DebugDraw();
		Debug.DrawLine(ForwardTopPoint, ForwardTopPoint + new Vector3(1, 0, 0), Color.magenta);
	}


	public static Character instance;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogError($"Multiple instances of singleton class: {typeof(Character)}\n");
			Debug.Break();
		}
		instance = this;
	}
}
