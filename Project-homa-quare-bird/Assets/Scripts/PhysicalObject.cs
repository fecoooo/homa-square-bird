using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject:MonoBehaviour
{
	public bool drawDebug = true;

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

	protected virtual void OnFixedUpdate()
	{
		if (GameHandler.instance.CurrentState != GameState.InGame)
			return;

		moveVector = Vector3.zero;

		if (!frontCollision)
			frontCollision = Physics.Raycast(ForwardPoint, new Vector3(1, 0, 0), GamePreferences.instance.bottomCheckDistance);

		moveVector.x = frontCollision ?  0 : GamePreferences.instance.speed;

		grounded = Physics.Raycast(BottomFrontPoint, Vector3.down, GamePreferences.instance.bottomCheckDistance) || 
			Physics.Raycast(BottomRearPoint, Vector3.down, GamePreferences.instance.bottomCheckDistance);

		if (!grounded)
			moveVector.y = GamePreferences.instance.gravityPerFrame;

		transform.position += moveVector;
	}

	void FixedUpdate()
	{
		OnFixedUpdate();
	}

	protected virtual void DebugDraw()
	{
		Debug.DrawLine(BottomRearPoint, BottomRearPoint - new Vector3(0, 1, 0), Color.red);
		Debug.DrawLine(BottomFrontPoint, BottomFrontPoint - new Vector3(0, 1, 0), Color.blue);
		Debug.DrawLine(ForwardPoint, ForwardPoint + new Vector3(1, 0, 0), Color.green);
	}

	private void LateUpdate()
	{
		if (drawDebug)
			DebugDraw();
	}

	protected void ResetProperies()
	{
		grounded = false;
		frontCollision = false;
	}
}
