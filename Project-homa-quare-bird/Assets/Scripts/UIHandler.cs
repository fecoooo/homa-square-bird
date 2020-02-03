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

	Image fadeImg;

	IEnumerator gratitudeRoutine;
	IEnumerator scoreFlashRoutineIEnum;

	const float gratitudeAnimationTime = 1f;
	const float scaleUpAnimModifier = 4f;
	const float fadeAnimTime = .3f;
	const float scoreFlashAnimTime = .3f;

	Transform shootProgress;
	Image shootProgressImg;

	readonly string[] gratitudes = { "Superb!", "Magnific!", "Impressive!" };

	readonly Color[] gratitudeColors = { Color.yellow, new Color(1f, 0.4f, 0), Color.red };

	readonly Vector3 ShootProgressOffest = new Vector3(0, 150f, 0);

	void Start()
	{
		scoreLbl = transform.Find("ScoreLbl").GetComponent<TextMeshProUGUI>();

		progressBarImg = transform.Find("Progress/ProgressBarImg").GetComponent<Image>();
		progressLbl = transform.Find("Progress/ProgressLbl").GetComponent<TextMeshProUGUI>();

		currentStageLbl = transform.Find("Progress/CurrentStageLbl").GetComponent<TextMeshProUGUI>();
		nextStageLbl = transform.Find("Progress/NextStageLbl").GetComponent<TextMeshProUGUI>();

		middleMsgLbl = transform.Find("MiddleMsgLbl").GetComponent<TextMeshProUGUI>();
		bottomMsgLbl = transform.Find("BottomMsgLbl").GetComponent<TextMeshProUGUI>();

		fadeImg = transform.Find("FadeImg").GetComponent<Image>();

		shootProgress = transform.Find("ShootProgress");
		shootProgressImg = transform.Find("ShootProgress/ProgressBarImg").GetComponent<Image>();

		GameHandler.instance.GameStateChanged += OnGameStateChanged;
		GameHandler.instance.ScoreChanged += OnScoreChanged;
		GameHandler.instance.ConsecutiveScoreChanged += OnConsecutiveScoreChanged;

	}

	private void LateUpdate()
	{
		if (Character.instance.IsShooting != shootProgress.gameObject.activeSelf)
			shootProgress.gameObject.SetActive(Character.instance.IsShooting);

		if (shootProgress.gameObject.activeSelf)
		{
			shootProgress.position = Camera.main.WorldToScreenPoint(Character.instance.transform.position) + ShootProgressOffest;
			shootProgressImg.fillAmount = Character.instance.ShootProgress;
		}
	}

	void Update()
	{

		if (GameHandler.instance.CurrentState == GameState.InGame)
			UpdateProgress();
	}

	void UpdateProgress()
	{
		progressBarImg.fillAmount = GameHandler.instance.Progress;// transform.localScale = new Vector3(GameHandler.instance.Progress, 1, 1);
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
			case GameState.FadeOut:
				StartCoroutine(FadeOut());
				break;
			case GameState.FadeIn:
				StartCoroutine(FadeIn());
				break;
			default:
				break;
		}
	}

	void OnScoreChanged(int currentScore)
	{
		scoreLbl.text = currentScore.ToString();
		if (scoreFlashRoutineIEnum != null)
			StopCoroutine(scoreFlashRoutineIEnum);

		scoreFlashRoutineIEnum = ScoreFlashRoutine();
		StartCoroutine(scoreFlashRoutineIEnum);
	}

	IEnumerator ScoreFlashRoutine()
	{
		Color newColor = Color.white;

		float timePassed = 0;
		while (timePassed < scoreFlashAnimTime)
		{
			float t = (timePassed / scoreFlashAnimTime);
			newColor = Color.Lerp(Color.yellow, Color.white, t);
			newColor.a = t;
			scoreLbl.color = newColor;

			float currentScale = Mathf.Lerp(3f, 1f, t);
			scoreLbl.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

			timePassed += Time.deltaTime;
			yield return null;
		}

		newColor.a = 1;
		scoreLbl.color = newColor;
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
		scoreLbl.transform.localScale = Vector3.one;
		scoreLbl.color = new Color(1, 1, 1, 1);

		progressBarImg.fillAmount = 0;//.transform.localScale = new Vector3(0, 1, 1);
		progressLbl.text = "0%";

		currentStageLbl.text = (GameHandler.instance.CurrentLevel + 1).ToString();
		nextStageLbl.text = (GameHandler.instance.CurrentLevel + 2).ToString();

		middleMsgLbl.color = Color.white;
		middleMsgLbl.gameObject.SetActive(true);
		middleMsgLbl.text = "Tap to start";

		bottomMsgLbl.gameObject.SetActive(false);
	}

	IEnumerator FadeIn()
	{
		Color newColor = Color.white;

		float timePassed = 0;
		while (timePassed < fadeAnimTime)
		{
			float t = (timePassed / fadeAnimTime);
			newColor.a = 1 - t;
			fadeImg.color = newColor;

			timePassed += Time.deltaTime;
			yield return null;
		}

		newColor.a = 0;
		fadeImg.color = newColor;
		GameHandler.instance.OnFadeInCompleted();
	}

	IEnumerator FadeOut()
	{
		Color newColor = Color.white;

		float timePassed = 0;
		while (timePassed < fadeAnimTime)
		{
			float t = (timePassed / fadeAnimTime);
			newColor.a = t;
			fadeImg.color = newColor;

			timePassed += Time.deltaTime;
			yield return null;
		}

		fadeImg.color = Color.white;
		GameHandler.instance.OnFadeOutCompleted();
	}
}