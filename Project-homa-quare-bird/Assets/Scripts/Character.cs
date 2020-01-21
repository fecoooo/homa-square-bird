using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : PhysicalObject
{
	public int jumpFrames;
	public float jumpHeight = 1f;
	int currentJumpFrame;
	float jumpStep;
	Vector3 eggSpawnPositionYZ;

	protected override void OnStart()
	{
		base.OnStart();
		jumpStep = jumpHeight / jumpFrames;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			eggSpawnPositionYZ = transform.position;
			currentJumpFrame = 0;
		}
	}

	protected override void OnFixedUpdate()
	{
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
}
