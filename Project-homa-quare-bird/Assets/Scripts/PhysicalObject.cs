using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject:MonoBehaviour
{
	public bool drawDebug = true;

	protected Vector3 nextMovePos = new Vector3();

	protected BoxCollider colldier;

	protected bool frontCollision;
	protected bool shouldUpdate;

	protected Vector3 MiddleRearPoint
	{
		get => colldier.bounds.center - new Vector3(colldier.bounds.extents.x, 0, 0);
	}

	protected Vector3 MiddleFrontPoint
	{
		get => colldier.bounds.center + new Vector3(colldier.bounds.extents.x, 0, 0);
	}

	protected Vector3 BottomForwardPoint
	{
		get => colldier.bounds.center + new Vector3(colldier.bounds.extents.x, -colldier.bounds.extents.y - GamePreferences.instance.gravityPerFrame, 0);
	}

	protected virtual void OnStart()
	{
		colldier = GetComponent<BoxCollider>();
	}

	void Start()
	{
		OnStart();
	}

	protected virtual void OnFixedUpdate()
	{
		SetShouldUpdate();

		if (!shouldUpdate)
			return;

		SetHorizontalMovement();
		SetVerticalMovement();

		transform.position = nextMovePos;

		if (frontCollision)
			DestroyObject();
	}

	protected virtual void SetShouldUpdate()
	{
		shouldUpdate = GameHandler.instance.CurrentState == GameState.InGame && !frontCollision;
	}

	protected virtual void SetHorizontalMovement()
	{
		if (!frontCollision)
			frontCollision = Physics.Raycast(BottomForwardPoint, new Vector3(1, 0, 0), GamePreferences.instance.bottomCheckDistance);

		nextMovePos.x = frontCollision ? transform.position.x : transform.position.x + GamePreferences.instance.speed;
	}

	protected virtual void SetVerticalMovement()
	{
		RaycastHit hitInfo;
		nextMovePos.y = transform.position.y + GamePreferences.instance.gravityPerFrame;

		//BottomFrontPoint
		if (Physics.Raycast(MiddleFrontPoint, Vector3.down, out hitInfo, .5f))
		{
			if (hitInfo.collider.bounds.Intersects(colldier.bounds))
			{
				float possibleNewY = hitInfo.collider.bounds.max.y + colldier.bounds.extents.y;
				nextMovePos.y = possibleNewY > nextMovePos.y ? possibleNewY : nextMovePos.y;
			}
		}

		//BottomRearPoint
		if (Physics.Raycast(MiddleRearPoint, Vector3.down, out hitInfo, .5f))
		{
			if (hitInfo.collider.bounds.Intersects(colldier.bounds))
			{
				float possibleNewY = hitInfo.collider.bounds.max.y + colldier.bounds.extents.y;
				nextMovePos.y = possibleNewY > nextMovePos.y ? possibleNewY : nextMovePos.y;
			}
		}
	}

	public virtual void DestroyObject(int framesToWait = 2)
	{
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<ParticleSystem>().Play();
		StartCoroutine(DestroyObjectDelayedPart(framesToWait));
	}

	IEnumerator DestroyObjectDelayedPart(int framesToWait)
	{
		for (int i = 0; i < framesToWait; ++i)
			yield return new WaitForFixedUpdate();

		GetComponent<Collider>().enabled = false;
	}

	void FixedUpdate()
	{
		OnFixedUpdate();
	}

	protected virtual void DebugDraw()
	{
		Debug.DrawLine(MiddleFrontPoint, MiddleFrontPoint + new Vector3(0, -1, 0), Color.blue);
		Debug.DrawLine(MiddleRearPoint, MiddleRearPoint + new Vector3(0, -1, 0), Color.red);
		Debug.DrawLine(BottomForwardPoint, BottomForwardPoint + new Vector3(1, 0, 0), Color.green);
	}

	private void LateUpdate()
	{
		if (drawDebug)
			DebugDraw();
	}

	protected void ResetProperies()
	{
		frontCollision = false;
	}
}
