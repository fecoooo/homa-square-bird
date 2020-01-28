using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	const float RotationSpeed = -5f;

    void FixedUpdate()
    {
		transform.Rotate(0, 0, RotationSpeed);
    }
}
