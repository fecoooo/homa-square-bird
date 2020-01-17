using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviourSingleton<GameHandler>
{
	public delegate void GameStateChangeHandler(GameState state);
	public event GameStateChangeHandler GameStateChanged;

	public GameState CurrentState { get; private set; }

	public float Progress
	{
		get =>	Mathf.Clamp01(Character.instance.transform.position.x / Map.instance.Width);
	}

	public int CurrentLevel { get; private set; }

	private void Start()
	{
		CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);

		GameStateChanged += OnGameStateChanged;
		GameStateChanged(GameState.Startup);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
			GameStateChanged(GameState.InGame);
	}

	private void OnGameStateChanged(GameState state)
	{
		CurrentState = state;
	}

}
public enum GameState
{
	Startup,
	BeforeGame,
	InGame,
}
