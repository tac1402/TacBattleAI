//#define OnlyLogic

using System.Collections.Generic;
using System.Linq;
using Tac;
using Tac.Agent;
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
		List<Person> people2 = Society.AddPerson(2, new Rect_(3400, 2920, 3420, 2940), false);
		foreach (Person person in people2)
		{
			Society.AddAgentPlan(person);
		}
	}

	public void AgentWalkEmulation(GameTime argGameTime)
	{
		Society.NextHour(argGameTime);

		List<Agent> agents = Society.People.Values.ToList<Agent>();
		foreach (AgentPoint building in Society.AllAgentPoint)
		{
			building.Tick(argGameTime, agents);
		}
	}

#if OnlyLogic

	public void Init()
	{
		DayNight = new DayNight();

		Society = new Society();
		Society.DayNight = DayNight;
		Society.Init();
	}

#endif

}
