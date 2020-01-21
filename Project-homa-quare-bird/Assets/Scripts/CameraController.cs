using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform focus;
	float differenceOnStart;

	private void Start()
	{
		differenceOnStart = transform.position.x - focus.position.x;
	}

	void Update()
    {
		transform.position = new Vector3(focus.position.x + differenceOnStart, transform.position.y, transform.position.z);
    }
}
