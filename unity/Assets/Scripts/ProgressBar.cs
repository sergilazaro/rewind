using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
	public Color frontColor;
	public Color backColor;
	public Material flatMaterial;

	public float maxValue = 1.0f;
	public float minValue = 0.0f;
	public float currentValue = 0.5f;

	private GameObject frontBar;
	private GameObject backBar;
	private GameObject frontBarHolder;

	void Awake()
	{
		frontBar = transform.Find("FrontBarHolder/FrontBar").gameObject;
		backBar = transform.Find("BackBar").gameObject;
		frontBarHolder = transform.Find("FrontBarHolder").gameObject;

		UpdateAppearance();
	}

	void Update()
	{
		UpdateAppearance();
	}

	private void UpdateAppearance()
	{
		frontBar.GetComponent<Renderer>().material = flatMaterial;
		backBar.GetComponent<Renderer>().material = flatMaterial;

		frontBar.GetComponent<Renderer>().material.color = frontColor;
		backBar.GetComponent<Renderer>().material.color = backColor;

		float normalizedValue = (currentValue - minValue) / (maxValue - minValue);
		normalizedValue = Mathf.Clamp01(normalizedValue);

		Vector3 scale = frontBarHolder.transform.localScale;

		scale.x = normalizedValue;

		frontBarHolder.transform.localScale = scale;
	}
}
