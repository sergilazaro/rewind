using UnityEngine;
using System.Collections;

public class ButtonTwitter : MonoBehaviour
{
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hitinfo;

			if (collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitinfo, Mathf.Infinity))
			{
				// CLICKED

				Application.OpenURL("http://twitter.com/sergilazaro");
			}
		}
	}
}