using UnityEngine;

[CreateAssetMenu(fileName = "GamePreferences", menuName = "ScriptableObjects/GamePreferences", order = 1)]
public class GamePreferencesScriptableObject: ScriptableObject
{
	public GameObject egg;
	public GameObject DirtBlock;
	public GameObject GrassyDirtBlock;
	public float gravityPerFrame = -0.1f;
}
