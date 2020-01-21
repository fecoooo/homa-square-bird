using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : PhysicalObject
{
	public int jumpFrames;
	int currentJumpFrame;
	float jumpStep;

	protected override void OnStart()
	{
		base.OnStart();
		jumpStep = 1f / jumpFrames;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
			currentJumpFrame = 0;
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		if (currentJumpFrame < jumpFrames)
		{
			moveVector.y = jumpStep;
			currentJumpFrame++;
		}
	}

	public void SpawnEgg()
	{
		//transform.position = new Vector3(transform.position.x, transform.position.y + 1.15f, transform.position.z);
		//GameObject egg = Instantiate(GamePreferences.instance.egg);
		//egg.transform.position = transform.position;
	}
}
