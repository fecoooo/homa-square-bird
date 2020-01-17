using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler:MonoBehaviour
{
	TextMeshProUGUI scoreLbl;

	Image progressBarImg;
	TextMeshProUGUI progressLbl;

	TextMeshProUGUI currentStageLbl;
	TextMeshProUGUI nextStageLbl;

	void Start()
    {
		scoreLbl = transform.Find("ScoreLbl").GetComponent<TextMeshProUGUI>();

		progressBarImg = transform.Find("Progress/ProgressBarImg").GetComponent<Image>();
		progressLbl = transform.Find("Progress/ProgressLbl").GetComponent<TextMeshProUGUI>();

		currentStageLbl = transform.Find("Progress/CurrentStageLbl").GetComponent<TextMeshProUGUI>();
		nextStageLbl = transform.Find("Progress/NextStageLbl").GetComponent<TextMeshProUGUI>();

		GameHandler.instance.GameStateChanged += OnGameStateChanged;
	}

	void Update()
    {
		if(GameHandler.instance.CurrentState == GameState.InGame)
			UpdateProgress();
    }

	void UpdateProgress()
	{
		progressBarImg.transform.localScale = new Vector3(GameHandler.instance.Progress, 1, 1);
		progressLbl.text = Mathf.CeilToInt(GameHandler.instance.Progress * 100) + "%";
	}

	void OnGameStateChanged(GameState state)
	{
		switch (state)
		{
			case GameState.Startup:
				InitLevel();
				break;
			case GameState.BeforeGame:
				InitLevel();
				break;
			default:
				break;
		}
	}

	void InitLevel()
	{
		scoreLbl.text = "0";

		progressBarImg.transform.localScale = new Vector3(0, 1, 1);
		progressLbl.text = "0%";

		currentStageLbl.text = GameHandler.instance.CurrentLevel.ToString() ;
		nextStageLbl.text = (GameHandler.instance.CurrentLevel + 1).ToString();
	}
}
