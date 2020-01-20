using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourSingleton<Character>
{
	new Rigidbody rigidbody;
	public float jumpForce = 10f;

    void Start()
    {
		rigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
    {
		if (Input.GetMouseButtonDown(0))
		{
			SpawnEgg();
		}
	}


	public void SpawnEgg()
	{
		rigidbody.velocity = new Vector3(0, jumpForce, 0);
		//transform.position = new Vector3(transform.position.x, transform.position.y + 1.15f, transform.position.z);
		//GameObject egg = Instantiate(GamePreferences.instance.egg);
		//egg.transform.position = transform.position;

	}
}
