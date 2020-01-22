using System;
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

	protected Vector3 TopForwardPoint
	{
		get => MiddleFrontPoint + new Vector3(0, colldier.bounds.extents.y + GamePreferences.instance.gravityPerFrame, 0);
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

	protected override void SetShouldUpdate()
	{
		shouldUpdate = GameHandler.instance.CurrentState == GameState.InGame || GameHandler.instance.CurrentState == GameState.GameWon;
	}

	protected override void SetHorizontalMovement()
	{
		if (!frontCollision)
		{
			RaycastHit hitInfo;

			frontCollision = Physics.Raycast(TopForwardPoint, new Vector3(1, 0, 0), out hitInfo, GamePreferences.instance.forwardCheckDistance);
			if (!frontCollision)
				frontCollision = Physics.Raycast(BottomForwardPoint, new Vector3(1, 0, 0), out hitInfo, GamePreferences.instance.forwardCheckDistance);

			if (frontCollision)
			{
				if (hitInfo.collider.name == "WinCollider")
				{
					foreach (GameObject egg in allEggs)
						egg.GetComponent<PhysicalObject>().DestroyObject(0);
					GameHandler.instance.TriggerGameWon();
				}
				else
					GameHandler.instance.TriggerGameLost();
			}
		}

		nextMovePos.x = frontCollision ? transform.position.x : transform.position.x + GamePreferences.instance.speed;
	}

	public override void DestroyObject(int framesToWait = 2)
	{
		if(GameHandler.instance.CurrentState == GameState.InGame)
			GetComponent<ParticleSystem>().Play();
	}

	protected override void SetVerticalMovement()
	{
		if (currentJumpFrame <= jumpFrames)
		{
			nextMovePos.y = transform.position.y + jumpStep;
			if (currentJumpFrame == jumpFrames)
				SpawnEgg();
			currentJumpFrame++;
		}
		else
		{
			base.SetVerticalMovement();
		}
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
		float spawnX = transform.position.x;
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
		Debug.DrawLine(TopForwardPoint, TopForwardPoint + new Vector3(1, 0, 0), Color.magenta);
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
