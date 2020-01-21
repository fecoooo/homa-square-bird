using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
	public bool drawDebug = true;
	public float speed = .1f;
	public float distanceFromGround = .1f;

	protected Vector3 moveVector = new Vector3();

	protected bool grounded;
	protected BoxCollider colldier;

	protected bool frontCollision;

	protected Vector3 FrontPoint
	{
		get => colldier.bounds.center + new Vector3(colldier.bounds.extents.x, -colldier.bounds.extents.y, 0);
	}

	protected Vector3 RearPoint
	{
		get => colldier.bounds.center + new Vector3(-colldier.bounds.extents.x, -colldier.bounds.extents.y, 0);
	}


	protected virtual void OnStart()
	{
		colldier = GetComponent<BoxCollider>();
	}

	void Start()
	{
		OnStart();
	}



	protected virtual void EarlyFixedUpdate()
	{
		if (GameHandler.instance.CurrentState != GameState.InGame)
			return;

		moveVector = Vector3.zero;
	}

	protected virtual void OnFixedUpdate()
	{
		moveVector.x = speed;

		frontCollision = Physics.Raycast(FrontPoint, Vector3.forward, distanceFromGround);
		grounded = Physics.Raycast(FrontPoint, Vector3.down, distanceFromGround) || Physics.Raycast(RearPoint, Vector3.down, distanceFromGround);

		if (!grounded)
			moveVector.y = GamePreferences.instance.gravityPerFrame;
	}

	protected virtual void LateFixedUpade()
	{
		transform.position += moveVector;
	}

	void FixedUpdate()
	{
		EarlyFixedUpdate();
		OnFixedUpdate();
		LateFixedUpade();
	}

	private void LateUpdate()
	{
		if (drawDebug)
		{
			Debug.DrawLine(RearPoint, RearPoint - new Vector3(0, 1, 0), Color.red);
			Debug.DrawLine(FrontPoint, FrontPoint - new Vector3(0, 1, 0), Color.blue);
			Debug.DrawLine(FrontPoint, FrontPoint + new Vector3(1, 0, 0), Color.green);
		}
	}
}
