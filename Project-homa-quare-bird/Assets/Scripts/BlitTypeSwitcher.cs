#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BlitTypeSwitcher : MonoBehaviour
{
	private void Start()
	{
		if(Application.isPlaying)
			Destroy(gameObject);
	}

	void Update()
    {
		if(PlayerSettings.Android.blitType != AndroidBlitType.Always)
			PlayerSettings.Android.blitType = AndroidBlitType.Always;
	}
}
#endif