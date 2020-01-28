using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : PhysicalObject
{
	public int jumpFrames;
	public float jumpHeight = 1f;

	public AnimationClip HitAnim;

	const float ShootDistance = 4f;
	const float ShootTime = 5f;
	float currentShootTime = 0f;

	int currentJumpFrame = int.MaxValue;
	float jumpStep;
	Vector3 eggSpawnPositionYZ;

	Vector3 spawnPosition;

	const float RotateTowardsCameraAnimTime = .3f;

	List<GameObject> allEggs = new List<GameObject>();

	LineRenderer laser;
	Animator animator;

	Transform cat;
	IEnumerator WaitFallThanRotateTowardsCamera_IEnum;

	IEnumerator DelayedOnDestroyObject_IEnum;

	protected bool Grounded
	{
		get
		{
			return 
			(verticalHitinfos.Item1.collider != null && verticalHitinfos.Item1.collider.tag != "Egg") ||
			(verticalHitinfos.Item2.collider != null && verticalHitinfos.Item2.collider.tag != "Egg");
		}
	}

	protected bool Falling
	{
		get => verticalHitinfos.Item1.collider == null && verticalHitinfos.Item2.collider == null;
	}

	protected Vector3 TopForwardPoint
	{
		get => MiddleFrontPoint + new Vector3(0, collider.bounds.extents.y + GamePreferences.instance.gravityPerFrame, 0);
	}

	public bool IsShooting { get => laser.enabled; }

	public bool Ready { get; private set; }

	const float TimeInAnimStateThreshold = .15f;
	float timeinThisAnimState;

	protected override void OnStart()
	{
		base.OnStart();
		cat = transform.Find("Cat");

		animator = transform.GetComponentInChildren<Animator>();

		laser = GetComponent<LineRenderer>();

		spawnPosition = transform.position;

		jumpStep = jumpHeight / jumpFrames;

		GameHandler.instance.GameStateChanged += OnGameStateChanged;
		GameHandler.instance.ConsecutiveScoreChanged += OnConsecutiveScoreChanged;
	}

	private void Update()
	{
		if (GameHandler.instance.CurrentState != GameState.InGame)
			return;

		if (Input.GetMouseButtonDown(0) && CanJump())
		{
			eggSpawnPositionYZ = transform.position;
			currentJumpFrame = 0;
		}

		if (currentShootTime > 0)
		{
			laser.SetPosition(0, transform.position);
			laser.SetPosition(1, transform.position + new Vector3(ShootDistance, 0, 0));
			currentShootTime -= Time.deltaTime;
			Map.instance.DestroyBlocks(Physics.RaycastAll(collider.bounds.center, new Vector3(1, 0, 0), ShootDistance));
			//Debug.DrawLine(collider.bounds.center, collider.bounds.center + new Vector3(ShootDistance, 0, 0), Color.yellow);
		}
		else if (laser.enabled)
			laser.enabled = false;

		timeinThisAnimState += Time.deltaTime;

		if(timeinThisAnimState > TimeInAnimStateThreshold)
		{
			if (Grounded || Falling)
			{
				timeinThisAnimState = 0;
				animator.SetInteger("State", (int)AnimStates.Run);
			}
			else
			{
				timeinThisAnimState = 0;
				animator.SetInteger("State", (int)AnimStates.Idle);
			}
		}
	}

	private bool CanJump()
	{
		return !Physics.Raycast(transform.position, Vector3.up, 1f);
	}

	void StartShooting()
	{
		laser.enabled = true;
		currentShootTime = ShootTime;
	}

	protected override void SetShouldUpdate()
	{
		shouldUpdate = GameHandler.instance.CurrentState == GameState.InGame || GameHandler.instance.CurrentState == GameState.GameWon;
	}

	protected override void SetHorizontalMovement()
	{
		if (!frontCollision)
		{
			RaycastHit hitInfo;

			frontCollision = Physics.Raycast(TopForwardPoint, new Vector3(1, 0, 0), out hitInfo, GamePreferences.instance.forwardCheckDistance);
			if (!frontCollision)
				frontCollision = Physics.Raycast(BottomForwardPoint, new Vector3(1, 0, 0), out hitInfo, GamePreferences.instance.forwardCheckDistance);

			if (frontCollision)
			{
				if (hitInfo.collider.name == "WinCollider")
				{
					winColliderHit = true;
					foreach (GameObject egg in allEggs)
						egg.GetComponent<PhysicalObject>().DestroyObject(0);
					GameHandler.instance.TriggerGameWon();
				}
				else if (hitInfo.collider.name == "TopCollider")
				{
					//Do nothings
				}
				else
					GameHandler.instance.TriggerGameLost();
			}
		}

		nextMovePos.x = frontCollision ? transform.position.x : transform.position.x + GamePreferences.instance.speed;
	}

	public override void DestroyObject(int framesToWait = 2)
	{
		if (winColliderHit)
			return;

		laser.enabled = false;
		GetComponent<ParticleSystem>().Play();
		animator.SetInteger("State", (int)AnimStates.Hit);
		DelayedOnDestroyObject_IEnum = DelayedOnDestroyObject();
		StartCoroutine(DelayedOnDestroyObject_IEnum);
	}

	IEnumerator DelayedOnDestroyObject()
	{
		yield return new WaitForSeconds(HitAnim.length / 2);
		cat.gameObject.SetActive(false);
	}

	protected override Tuple<RaycastHit, RaycastHit> SetVerticalMovement()
	{
		if (currentJumpFrame <= jumpFrames)
		{
			nextMovePos.y = transform.position.y + jumpStep;
			if (currentJumpFrame == jumpFrames)
				SpawnEgg();
			currentJumpFrame++;

			return new Tuple<RaycastHit, RaycastHit>(new RaycastHit(), new RaycastHit());
		}
		else
		{
			Tuple <RaycastHit, RaycastHit> hitInfos = base.SetVerticalMovement();

			if (hitInfos.Item1.collider != null && (hitInfos.Item1.collider.gameObject.tag == "Score" || hitInfos.Item1.collider.gameObject.tag == "UnbreakableScore"))
				GameHandler.instance.AddWithScore(hitInfos.Item1.collider.gameObject);

			if (hitInfos.Item2.collider != null && (hitInfos.Item2.collider.gameObject.tag == "Score" || hitInfos.Item2.collider.gameObject.tag == "UnbreakableScore"))
				GameHandler.instance.AddWithScore(hitInfos.Item2.collider.gameObject);

			return hitInfos;
		}
	}

	void ResetPosition()
	{
		transform.position = spawnPosition;
	}

	void OnGameStateChanged(GameState state)
	{
		switch (state)
		{
			case GameState.BeforeGame:
				winColliderHit = false;
				cat.gameObject.SetActive(true);

				if (DelayedOnDestroyObject_IEnum != null)
					StopCoroutine(DelayedOnDestroyObject_IEnum);

				if (WaitFallThanRotateTowardsCamera_IEnum != null)
					StopCoroutine(WaitFallThanRotateTowardsCamera_IEnum);

				animator.enabled = true;

				animator.SetInteger("State", (int)AnimStates.Idle);
				cat.rotation = Quaternion.Euler(0, 100, 0);

				ClearEggs();
				ResetPosition();
				ResetProperies();
				currentJumpFrame = int.MaxValue;
				currentShootTime = 0;
				laser.enabled = false;
				Ready = true;
				break;
			case GameState.InGame:
				break;
			case GameState.GameWon:
				animator.SetInteger("State", (int)AnimStates.Idle);

				WaitFallThanRotateTowardsCamera_IEnum = WaitFallThanRotateTowardsCamera();
				StartCoroutine(WaitFallThanRotateTowardsCamera_IEnum);

				currentJumpFrame = int.MaxValue;

				laser.enabled = false;
				Ready = false;
				break;
			case GameState.GameLost:
				Ready = false;
				break;
			default:
				break;
		}
	}

	

	void OnConsecutiveScoreChanged(int consecutiveScore)
	{
		if (consecutiveScore == 2)
			StartShooting();
	}


	IEnumerator WaitFallThanRotateTowardsCamera()
	{
		while (!Grounded)
			yield return null;

		float timePassed = 0;
		while(timePassed < RotateTowardsCameraAnimTime)
		{
			float t = timePassed / RotateTowardsCameraAnimTime;
			cat.rotation = Quaternion.Euler(0, 90 + t * 90, 0);
			timePassed += Time.deltaTime;

			yield return null;
		}

		cat.rotation = Quaternion.Euler(0, 180, 0);

		animator.SetInteger("State", (int)AnimStates.Jump);

	}

	public void SpawnEgg()
	{
		//animator.SetTrigger("Jump");

		GameObject egg = Instantiate(GamePreferences.instance.egg);
		float spawnX = transform.position.x;
		egg.transform.position = new Vector3(spawnX, eggSpawnPositionYZ.y, eggSpawnPositionYZ.z);

		allEggs.Add(egg);
	}

	void ClearEggs()
	{
		foreach (GameObject go in allEggs)
			Destroy(go);
		allEggs.Clear();
	}

	protected override void DebugDraw()
	{
		base.DebugDraw();
		Debug.DrawLine(TopForwardPoint, TopForwardPoint + new Vector3(1, 0, 0), Color.magenta);
	}


	public static Character instance;
	bool winColliderHit;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogError($"Multiple instances of singleton class: {typeof(Character)}\n");
			Debug.Break();
		}
		instance = this;
	}

	enum AnimStates
	{
		Idle,
		Idle2,
		Walk,
		Run,
		Jump,
		Hit,
	}
}
