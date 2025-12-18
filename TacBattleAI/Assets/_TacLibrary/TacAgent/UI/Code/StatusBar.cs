using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
	public GameObject Status;
	public GameObject SelectView;

	public HealthBar HealthBar;
	public HealthBar StaminaBar;

	private Renderer renderer;

	public void Init()
	{
		if (Status != null)
		{
			renderer = Status.GetComponent<Renderer>();
		}
	}

	public void Select(bool isSelected)
	{
		if (SelectView != null)
		{
			SelectView.SetActive(isSelected);
		}
	}

	public void SetHealth(float argValue, float argPrevios = -1)
	{
		if (HealthBar != null)
		{
			if (argPrevios == -1) { argPrevios = argValue; }
			HealthBar.ChangeHealth(new MaxValue(argValue, argPrevios, 100));
		}
	}

	public void SetStamina(float argValue, float argPrevios = -1)
	{
		if (StaminaBar != null)
		{
			if (argPrevios == -1) { argPrevios = argValue; }
			StaminaBar.ChangeHealth(new MaxValue(argValue, argPrevios, 100));
		}
	}

	public void ChangeMaterial(Color argColor)
	{
		if (renderer != null)
		{
			renderer.material.color = argColor;
		}
	}



}
