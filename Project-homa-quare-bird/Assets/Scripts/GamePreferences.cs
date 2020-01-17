using UnityEngine;

public class GamePreferences: MonoBehaviour
{
	public GamePreferencesScriptableObject gamePreferences;

	public static GamePreferencesScriptableObject instance { get; private set; }

	private void Awake()
	{
		instance = gamePreferences;
	}
}
