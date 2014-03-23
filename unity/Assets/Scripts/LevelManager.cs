using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
	public GameObject gameFinishedObject;
	public GameObject circlePrefab;
	public float minX, maxX, minY, maxY;

	public TextMesh textLevelNum;

	public GameManager gameManager;

	public float transitionHalfTime = 0.6f;

	private string[] levels = new string[]
	{
		/* 0 */ "0.18,0.32,5.00,M,35.00,0.00,0.18,0.72,5.00,M,35.00,0.00,",
		/* 8 */ "0.57,0.53,4.00,M,-10.00,0.00,0.40,0.53,4.00,M,10.00,0.00,0.72,0.53,4.00,M,-10.00,0.00,",
		/* 1 */ "0.18,0.20,5.00,M,35.00,0.00,0.18,0.81,5.00,M,35.00,0.00,0.49,0.20,5.00,M,0.00,-20.00,0.76,0.81,5.00,M,0.00,20.00,",
		/* 4 */ "0.88,0.16,5.00,M,-30.00,20.00,0.89,0.84,5.00,M,-30.00,-20.00,0.12,0.17,5.00,M,30.00,20.00,0.14,0.81,5.00,M,30.00,-20.00,",
		/* 7 */ "0.57,0.53,4.00,M,-5.00,0.00,0.40,0.53,4.00,M,40.00,0.00,",
		/* 2.1 */ "0.40,0.39,4.00,M,15.00,15.00,0.48,0.61,4.00,M,0.00,-25.00,0.56,0.40,4.00,M,-15.00,15.00,",
		/* 3 */ "0.52,0.80,3.50,M,20.00,-15.00,0.65,0.65,3.50,M,0.00,-25.00,0.66,0.41,3.50,M,-15.00,-10.00,0.52,0.27,3.50,M,-15.00,10.00,0.38,0.41,3.50,M,0.00,25.00,0.39,0.65,3.50,M,15.00,10.00,",
		/* 6 */ "0.60,0.53,9.90,M,-5.00,0.00,0.35,0.68,4.00,M,5.00,0.00,0.35,0.38,4.00,M,5.00,0.00,",
		/* 5 */ "0.88,0.16,5.00,M,0.00,60.00,0.87,0.82,5.00,M,-60.00,0.00,0.12,0.17,5.00,M,60.00,0.00,0.14,0.81,5.00,M,0.00,-60.00,0.63,0.16,5.00,M,0.00,-20.00,0.38,0.17,5.00,M,0.00,-20.00,0.37,0.81,5.00,M,0.00,20.00,0.63,0.83,5.00,M,0.00,20.00,",
		/* 10 */ "0.13,0.13,4.00,M,80.00,0.00,0.13,0.87,4.00,M,80.00,0.00,0.13,0.63,4.00,M,80.00,0.00,0.13,0.39,4.00,M,80.00,0.00,0.34,0.77,2.00,M,0.00,80.00,0.34,0.56,2.00,M,0.00,80.00,0.34,0.36,2.00,M,0.00,80.00,0.34,0.12,2.00,M,0.00,80.00,",
		/* 9 */ "0.88,0.50,5.00,M,0.00,60.00,0.87,0.82,5.00,M,-60.00,0.00,0.15,0.17,5.00,M,60.00,0.00,0.14,0.81,5.00,M,0.00,-60.00,0.63,0.50,5.00,M,0.00,60.00,0.38,0.17,5.00,M,0.00,60.00,0.38,0.50,5.00,M,60.00,0.00,0.63,0.83,5.00,M,0.00,-60.00,"
		
		
		//* 2 */ "0.39,0.38,5.00,M,15.00,15.00,0.48,0.66,5.00,M,0.00,-25.00,0.59,0.40,5.00,M,-15.00,15.00,",
		// original test level:
		//"0.19,0.82,3.00,M,-20.00,15.00,0.32,0.36,5.00,M,5.00,15.00,0.28,0.62,3.00,M,15.00,10.00,0.64,0.62,3.00,S,"
	};

	private int currentLevel;


	void Awake()
	{
	}

	void OnEnable()
	{
		Init();
	}

	public void Init()
	{
		Debug.Log("before clear");
		ClearLevel();
		Debug.Log("before sitch to level");
		SwitchToLevel(0);
		Debug.Log("after");

		gameFinishedObject.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			PrintCurrentLevel();
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;

		Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, this.transform.position.z);
		Vector3 size = new Vector3(minX - maxX, minY - maxY, 1.0f);

		Gizmos.DrawWireCube(center, size);
	}

	private void SwitchToLevel(int numLevel)
	{
		GameObject[] circles = GameObject.FindGameObjectsWithTag("Circle");

		gameManager.gameState = GameState.Transitioning;

		if (circles.Length == 0)
		{
			FadeInLevel(numLevel);
		}
		else
		{
			TweenFlow fadeOutFlow = new TweenFlow();
		
			foreach (GameObject circle in circles)
			{
				Tween fadeOutTween = new Tween(circle.transform, transitionHalfTime, new TweenConfig().scale(0).setEaseType(EaseType.Linear));
				//fadeOutTween.setOnCompleteHandler((a) => { Go.killAllTweensWithTarget(circle.transform); Destroy(circle); });
				fadeOutFlow.insert(0, fadeOutTween);
			}

			fadeOutFlow.setOnCompleteHandler((a) =>
				{
					FadeInLevel(numLevel);
				}
			);

			fadeOutFlow.play();
		}

	}

	private void FadeInLevel(int numLevel)
	{
		gameManager.currentTime_s = 0.0f;
		gameManager.AddTime(0);

		textLevelNum.text = "" + currentLevel;

		ClearLevel();

		LoadLevel(numLevel);

		TweenFlow fadeInFlow = new TweenFlow();

		foreach (GameObject circle in GameObject.FindGameObjectsWithTag("Circle"))
		{
			float origScale = circle.transform.localScale.x;
			circle.transform.localScale = Vector3.zero;

			Tween fadeInTween = new Tween(circle.transform, transitionHalfTime, new TweenConfig().scale(origScale).setEaseType(EaseType.QuadIn));

			fadeInFlow.insert(0, fadeInTween);
		}

		fadeInFlow.setOnCompleteHandler((aa) => { gameManager.gameState = GameState.Playing; });

		fadeInFlow.play();
	}

	private void ClearLevel()
	{
		foreach (GameObject circle in GameObject.FindGameObjectsWithTag("Circle"))
		{
			Destroy(circle);
		}
	}

	private void LoadLevel(int numLevel)
	{
		currentLevel = numLevel;

		string[] tokens = levels[numLevel].Split(',');

		int i = 0;

		while (i < tokens.Length)
		{
			if (tokens[i] == null || tokens[i] == "")
			{
				break;
			}

			float normalizedX = float.Parse(tokens[i++]);
			float normalizedY = float.Parse(tokens[i++]);
			float scale = float.Parse(tokens[i++]);

			GameObject circle = GameObject.Instantiate(circlePrefab) as GameObject;
			circle.transform.position = new Vector3(minX + normalizedX * (maxX - minX), minY + normalizedY * (maxY - minY), this.transform.position.z);
			circle.transform.localScale = Vector3.one * scale;
			circle.transform.parent = this.transform;
			circle.name = "Circle";

			CircleController cc = circle.GetComponent<CircleController>();

			string type = tokens[i++];

			if (type == "M")
			{
				cc.circleType = CircleType.Movable;

				float velocityX = float.Parse(tokens[i++]);
				float velocityY = float.Parse(tokens[i++]);

				cc.velocity = new Vector2(velocityX, velocityY);
			}
			else if (type == "S")
			{
				cc.circleType = CircleType.Static;
			}
			else
			{
				throw new System.Exception("Corrupt level");
			}
		}
	}

	public void LevelEnded()
	{
		currentLevel++;

		PauseAllCircles();

		gameManager.gameState = GameState.Transitioning;

		ExecuteDelayedAction(1.0f, () =>
			{
				if (currentLevel == levels.Length)
				{
					gameManager.gameState = GameState.Ended;


					TweenFlow fadeOutFlow = new TweenFlow();

					foreach (GameObject circle in GameObject.FindGameObjectsWithTag("Circle"))
					{
						Tween fadeOutTween = new Tween(circle.transform, transitionHalfTime, new TweenConfig().scale(0).setEaseType(EaseType.Linear));
						fadeOutFlow.insert(0, fadeOutTween);
					}

					fadeOutFlow.setOnCompleteHandler((a) =>
					{
						ClearLevel();

						TweenFlow fadeInFlow = new TweenFlow();

						gameFinishedObject.SetActive(true);
						Vector3 origScale = gameFinishedObject.transform.localScale;

						gameFinishedObject.transform.localScale = Vector3.zero;

						Tween fadeInTween = new Tween(gameFinishedObject.transform, transitionHalfTime, new TweenConfig().scale(origScale).setEaseType(EaseType.QuadIn));

						fadeInFlow.insert(0, fadeInTween);

						fadeInFlow.setOnCompleteHandler((aaa) =>
						{
							ExecuteDelayedAction(2.0f, () =>
							{
								GameObject.Find("PageManager").GetComponent<PageManager>().GoToPage(GamePage.MainMenu);
							});
						});

						fadeInFlow.play();
					}
					);

					gameManager.gameState = GameState.Transitioning;
					fadeOutFlow.play();
				}
				else
				{
					SwitchToLevel(currentLevel);
				}
			}
		);

	}

	private void PauseAllCircles()
	{
		foreach (GameObject circle in GameObject.FindGameObjectsWithTag("Circle"))
		{
			CircleController cc = circle.GetComponent<CircleController>();

			if (cc.circleType == CircleType.Movable)
			{
				cc.circleState = CircleState.Paused;
			}
		}
	}

	private void PrintCurrentLevel()
	{
		GameObject[] circles = GameObject.FindGameObjectsWithTag("Circle");

		string level = "(";

		foreach (GameObject circle in circles)
		{
			CircleController cc = circle.GetComponent<CircleController>();

			float normalizedX = (circle.transform.position.x - minX) / (maxX - minX);
			float normalizedY = (circle.transform.position.y - minY) / (maxY - minY);

			level += normalizedX.ToString("F") + "," + normalizedY.ToString("F") + ",";

			level += circle.transform.localScale.x.ToString("F") + ",";

			switch (cc.circleType)
			{
				case CircleType.Static:
					level += "S,";
					break;

				case CircleType.Movable:
					level += "M,";

					level += cc.velocity.x.ToString("F") + "," + cc.velocity.y.ToString("F") + ",";

					break;
			}
		}

		level += ")";

		Debug.Log("LEVEL: " + level);
	}

	public void ExecuteDelayedAction(float delay, System.Action action)
	{
		StartCoroutine(ExecuteDelayedAction_Coroutine(delay, action));
	}

	private IEnumerator ExecuteDelayedAction_Coroutine(float delay, System.Action action)
	{
		yield return new WaitForSeconds(delay);

		action.Invoke();
	}
}
