

using Tac.DConvert;

public partial class World 
{
	public override void SaveData(bool argLoadMode)
	{
		base.SaveData(argLoadMode);

		DayNight.CurrentDay = SaveQ(DayNight.CurrentDay, () => DayNight.CurrentDay);
		DayNight.CurrentTime = SaveQ(DayNight.CurrentTime, () => DayNight.CurrentTime);

		Society.PlayerPersonId = SaveQ(Society.PlayerPersonId, () => Society.PlayerPersonId);
		Society.People = SaveQ(Society.People, () => Society.People, PredefinedTag.OnlyPrefabId);
		Society.AllAgentPoint = SaveQ(Society.AllAgentPoint, () => Society.AllAgentPoint, PredefinedTag.OnlyPrefabId);

		//Society.RobotJob.AgentPlans = SaveQ(Society.RobotJob.AgentPlans, () => Society.RobotJob.AgentPlans);
		//Society.PlayerJob.AgentPlans = SaveQ(Society.PlayerJob.AgentPlans, () => Society.PlayerJob.AgentPlans);
	}
}


