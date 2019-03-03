using UnityEngine;
using System.Collections;

public class ButtonPlay : MonoBehaviour
{
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hitinfo;

			if (GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitinfo, Mathf.Infinity))
			{
				// CLICKED

				GameObject.Find("PageManager").GetComponent<PageManager>().GoToPage(GamePage.Instructions);
			}
		}
	}
}
