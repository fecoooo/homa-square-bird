using UnityEngine;

[CreateAssetMenu(fileName = "GamePreferences", menuName = "ScriptableObjects/GamePreferences", order = 1)]
public class GamePreferencesScriptableObject: ScriptableObject
{
	public GameObject Egg;
	public Material[] EggMaterials;
	public GameObject DirtBlock;
	public GameObject TopDirtBlock;
	public GameObject GrassyDirtBlock;
	public GameObject GrassyDirtBlockWithScore;
	public GameObject StoneBlock;
	public GameObject GrassyStoneBlockWithScore;
	public float gravityPerFrame = -0.1f;

	public float speed = .1f;
	public float bottomCheckDistance = .15f;
	public float forwardCheckDistance = .15f;
	public float collisionErrorCorrection = .01f;

	public int scoreStep = 5;
}
