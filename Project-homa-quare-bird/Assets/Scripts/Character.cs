using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	new Rigidbody rigidbody;

    void Start()
    {
		rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{

		}
    }


	public void SpawnEgg()
	{
		GameObject.CreatePrimitive(PrimitiveType.Sphere);
	}
}
