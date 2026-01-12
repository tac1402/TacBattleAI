using Tac.DConvert;


public partial class World 
{
	public override void SaveData(bool argLoadMode)
	{
		base.SaveData(argLoadMode);

		DayNight.CurrentDay = base.SaveQ(DayNight.CurrentDay, () => DayNight.CurrentDay);
		DayNight.CurrentTime = base.SaveQ(DayNight.CurrentTime, () => DayNight.CurrentTime);

		Society.PlayerPersonId = base.SaveQ(Society.PlayerPersonId, () => Society.PlayerPersonId);
		Society.People = base.SaveQ(Society.People, () => Society.People, PredefinedTag.OnlyPrefabId);
		Society.AllBusiness = base.SaveQ(Society.AllBusiness, () => Society.AllBusiness, PredefinedTag.OnlyPrefabId);

		//Society.RobotJob.AgentPlans = SaveQ(Society.RobotJob.AgentPlans, () => Society.RobotJob.AgentPlans);
		//Society.PlayerJob.AgentPlans = SaveQ(Society.PlayerJob.AgentPlans, () => Society.PlayerJob.AgentPlans);
	}
}
