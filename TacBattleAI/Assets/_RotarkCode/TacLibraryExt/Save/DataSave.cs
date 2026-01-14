
using System.Collections.Generic;
using System.Linq;
using Tac.Agent;
using Tac.DConvert;
using UnityEngine;
using static UnityEditor.Progress;

namespace Tac.Agent
{
	public partial class Agent
	{
		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);

			PathStatus = SaveQ(PathStatus, () => PathStatus);
			TargetPoint = SaveQ(TargetPoint, () => TargetPoint);
			WalkDistance = SaveQ(WalkDistance, () => WalkDistance);

			PathStatus = SaveQ(PathStatus, () => PathStatus);
			TargetId = SaveQ(TargetId, () => TargetId);
			IsBusy = SaveQ(IsBusy, () => IsBusy);
			LocatedId = SaveQ(LocatedId, () => LocatedId);
			health = SaveQ(health, () => health);
		}
	}

	public partial class AgentPoint
	{

		// Queue система сохранений не поддерживает, поэтому используем конвертацию через список
		public List<AgentInPoint> AgentsList
		{
			get { if (Agents != null) { return Agents.ToList(); } else { return null; } }
			set { Agents = new Queue<AgentInPoint>(value); }
		}

		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);
			Transform = SaveQ(Transform, () => Transform);
			AgentsList = SaveQ(AgentsList, () => AgentsList);

			BuildItem item = GetComponent<BuildItem>();
			if (item != null)
			{
				item.Turn.CurrentTurn = SaveQ(item.Turn.CurrentTurn, () => item.Turn.CurrentTurn); item.SetTurn();
			}
		}
	}
	public partial class AgentInPoint : ConvertData
	{
		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);

			Agent = SaveQ(Agent, () => Agent, PredefinedTag.OnlyPrefabId);
			EnterTime = SaveQ(EnterTime, () => EnterTime);
		}
	}
}

namespace Tac.Person
{
	public partial class Person
	{

		public Dictionary<string, int> LoadPlacesId;
		public Dictionary<string, int> PlacesId
		{
			get
			{
				Dictionary<string, int> ret = new Dictionary<string, int>();
				foreach (var item in Places)
				{
					if (item.Value != null)
					{
						ret.Add(item.Key, item.Value.Id);
					}
				}
				return ret;
			}
			set { LoadPlacesId = value; }
		}

		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);
			Transform = SaveQ(Transform, () => Transform);
			IsActive = SaveQ(IsActive, () => IsActive);

			Gender = SaveQ(Gender, () => Gender);
			Name = SaveQ(Name, () => Name);

			Stats = SaveQ(Stats, () => Stats);
			Skills = SaveQ(Skills, () => Skills);
			Info = SaveQ(Info, () => Info);
			PlacesId = SaveQ(PlacesId, () => PlacesId);
		}
	}

	public partial class PersonPlan : ConvertData
	{
		public PersonPlan() { }

		// Queue система сохранений не поддерживает, поэтому используем конвертацию через список
		public List<AgentPoint> DayPlanList
		{
			get { return DayPlan.ToList(); }
			set { DayPlan = new Queue<AgentPoint>(value); }
		}

		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);
			Person = SaveQ(Person, () => Person, PredefinedTag.OnlyPrefabId);
			DayPlanList = SaveQ(DayPlanList, () => DayPlanList, PredefinedTag.OnlyPrefabId);
		}
	}

}
