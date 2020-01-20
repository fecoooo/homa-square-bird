using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourSingleton<Character>
{
	new Rigidbody rigidbody;
	public float jumpForce = 10f;
	public int framesToSpawn = 2;

	bool clicked = false;
	
    void Start()
    {
		rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
			clicked = true;
	}

	void FixedUpdate()
    {
		if (clicked)
		{
			clicked = false;
			SpawnEgg();
		}
		if(currentFrames == framesToSpawn)
		{
			GameObject egg = Instantiate(GamePreferences.instance.egg);
			egg.transform.position = savedPosition;
		}

		currentFrames++;
	}

	int currentFrames = 100;

	Vector3 savedPosition;

	public void SpawnEgg()
	{
		savedPosition = transform.position + new Vector3(0, 0.1f, 0);
		rigidbody.velocity = new Vector3(0, jumpForce, 0);
		currentFrames = 0;
	}
}
