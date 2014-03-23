using UnityEngine;
using System.Collections;


public enum GamePage
{
	MainMenu,
	Instructions,
	PlayArea
}

public class PageManager : MonoBehaviour
{
	public GamePage currentPage;

	public GameObject mainMenu;
	public GameObject instructionsMenu;
	public GameObject playArea;

	public LevelManager levelManager;

	void Awake()
	{
		if (false)
		{
			// DEBUG / LEVEL DESIGN MODE

			Camera.main.transform.Find("Vignette").gameObject.SetActive(false);
			GoToPage(GamePage.PlayArea);
		}
		else
		{
			GoToPage(GamePage.MainMenu);
		}
	}

	public void GoToPage(GamePage page)
	{
		switch (page)
		{
			case GamePage.MainMenu:
				mainMenu.SetActive(true);
				instructionsMenu.SetActive(false);
				playArea.SetActive(false);

				//levelManager.Init();

				break;

			case GamePage.Instructions:
				mainMenu.SetActive(false);
				instructionsMenu.SetActive(true);
				playArea.SetActive(false);

				instructionsMenu.GetComponent<Instructions>().StartAnimation();
				break;

			case GamePage.PlayArea:
				mainMenu.SetActive(false);
				instructionsMenu.SetActive(false);
				playArea.SetActive(true);
				break;
		}

	}
}
