using UnityEngine;
using System.Collections;

public class Instructions : MonoBehaviour
{
	public GameObject mouse;
	public GameObject circle;
	public GameObject circleButton;
	public Material mouseNormal;
	public Material mouseRight;
	public Material mouseLeft;
	public Material buttonPlay;
	public Material buttonPause;
	public Material buttonRewind;
	public TextMesh textSeconds;
	public TextMesh textHundredths;
	public ProgressBar progressBar;
	public Transform circleInitialPosition;

	public float circleSpeed = 15.0f;

	public float seconds
	{
		get
		{
			return _seconds;
		}
		set
		{
			_seconds = value;

			UpdateTime();
		}
	}

	private float _seconds = 0.0f;

	void Awake()
	{
	}

	void OnDisable()
	{
		Go.killAllTweensWithTarget(this);
		Go.killAllTweensWithTarget(circle.transform);
	}

	private void UpdateTime()
	{
		progressBar.currentValue = _seconds;

		float time = Mathf.Min(_seconds, 10.0f);

		string[] tokens = time.ToString("00.00").Split('.');

		textSeconds.text = tokens[0];
		textHundredths.text = tokens[1];

		textSeconds.renderer.material.color = Color.black;
		textHundredths.renderer.material.color = Color.black;
	}

	private void MouseClick(bool left = true)
	{
		if (left)
		{
			mouse.renderer.material = mouseLeft;
		}
		else
		{
			mouse.renderer.material = mouseRight;
		}

		ExecuteDelayedAction(0.3f, () =>
		{
			mouse.renderer.material = mouseNormal;
		});
	}

	private void MoveCircle(float seconds, bool forward, System.Action onComplete)
	{
		Vector3 direction;

		if (forward)
		{
			AdvanceTime(seconds * 0.5f);
			direction = Vector3.right;
		}
		else
		{
			AdvanceTime(-seconds * 0.5f);
			direction = Vector3.left;
		}

		Vector3 destPosition = circle.transform.position + direction * circleSpeed * seconds;

		TweenFlow moveFlow = new TweenFlow();
		Tween moveTween = new Tween(circle.transform, seconds * 0.5f, new TweenConfig().position(destPosition).setEaseType(EaseType.Linear));
		moveFlow.insert(0, moveTween);
		moveFlow.setOnCompleteHandler((a) => { onComplete.Invoke(); });

		moveFlow.play();
	}

	private void AdvanceTime(float secondsToAdd)
	{
		float dest = _seconds + secondsToAdd;

		TweenFlow moveFlow = new TweenFlow();
		Tween moveTween = new Tween(this, Mathf.Abs(secondsToAdd), new TweenConfig().floatProp("seconds", dest).setEaseType(EaseType.Linear));
		moveFlow.insert(0, moveTween);

		moveFlow.play();
	}

	private void SetCircleState(CircleState state)
	{
		switch (state)
		{
			case CircleState.Paused:
				circleButton.renderer.material = buttonPause;
				break;

			case CircleState.Playing:
				circleButton.renderer.material = buttonPlay;
				break;

			case CircleState.Rewinding:
				circleButton.renderer.material = buttonRewind;
				break;
		}
	}

	public void StartAnimation()
	{
		circle.transform.position = circleInitialPosition.position;
		SetCircleState(CircleState.Playing);

		seconds = 0.0f;

		// move forward al the way
		ExecuteDelayedAction(1.5f, () =>
		{
			MouseClick();
			SetCircleState(CircleState.Paused);

			MoveCircle(4.0f, true, () =>
			{
				SetCircleState(CircleState.Rewinding);

				// move backward all the way
				ExecuteDelayedAction(1.5f, () =>
				{
					MouseClick();
					SetCircleState(CircleState.Paused);

					MoveCircle(4.0f, false, () =>
					{
						MouseClick();
						SetCircleState(CircleState.Playing);

						// move forward halfway again
						ExecuteDelayedAction(1.5f, () =>
						{
							MouseClick();
							SetCircleState(CircleState.Paused);

							MoveCircle(2.0f, true, () =>
							{
								MouseClick();
								SetCircleState(CircleState.Playing);

								// move backward halfway till the beginning
								ExecuteDelayedAction(1.5f, () =>
								{
									MouseClick(false);
									SetCircleState(CircleState.Paused);

									MoveCircle(2.0f, false, () =>
									{
										MouseClick();
										SetCircleState(CircleState.Playing);

										StartAnimation();
									});
								});
							});
						});
					});
				});
			});
		});



	//    // move forward halfway
	//    ExecuteDelayedAction(1.5f, () =>
	//    {
	//        MouseClick();
	//        SetCircleState(CircleState.Paused);

	//        MoveCircle(2.0f, true, () =>
	//        {
	//            MouseClick();
	//            SetCircleState(CircleState.Playing);

	//            // move forward all the way
	//            ExecuteDelayedAction(1.5f, () =>
	//            {
	//                MouseClick();
	//                SetCircleState(CircleState.Paused);

	//                MoveCircle(2.0f, true, () =>
	//                {
	//                    SetCircleState(CircleState.Rewinding);

	//                    // move backward all the way
	//                    ExecuteDelayedAction(1.5f, () =>
	//                    {
	//                        MouseClick();
	//                        SetCircleState(CircleState.Paused);

	//                        MoveCircle(4.0f, false, () =>
	//                        {
	//                            MouseClick();
	//                            SetCircleState(CircleState.Playing);

	//                            // move forward halfway again
	//                            ExecuteDelayedAction(1.5f, () =>
	//                            {
	//                                MouseClick();
	//                                SetCircleState(CircleState.Paused);

	//                                MoveCircle(2.0f, true, () =>
	//                                {
	//                                    MouseClick();
	//                                    SetCircleState(CircleState.Playing);

	//                                    // move backward halfway till the beginning
	//                                    ExecuteDelayedAction(1.5f, () =>
	//                                    {
	//                                        MouseClick(false);
	//                                        SetCircleState(CircleState.Paused);

	//                                        MoveCircle(2.0f, false, () =>
	//                                        {
	//                                            MouseClick();
	//                                            SetCircleState(CircleState.Playing);

	//                                            StartAnimation();
	//                                        });
	//                                    });
	//                                });
	//                            });
	//                        });
	//                    });
	//                });
	//            });
	//        });
	//    });
	}

	public void ExecuteDelayedAction(float delay, System.Action action)
	{
		if (this.gameObject.activeSelf == false)
		{
			return;
		}

		StartCoroutine(ExecuteDelayedAction_Coroutine(delay, action));
	}

	private IEnumerator ExecuteDelayedAction_Coroutine(float delay, System.Action action)
	{
		yield return new WaitForSeconds(delay);

		if (this.gameObject.activeSelf == false)
		{
			yield break;
		}

		action.Invoke();
	}
}
