using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject:MonoBehaviour
{
	public bool drawDebug = true;

	protected Vector3 nextMovePos = new Vector3();

	new protected BoxCollider collider;

	protected Tuple<RaycastHit, RaycastHit> verticalHitinfos = new Tuple<RaycastHit, RaycastHit>(new RaycastHit(), new RaycastHit());

	protected bool frontCollision;
	protected bool shouldUpdate;

	protected Vector3 MiddleRearPoint
	{
		get => collider.bounds.center - new Vector3(collider.bounds.extents.x, 0, 0);
	}

	protected Vector3 MiddleFrontPoint
	{
		get => collider.bounds.center + new Vector3(collider.bounds.extents.x, 0, 0);
	}

	protected Vector3 BottomForwardPoint
	{
		get => collider.bounds.center + new Vector3(collider.bounds.extents.x, -collider.bounds.extents.y - GamePreferences.instance.gravityPerFrame * 2, 0);
	}

	protected virtual void OnStart()
	{
		collider = GetComponent<BoxCollider>();
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
		verticalHitinfos = SetVerticalMovement();

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

	protected virtual Tuple<RaycastHit, RaycastHit> SetVerticalMovement()
	{
		RaycastHit hitInfo1;
		RaycastHit hitInfo2;
		nextMovePos.y = transform.position.y + GamePreferences.instance.gravityPerFrame;

		//BottomFrontPoint
		if (Physics.Raycast(MiddleFrontPoint, Vector3.down, out hitInfo1, .5f))
		{
			if (hitInfo1.collider.bounds.Intersects(collider.bounds))
			{
				float possibleNewY = hitInfo1.collider.bounds.max.y + collider.bounds.extents.y - GamePreferences.instance.collisionErrorCorrection;
				nextMovePos.y = possibleNewY > nextMovePos.y ? possibleNewY : nextMovePos.y;
			}
		}

		//BottomRearPoint
		if (Physics.Raycast(MiddleRearPoint, Vector3.down, out hitInfo2, .5f))
		{
			if (hitInfo2.collider.bounds.Intersects(collider.bounds))
			{
				float possibleNewY = hitInfo2.collider.bounds.max.y + collider.bounds.extents.y - GamePreferences.instance.collisionErrorCorrection;
				nextMovePos.y = possibleNewY > nextMovePos.y ? possibleNewY : nextMovePos.y;
			}
		}

		return new Tuple<RaycastHit, RaycastHit>(hitInfo1, hitInfo2);
	}

	public virtual void DestroyObject(int framesToWait = 2)
	{
		GetComponentInChildren<MeshRenderer>().enabled = false;
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
