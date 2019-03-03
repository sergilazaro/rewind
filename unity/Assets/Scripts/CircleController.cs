using UnityEngine;
using System.Collections;

public enum CircleType
{
	Static,
	Movable
}

public enum CircleState
{
	Paused,
	Playing,
	Rewinding
}

public class CircleController : MonoBehaviour
{
	public Material playMaterial;
	public Material pauseMaterial;
	public Material rewindMaterial;

	public Material regularCircleMaterial;
	public Material fullCircleMaterial;

	public Vector2 velocity = Vector2.zero;

	public CircleType circleType = CircleType.Static;
	public CircleState circleState = CircleState.Paused;

	private GameObject buttonChild;
	private GameObject blurChild;
	private GameManager gameManager
	{
		get
		{
			if (_gameManager == null)
			{
				_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
			}

			return _gameManager;
		}
	}

	private GameManager _gameManager = null;

	public static readonly float DISTANCE_EPSILON = 1e-8f;
	public static readonly float RAYCAST_EPSILON = 1e-2f;

	void Awake()
	{
		buttonChild = transform.Find("Button").gameObject;
		blurChild = transform.Find("Blur").gameObject;

		float angle = Mathf.Rad2Deg * Mathf.Atan2(velocity.y, velocity.x);

		//Debug.Log("angle = " + angle);

		UpdateVisualState();
	}

	private void UpdateVisualState()
	{
		blurChild.transform.localRotation = Quaternion.AngleAxis(180 - Mathf.Rad2Deg * Mathf.Atan2(velocity.y, velocity.x), Vector3.up);

		switch (circleType)
		{
			case CircleType.Static:
				buttonChild.SetActive(false);
				blurChild.SetActive(false);

				GetComponent<Renderer>().material = fullCircleMaterial;
				break;

			case CircleType.Movable:
				buttonChild.SetActive(true);
				blurChild.SetActive(true);

				GetComponent<Renderer>().material = regularCircleMaterial;

				switch (circleState)
				{
					case CircleState.Paused:
						if (CanMove(true))
						{
							buttonChild.GetComponent<Renderer>().material = playMaterial;
						}
						else if (CanMove(false))
						{
							buttonChild.GetComponent<Renderer>().material = rewindMaterial;
						}
						else
						{
							//Debug.Log("should never happen, i guess?");
							buttonChild.SetActive(false);
							blurChild.SetActive(false);
						}
						break;

					case CircleState.Playing:
					case CircleState.Rewinding:
						buttonChild.GetComponent<Renderer>().material = pauseMaterial;
						break;
				}

				break;
		}
	}

