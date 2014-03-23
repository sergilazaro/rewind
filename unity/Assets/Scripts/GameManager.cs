using UnityEngine;
using System.Collections;

public enum GameState
{
	Playing,
	Transitioning,
	Ended
}

public class GameManager : MonoBehaviour
{
	public TextMesh textSeconds;
	public TextMesh textHundredths;

	public float currentTime_s;

	public ProgressBar progressBar;

	public LevelManager levelManager;

	public Color textColor;
	public Color wrongTextColor;
	public Color fullTextColor;

	public GameState gameState;

	void Awake()
	{
		currentTime_s = 0.0f;

		UpdateTime();
	}

	public void AddTime(float time)
	{
		currentTime_s += time;

		UpdateTime();

		CheckLevelEnded();
	}

	private void CheckLevelEnded()
	{
		if (currentTime_s >= 10.0f)
		{
			levelManager.LevelEnded();
		}
	}

	private void UpdateTime()
	{
		progressBar.currentValue = currentTime_s;

		float time = Mathf.Min(currentTime_s, 10.0f);
		//float time = currentTime_s;

		string[] tokens = time.ToString("00.00").Split('.');

		textSeconds.text = tokens[0];
		textHundredths.text = tokens[1];

		if (currentTime_s < 0.0f)
		{
			textSeconds.font.material.color = wrongTextColor;
		}
		else if (currentTime_s >= 10.0f)
		{
			textSeconds.font.material.color = fullTextColor;
		}
		else
		{
			textSeconds.font.material.color = textColor;
		}
	}
}
