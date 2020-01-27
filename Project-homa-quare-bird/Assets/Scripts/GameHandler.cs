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
			GameStateChanged(GameState.WaitingForInput);
	}

	private void OnClick()
	{
		switch (CurrentState)
		{
			case GameState.WaitingForInput:
				GameStateChanged(GameState.InGame);
				break;
			case GameState.GameWon:
				GameStateChanged(GameState.BeforeGame);
				break;
			case GameState.GameLost:
				GameStateChanged(GameState.BeforeGame);
				break;
			default:
				break;
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
			if (!Character.instance.IsShooting)
			{
				consecutiveScore = (consecutiveScore + 1) % 3;
				ConsecutiveScoreChanged(consecutiveScore);
			}
			ScoreChanged(score);
		}
	}
}
public enum GameState
{
	BeforeGame,
	WaitingForInput,
	InGame,
	GameWon,
	GameLost,
}