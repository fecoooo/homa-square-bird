using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	public void DestroyBlock()
	{
		GetComponent<ParticleSystem>().Play();
		GetComponent<Collider>().enabled = false;
		GetComponent<MeshRenderer>().enabled = false;
	}
}
