using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
	public float speed = .2f;
	new Rigidbody rigidbody;

	void Start()
    {
		rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
		if (GameHandler.instance.CurrentState == GameState.InGame)
			rigidbody.MovePosition(rigidbody.position + new Vector3(speed, 0, 0));
    }
}
