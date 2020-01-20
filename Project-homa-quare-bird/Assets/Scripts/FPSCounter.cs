using TMPro;
using UnityEngine;

public class FPSCounter:MonoBehaviour
{
	TextMeshProUGUI fpsCounterText;
	int prevFPS;

	private void Start()
	{
		fpsCounterText = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		int avgFrameRate = Mathf.CeilToInt(1f / Time.unscaledDeltaTime);
		if (prevFPS == avgFrameRate)
			fpsCounterText.text = avgFrameRate.ToString() + " FPS";
		prevFPS = avgFrameRate;
	}
}
