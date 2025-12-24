//#define OnlyLogic

using System.Collections.Generic;
using Tac;
using Tac.Person;
using Tac.Society;


public partial class World
{
	public Society Society;
	public DayNight DayNight;

	public Vector2Int_ WorldSize = new Vector2Int_(10, 10);

	private System.Random rnd = new System.Random();


	public void CreateWorld_Logic()
	{
		List<Person> people1 = Society.AddPerson(1, new Rect_(3400, 2920, 3420, 2940), false);
		Society.AddAgentPlan(people1[0], true);
		List<Person> people2 = Society.AddPerson(9, new Rect_(3400, 2920, 3420, 2940), false);
		foreach (Person person in people2)
		{
			Society.AddAgentPlan(person);
		}
	}

#if OnlyLogic

	public void Init()
	{
		DayNight = new DayNight();

		Society = new Society();
		Society.DayNight = DayNight;
		Society.Init();

		DayNight.NextHour += AgentWalkEmulation;
	}

	public void AgentWalkEmulation(GameTime argGameTime)
	{
		Society.NextHour(argGameTime);

		List<Agent> agents = Society.People.Values.ToList<Agent>();
		foreach (Landlot landlot in AllLandlot.Values) 
		{
			foreach (AgentPoint building in landlot.Building)
			{
				building.Tick(argGameTime, agents);
			}
		}

	}

#endif

}
