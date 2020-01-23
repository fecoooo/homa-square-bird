using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GammaChanger : MonoBehaviour
{
	Slider slider;

	private void Start()
	{
		slider = GetComponent<Slider>();
	}

	public void OnGammaChange()
	{
		RenderSettings.ambientLight = new Color(slider.value, slider.value, slider.value, 1.0f);
	}
}
