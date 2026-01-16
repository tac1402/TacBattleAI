
using System.Collections.Generic;
using Tac.Agent;
using Tac.DConvert;

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
			TargetId = SaveQ(TargetId, () => TargetId);
			IsBusy = SaveQ(IsBusy, () => IsBusy);
			LocatedId = SaveQ(LocatedId, () => LocatedId);
			health = SaveQ(health, () => health);
		}
	}

	public partial class AgentPoint
	{
		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);
			Transform = SaveQ(Transform, () => Transform);
			Agents = SaveQQ(Agents, () => Agents);
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
		public CrossRef<AgentPoint> PlacesRef = new CrossRef<AgentPoint>();
		public Dictionary<string, int> PlacesId
		{
			get { return PlacesRef.GetRef(Places); }
			set { PlacesRef.Ref = value; }
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

		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);
			Person = SaveQ(Person, () => Person, PredefinedTag.OnlyPrefabId);
			DayPlan = SaveQQ(DayPlan, () => DayPlan, PredefinedTag.OnlyPrefabId);
		}
	}

}

