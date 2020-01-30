using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler:MonoBehaviourSingleton<GameHandler>
{
	public delegate void GameStateChangeHandler(GameState state);
	public event GameStateChangeHandler GameStateChanged;

	public delegate void ScoreChangeHandler(int currentScore);
	public event ScoreChangeHandler ScoreChanged;

	public delegate void ConsecutiveScoreChangeHandler(int consecutiveScores);
	public event ConsecutiveScoreChangeHandler ConsecutiveScoreChanged;

	public GameState CurrentState { get; private set; }

	int score = 0;
	int consecutiveScore = -1;

	List<GameObject> scoreBlocks = new List<GameObject>();

	public float Progress
	{
		get => Mathf.Clamp01(Character.instance.transform.position.x / Map.instance.Width);
	}

	public int CurrentLevel { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
		}
		else
		{
			//Handle FB.Init
			FB.Init(() =>
			{
				FB.ActivateApp();
			});
		}
	}

	private void Start()
	{
		PlayerPrefs.DeleteKey("CurrentLevel");

		CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);

		ScoreChanged += OnScoreChanged;
		ConsecutiveScoreChanged += OnConsecutiveScoreChanged;

		GameStateChanged += OnGameStateChanged;
		GameStateChanged(GameState.BeforeGame);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
			OnClick();

		if (CurrentState == GameState.BeforeGame && Character.instance.Ready && Map.instance.Ready)
			GameStateChanged(GameState.FadeIn);
	}

	private void OnClick()
	{
		switch (CurrentState)
		{
			case GameState.WaitingForInput:
				GameStateChanged(GameState.InGame);
				break;
			case GameState.GameWon:
				GameStateChanged(GameState.FadeOut);
				break;
			case GameState.GameLost:
				GameStateChanged(GameState.FadeOut);
				break;

			default:
				break;
		}
	}

	void OnApplicationPause(bool pauseStatus)
	{
		// Check the pauseStatus to see if we are in the foreground
		// or background
		if (!pauseStatus)
		{
			//app resume
			if (FB.IsInitialized)
			{
				FB.ActivateApp();
			}
			else
			{
				//Handle FB.Init
				FB.Init(() => {
					FB.ActivateApp();
				});
			}
		}
	}

	private void OnGameStateChanged(GameState state)
	{
		CurrentState = state;

		switch (CurrentState)
		{
			case GameState.BeforeGame:
				scoreBlocks.Clear();
				consecutiveScore = -1;
				score = 0;
				break;
			case GameState.GameWon:
				CurrentLevel++;
				if (CurrentLevel > 9)
					CurrentLevel = 0;
				PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
				PlayerPrefs.Save();
				break;
			default:
				break;
		}
	}

	void OnScoreChanged(int currentScore)
	{
		//dummy
	}

	void OnConsecutiveScoreChanged(int consecutiveScore)
	{
		//dummy
	}

	public void TriggerGameLost()
	{
		GameStateChanged(GameState.GameLost);
	}

	public void TriggerGameWon()
	{
		GameStateChanged(GameState.GameWon);
	}

	internal void AddWithScore(GameObject scoreBlock)
	{
		if (!scoreBlocks.Contains(scoreBlock))
		{
			scoreBlocks.Add(scoreBlock);
			score += GamePreferences.instance.scoreStep;
			Taptic.Failure();
			if (!Character.instance.IsShooting)
			{
				consecutiveScore = (consecutiveScore + 1) % 3;
				ConsecutiveScoreChanged(consecutiveScore);
			}
			ScoreChanged(score);
		}
	}

	public void Vibrate()
	{
		Vibration.Vibrate();
	}

	public void OnFadeOutCompleted()
	{
		GameStateChanged(GameState.BeforeGame);
	}

	internal void OnFadeInCompleted()
	{
		GameStateChanged(GameState.WaitingForInput);
	}
}
public enum GameState
{
	BeforeGame,
	WaitingForInput,
	InGame,
	GameWon,
	GameLost,
	FadeOut,
	FadeIn,
}