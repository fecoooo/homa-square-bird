using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject:MonoBehaviour
{
	public bool drawDebug = true;
	public float speed = .1f;
	public float bottomCheckDistance = .15f;
	public float forwardCheckDistance = .15f;

	protected Vector3 moveVector = new Vector3();

	protected bool grounded;
	protected BoxCollider colldier;

	protected bool frontCollision;

	protected Vector3 BottomRearPoint
	{
		get => colldier.bounds.center + new Vector3(-colldier.bounds.extents.x, -colldier.bounds.extents.y, 0);
	}

	protected Vector3 BottomFrontPoint
	{
		get => colldier.bounds.center + new Vector3(colldier.bounds.extents.x, -colldier.bounds.extents.y, 0);
	}

	protected Vector3 ForwardPoint
	{
		get => BottomFrontPoint - new Vector3(0, 1 * GamePreferences.instance.gravityPerFrame, 0);
	}

	protected virtual void OnStart()
	{
		colldier = GetComponent<BoxCollider>();
	}

	void Start()
	{
		OnStart();
	}

	protected virtual bool EarlyFixedUpdate()
	{
		if (GameHandler.instance.CurrentState != GameState.InGame)
			return false;

		moveVector = Vector3.zero;
		return true;
	}

	protected virtual void OnFixedUpdate()
	{
		if (!frontCollision)
		{
			frontCollision = Physics.Raycast(ForwardPoint, new Vector3(1, 0, 0), bottomCheckDistance);
			//TODO: place to proper location (where not intersecting with other)
			if (!frontCollision)
				moveVector.x = speed;
		}

		grounded = Physics.Raycast(BottomFrontPoint, Vector3.down, bottomCheckDistance) || Physics.Raycast(BottomRearPoint, Vector3.down, bottomCheckDistance);

		if (!grounded)
			moveVector.y = GamePreferences.instance.gravityPerFrame;
	}

	protected virtual void LateFixedUpade()
	{
		transform.position += moveVector;
	}

	void FixedUpdate()
	{
		if (EarlyFixedUpdate())
		{
			OnFixedUpdate();
			LateFixedUpade();
		}
	}

	private void LateUpdate()
	{
		if (drawDebug)
		{
			Debug.DrawLine(BottomRearPoint, BottomRearPoint - new Vector3(0, 1, 0), Color.red);
			Debug.DrawLine(BottomFrontPoint, BottomFrontPoint - new Vector3(0, 1, 0), Color.blue);
			Debug.DrawLine(ForwardPoint, ForwardPoint + new Vector3(1, 0, 0), Color.green);
		}
	}
}
