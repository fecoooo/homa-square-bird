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

	protected Vector3 ForwardTopPoint
	{
		get => BottomFrontPoint + new Vector3(0, colldier.size.y, 0);
	}

	protected override void OnStart()
	{
		base.OnStart();
		jumpStep = jumpHeight / jumpFrames;
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
		if (!frontCollision)
			frontCollision = Physics.Raycast(ForwardTopPoint, new Vector3(1, 0, 0), GamePreferences.instance.bottomCheckDistance);

		base.OnFixedUpdate();

		if (currentJumpFrame <= jumpFrames)
		{
			moveVector.y = jumpStep;
			if (currentJumpFrame == jumpFrames)
				SpawnEgg();
			currentJumpFrame++;
		}
	}

	public void SpawnEgg()
	{
		GameObject egg = Instantiate(GamePreferences.instance.egg);
		float spawnX = transform.position.x + GamePreferences.instance.speed;
		egg.transform.position = new Vector3(spawnX, eggSpawnPositionYZ.y, eggSpawnPositionYZ.z);
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
