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

	TextMeshProUGUI middleMsgLbl;
	TextMeshProUGUI bottomMsgLbl;

	IEnumerator gratitudeRoutine;

	const float gratitudeAnimationTime = 1f;
	const float scaleUpAnimModifier = 4f;

	readonly string[] gratitudes = { "Superb!", "Magnific!", "Impressive!" };

	readonly Color[] gratitudeColors = { Color.yellow, new Color(1f, 0.4f, 0), Color.red };

	void Start()
	{
		scoreLbl = transform.Find("ScoreLbl").GetComponent<TextMeshProUGUI>();

		progressBarImg = transform.Find("Progress/ProgressBarImg").GetComponent<Image>();
		progressLbl = transform.Find("Progress/ProgressLbl").GetComponent<TextMeshProUGUI>();

		currentStageLbl = transform.Find("Progress/CurrentStageLbl").GetComponent<TextMeshProUGUI>();
		nextStageLbl = transform.Find("Progress/NextStageLbl").GetComponent<TextMeshProUGUI>();

		middleMsgLbl = transform.Find("MiddleMsgLbl").GetComponent<TextMeshProUGUI>();
		bottomMsgLbl = transform.Find("BottomMsgLbl").GetComponent<TextMeshProUGUI>();

		GameHandler.instance.GameStateChanged += OnGameStateChanged;
		GameHandler.instance.ScoreChanged += OnScoreChanged;
		GameHandler.instance.ConsecutiveScoreChanged += OnConsecutiveScoreChanged;

	}

	void Update()
	{
		if (GameHandler.instance.CurrentState == GameState.InGame)
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
			case GameState.BeforeGame:
				InitLevel();
				break;
			case GameState.InGame:
				middleMsgLbl.gameObject.SetActive(false);
				break;
			case GameState.GameWon:
				middleMsgLbl.color = Color.white;
				middleMsgLbl.gameObject.SetActive(true);
				middleMsgLbl.text = "Level Complete";
				bottomMsgLbl.gameObject.SetActive(true);
				bottomMsgLbl.text = "Tap for next level";
				break;
			case GameState.GameLost:
				middleMsgLbl.color = Color.white;
				middleMsgLbl.gameObject.SetActive(true);
				middleMsgLbl.text = "Game Over";
				bottomMsgLbl.gameObject.SetActive(true);
				bottomMsgLbl.text = "Tap to restart";
				break;
			default:
				break;
		}
	}

	void OnScoreChanged(int currentScore)
	{
		scoreLbl.text = currentScore.ToString();
	}

	void OnConsecutiveScoreChanged(int consecutiveScore)
	{
		if (gratitudeRoutine != null)
			StopCoroutine(gratitudeRoutine);

		gratitudeRoutine = GratitudeOnScore(consecutiveScore);
		StartCoroutine(gratitudeRoutine);
	}

	IEnumerator GratitudeOnScore(int consecutiveScore)
	{
		middleMsgLbl.gameObject.SetActive(true);
		middleMsgLbl.text = gratitudes[consecutiveScore];
		middleMsgLbl.color = gratitudeColors[consecutiveScore];
		middleMsgLbl.transform.localScale = Vector3.zero;

		float timePassed = 0;
		while (timePassed < gratitudeAnimationTime)
		{
			float t = Mathf.Clamp01((timePassed / gratitudeAnimationTime) * scaleUpAnimModifier);
			middleMsgLbl.transform.localScale = new Vector3(t, t, t);

			timePassed += Time.deltaTime;
			yield return null;
		}

		middleMsgLbl.gameObject.SetActive(false);
	}

	void InitLevel()
	{
		scoreLbl.text = "0";

		progressBarImg.transform.localScale = new Vector3(0, 1, 1);
		progressLbl.text = "0%";

		currentStageLbl.text = (GameHandler.instance.CurrentLevel + 1).ToString();
		nextStageLbl.text = (GameHandler.instance.CurrentLevel + 2).ToString();

		middleMsgLbl.color = Color.white;
		middleMsgLbl.gameObject.SetActive(true);
		middleMsgLbl.text = "Tap to start";

		bottomMsgLbl.gameObject.SetActive(false);
	}
}