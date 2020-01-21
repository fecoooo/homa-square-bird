using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler:MonoBehaviourSingleton<GameHandler>
{
	public delegate void GameStateChangeHandler(GameState state);
	public event GameStateChangeHandler GameStateChanged;

	public GameState CurrentState { get; private set; }

	public float Progress
	{
		get => Mathf.Clamp01(Character.instance.transform.position.x / Map.instance.Width);
	}

	public int CurrentLevel { get; private set; }

	private void Start()
	{
		PlayerPrefs.DeleteKey("CurrentLevel");

		CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);

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
			case GameState.InGame:
				break;
			case GameState.GameWon:
				CurrentLevel++;
				PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
				PlayerPrefs.Save();
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
	}

	public void TriggerGameLost()
	{
		GameStateChanged(GameState.GameLost);
	}

	public void TriggerGameWon()
	{
		GameStateChanged(GameState.GameWon);
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