using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
	public float speed = .2f;
	

	void Start()
    {
        
    }

    void FixedUpdate()
    {
		transform.Translate(speed, 0, 0);
    }
}
