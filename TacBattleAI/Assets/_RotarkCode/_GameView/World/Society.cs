using DnaCore;
using System.Collections.Generic;
using System.Linq;
using Tac.Agent_;
using Tac.Person_;
using Tac.ItemCreate_;
using Tac.UI;
using UnityEF;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tac.Society
{
	public partial class Society : Flow
	{
        //[TacLogic] SocietyLogic logic;
#region Generated Logic
        protected SocietyLogic logic
        {
            get
            {
                return baseLogic as SocietyLogic;
            }

            set
            {
                baseLogic = value;
            }
        }

        protected override void CreateLogic()
        {
            baseLogic = new SocietyLogic();
        }

        public GDictionary<int, Person> People => logic.People;
        public RobotJob RobotJob => logic.RobotJob;
        public PlayerJob PlayerJob => logic.PlayerJob;
        public ItemCreate ItemCreate => logic.ItemCreate;
        public List<AgentPoint> AllAgentPoint => logic.AllAgentPoint;

        public List<Person> AddPerson(int argCount, Rect_ argLocation, List<Person> argFakePerson, bool IsFamily = true) => logic.AddPerson(argCount, argLocation, argFakePerson, IsFamily);
        public override void InitData()
        {
            base.InitData();
            logic.People = new GDictionary<int, Person>();
        }

        public Person AddPerson(Person argPerson) => logic.AddPerson(argPerson);
#endregion


		/// <summary>
		/// Идентификатор персонажа игрока
		/// </summary>
		public int PlayerPersonId = 0;


		public List<GameObject> MenModel;
		public List<GameObject> WomenModel;
		public AgentSelection AgentSelection;

		public GameObject PeoplePanel;


		public void Init()
		{
			logic.RobotJob = GetComponent<RobotJob>();
			logic.PlayerJob = GetComponent<PlayerJob>();

			logic.PersonName.LoadName(logic.rnd);
			logic.AllAgentPoint = GetComponentsInChildren<AgentPoint>().ToList();

			for (int i = 0; i < AllAgentPoint.Count; i++)
			{
				AllAgentPoint[i].Id = logic.ItemCreate.GetNewId();
				AllAgentPoint[i].GetInfoHandler = GetAgentPointInfo;
			}

			if (PeoplePanel != null)
			{
				TableUI tableUI = PeoplePanel.GetComponentInChildren<TableUI>();
				logic.PeopleTable.Assign(tableUI, FindAgent);
			}
		}


		int oldKey = 0;
		bool showPanel = false;
		private void Update()
		{
			int key = 0;
			if (Keyboard.current[Key.Digit1].wasPressedThisFrame) { key = 1; }

			if (key != 0)
			{
				if (PeoplePanel != null) { PeoplePanel.SetActive(false); logic.PeopleTable.Hide(); }

				if (oldKey == key) { showPanel = !showPanel; } else { showPanel = true; }

				if (showPanel)
				{
					switch (key)
					{
						case 1:
							PeoplePanel.SetActive(true);
							logic.PeopleTable.Show();
							break;
					}
				}
				oldKey = key;
			}

		}

		public void AddAgentPlan(Person argAgent, bool IsPlayer = false)
		{
			if (IsPlayer)
			{
				PlayerJob.AddPersonPlan(argAgent);
				PlayerPersonId = argAgent.Id;
			}
			else
			{
				RobotJob.AddPersonPlan(argAgent);
			}
		}
		public List<Person> AddPerson(int argCount, Rect_ argLocation, bool IsFamily = true)
		{
			List<Person> fakePerson = new List<Person>();

			for (int i = 0; i < argCount; i++)
			{
				fakePerson.Add(CreateFakePerson());
			}

			return logic.AddPerson(argCount, argLocation, fakePerson, IsFamily);
		}

		public Person CreateFakePerson()
		{
			Person person = new Person();

			int isMen = logic.rnd.Next(100);
			if (isMen > 50)
			{
				int menIndex = logic.rnd.Next(0, MenModel.Count);

				Flow menId = MenModel[menIndex].GetComponent<Flow>();

				person.ModelName = menId.ModelName;
				person.SetGender(GenderType.Men);
			}
			else
			{
				int womenIndex = logic.rnd.Next(0, WomenModel.Count);

				Flow womenId = WomenModel[womenIndex].GetComponent<Flow>();

				person.ModelName = womenId.ModelName;
				person.SetGender(GenderType.Women);
			}
			return person;
		}


		public string GetAgentPointInfo(int argId)
		{
			return "AgentPointInfo";
		}

		public void InitWorkPlace()
		{
			foreach (Person p in People.Values)
			{
				int pointIndex = logic.rnd.Next(0, AllAgentPoint.Count);
				p.SetPlace("WorkPlace", AllAgentPoint[pointIndex]);
			}
		}

		public void NextHour(GameTime argGameTime)
		{
			logic.oldGameTime = argGameTime;
			PlayerJob.NextHour(argGameTime);
			RobotJob.NextHour(argGameTime);
		}


		public void AddModel()
		{
			for (int i = 0; i < MenModel.Count; i++)
			{
				logic.ItemCreate.AddModel(MenModel[i]);
			}
			for (int i = 0; i < WomenModel.Count; i++)
			{
				logic.ItemCreate.AddModel(WomenModel[i]);
			}
		}


		public void FindAgent(int argId)
		{
			AgentSelection.OnAgentTap(People[argId]);
			AgentSelection.TopCamera.SetPosition(People[argId].transform.position);

			PeoplePanel.SetActive(false); 
			logic.PeopleTable.Hide();
		}


	}
}