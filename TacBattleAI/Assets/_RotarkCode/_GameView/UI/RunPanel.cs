using System;
using System.Collections;
using System.Collections.Generic;
using Tac;
using Tac.Agent;
using Tac.Person;
using Tac.Society;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunPanel : MonoBehaviour
{
	public TMP_Text Name;
	public TMP_Text Message;
	public Toggle Studies;
	public Toggle Work;
	public Toggle SpeedUp;

	private DayNight DayNight;
	private Society Society;
	private PlayerJob PlayerJob;

	public AgentSelection AgentSelection;


	public void Init(DayNight argDayNight, Society argSociety)
	{
		DayNight = argDayNight;
		DayNight.Pause = true;
		DayNight.NextDay += OnNextDay;
		DayNight.NextHour += OnNextHour;
		Society = argSociety;
		PlayerJob = argSociety.PlayerJob;

		Name.text = "Ваш персонаж: " + Society.People[Society.PlayerPersonId].Name;
		AgentSelection.OnAgentTap(Society.People[Society.PlayerPersonId]);
	}

	private void OnNextHour(GameTime argGameTime)
	{
		/*if (DayNight.Time == new TimeSpan(1, 0, 0))
		{
			if (Work.isOn == true)
			{
				PlayerJob.WorkMode = true;
			}
			else if (Studies.isOn == true)
			{
				PlayerJob.WorkMode = false;
			}
		}
		if (DayNight.Time == new TimeSpan(8, 0, 0))
		{
			if (PlayerJob.WorkMode && PlayerJob.WorkError > 0)
			{
				Message.text = "Вас приняли на завод";
			}
			else if (PlayerJob.WorkMode && PlayerJob.WorkError == -1)
			{
				Message.text = "Вас не приняли на завод";
				Work.isOn = false;
				Studies.isOn = true;
				PlayerJob.WorkMode = false;
			}
			else if(Society.People[Society.PlayerPersonId].IsFired == true)
			{
				Message.text = "Вас уволили";
				Work.isOn = false;
				Studies.isOn = true;
				PlayerJob.WorkMode = false;
				Society.People[Society.PlayerPersonId].IsFired = true;
			}
			else
			{
				Message.text = "";
			}
		}*/

	}

	private void OnNextDay(GameTime argGameTime)
	{
		if (AgentSelection.SelectedAgent == null)
		{
			AgentSelection.OnAgentTap(Society.People[Society.PlayerPersonId]);
		}
		else
		{
			Person person = AgentSelection.SelectedAgent as Person;
			if (person.Name != Society.People[Society.PlayerPersonId].Name)
			{
				AgentSelection.OnAgentTap(Society.People[Society.PlayerPersonId]);
			}
		}

		DayNight.Pause = true;
		gameObject.SetActive(true);
	}

	public void NextDay()
	{
		DayNight.Pause = false;
		gameObject.SetActive(false);
	}

	/*public void SetSpeedUp()
	{
		if (SpeedUp.isOn)
		{
			DayNight.PlaySpeed = 6;
		}
		else
		{
			DayNight.PlaySpeed = 2;
		}
	}*/

}
