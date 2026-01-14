

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

		Society.RobotJob.PersonPlans = SaveQ(Society.RobotJob.PersonPlans, () => Society.RobotJob.PersonPlans);
		Society.PlayerJob.PersonPlans = SaveQ(Society.PlayerJob.PersonPlans, () => Society.PlayerJob.PersonPlans);
	}
}