	private float GetRemainingDistance(bool forward, out bool justAboutToTouch, float timeDelta = Mathf.Infinity)
	{
		float worldRadius = GetWorldRadius();

		Vector2 actualVelocity = velocity;

		//Debug.Log("radius = " + worldRadius);

		if (!forward)
		{
			actualVelocity *= -1.0f;
		}

		Ray ray = new Ray(this.transform.position, actualVelocity.normalized);
		Debug.DrawRay(this.transform.position, actualVelocity.normalized, Color.red, 0.1f);

		RaycastHit[] hits = Physics.SphereCastAll(ray, worldRadius);

		//Debug.Log("spherecast: " + hits.Length + " hits");

		RaycastHit closest = new RaycastHit();
		bool found = false;
		float closestDist = Mathf.Infinity;

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider != this.GetComponent<Collider>())
			{
				if (hit.distance < closestDist)
				{
					Vector3 hereToThere = Vector3.zero;

					if (hit.collider.name.Contains("border"))
					{
						var rayhits = Physics.RaycastAll(new Ray(transform.position, actualVelocity.normalized), worldRadius);
						foreach (var rayhit in rayhits)
						{
							if (rayhit.collider == hit.collider)
							{
								hereToThere = (rayhit.point - transform.position);
							}
						}
					}
					else
					{
						hereToThere = (hit.collider.transform.position - this.transform.position);
					}

					if (Vector3.Dot(hereToThere, actualVelocity) <= 0.0f)
					{
						// it's behind
						continue;
					}

					closest = hit;
					closestDist = hit.distance;
					found = true;
				}
			}
		}

		float distanceToMove;

		if (found)
		{
			//if (closest.distance < DISTANCE_EPSILON)
			//{
			//    //Debug.Log("TOUCHING, returning 0");

			//    return 0.0f;
			//}
			//else
			if (closest.distance < (actualVelocity.magnitude * timeDelta))
			{
				distanceToMove = closest.distance;
				justAboutToTouch = true;

				//Debug.Log("found close collision, returning " + distanceToMove);
			}
			else
			{
				distanceToMove = actualVelocity.magnitude * timeDelta;
				justAboutToTouch = false;

				//Debug.Log("didn't find close collision, returning " + distanceToMove);
			}

			//Debug.Log("SPHERE HIT " + this.name + closest.collider.name);
		}
		else
		{
			distanceToMove = actualVelocity.magnitude * timeDelta;
			justAboutToTouch = false;

			//Debug.Log("didn't find any collision, returning " + distanceToMove);
		}

		return distanceToMove;
	}

	private bool CanMove(bool forward)
	{
		return CanMove(forward, 0.0f) && CanMove(forward, RAYCAST_EPSILON);
	}

	private bool CanMove(bool forward, float epsilon)
	{
		//return (GetRemainingDistance(forward) > 0.0f);

		float worldRadius = GetWorldRadius();

		Vector3 actualVelocity = velocity;

		//Debug.Log("radius = " + worldRadius);

		if (!forward)
		{
			actualVelocity *= -1.0f;
		}

		Vector3 origin = this.transform.position + (actualVelocity.normalized * -epsilon);

		Ray ray = new Ray(origin, actualVelocity.normalized);
		//Debug.DrawRay(this.transform.position, actualVelocity.normalized, Color.red, 0.1f);

		RaycastHit[] hits = Physics.SphereCastAll(ray, worldRadius);

		//Debug.Log("spherecast: " + hits.Length + " hits");

		RaycastHit closest = new RaycastHit();
		bool found = false;
		float closestDist = Mathf.Infinity;

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider != this.GetComponent<Collider>())
			{
				if (hit.distance < closestDist)
				{
					Vector3 hereToThere = Vector3.zero;

					if (hit.collider.name.Contains("border"))
					{
						var rayhits = Physics.RaycastAll(new Ray(transform.position, actualVelocity.normalized), worldRadius);
						foreach (var rayhit in rayhits)
						{
							if (rayhit.collider == hit.collider)
							{
								hereToThere = (rayhit.point - transform.position);
							}
						}
					}
					else
					{
						hereToThere = (hit.collider.transform.position - this.transform.position);
					}

					if (Vector3.Dot(hereToThere, actualVelocity) <= 0.0f)
					{
						// it's behind
						continue;
					}

					closest = hit;
					closestDist = hit.distance;
					found = true;
				}
			}
		}

		if (found)
		{
			if (closest.distance <= epsilon * 1.01f)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		else
		{
			return true;
		}
	}

	private float GetWorldRadius()
	{
		return (this.transform.TransformPoint(Vector3.right * (GetComponent<Collider>() as SphereCollider).radius) - this.transform.position).magnitude;
	}

	private void SwitchState(CircleState newState)
	{
		circleState = newState;

		UpdateVisualState();
	}

	void Update()
	{
		bool leftClick = Input.GetMouseButtonDown(0);
		bool rightClick = Input.GetMouseButtonDown(1);

		if (gameManager.gameState == GameState.Playing && (leftClick || rightClick))
		{
			RaycastHit hitinfo;

			if (circleType == CircleType.Movable)
			{
				if (GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitinfo, Mathf.Infinity))
				{
					switch (circleState)
					{
						case CircleState.Paused:
							if (leftClick)
							{
								if (CanMove(true))
								{
									SwitchState(CircleState.Playing);
								}
								else if (CanMove(false))
								{
									SwitchState(CircleState.Rewinding);
								}
								else
								{
									//throw new System.Exception("paused but can't move when clicked!");
								}
							}
							else
							{
								if (CanMove(false))
								{
									SwitchState(CircleState.Rewinding);
								}
							}
							break;

						case CircleState.Playing:
						case CircleState.Rewinding:
							SwitchState(CircleState.Paused);
							break;
					}
				}
				else
				{
					UpdateVisualState();
				}
			}
		}
		else
		{
			UpdateVisualState();
		}
	}

	void FixedUpdate()
	{
		if (circleType != CircleType.Movable)
		{
			return;
		}

		switch (circleState)
		{
			case CircleState.Playing:
			case CircleState.Rewinding:
				{
					bool forward = (circleState == CircleState.Playing);

					bool justAboutToTouch;

					float distanceToMove = GetRemainingDistance(forward, out justAboutToTouch, Time.fixedDeltaTime);

					Vector2 actualVelocity = velocity;

					if (!forward)
					{
						actualVelocity *= -1.0f;
					}

					this.transform.position = this.transform.position + new Vector3(actualVelocity.x, actualVelocity.y, 0).normalized * distanceToMove;

					float timeAdded = distanceToMove / velocity.magnitude;

					if (!forward)
					{
						timeAdded *= -1.0f;
					}

					gameManager.AddTime(timeAdded);

					if (justAboutToTouch || !CanMove(forward))
					{
						SwitchState(CircleState.Paused);
					}
				}
				break;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;

		Gizmos.DrawLine(transform.position, transform.position + new Vector3(velocity.x, velocity.y, 0).normalized * GetWorldRadius() * 2.0f);
	}
}
