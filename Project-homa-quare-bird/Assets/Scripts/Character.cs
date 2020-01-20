using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourSingleton<Character>
{
	new Rigidbody rigidbody;
	public float jumpForce = 10f;
	public float speed = .2f;
	bool mouseClick;

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
			mouseClick = true;	
	}

	void FixedUpdate()
    {
		

		if (mouseClick)
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
