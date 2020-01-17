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

	void FixedUpdate()
    {
		if (Input.GetMouseButtonDown(0))
		{
			SpawnEgg();
		}
	}


	public void SpawnEgg()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y + 1.15f, transform.position.z);
		//GameObject egg = Instantiate(GamePreferences.instance.egg);
		//egg.transform.position = transform.position;

	}
}
