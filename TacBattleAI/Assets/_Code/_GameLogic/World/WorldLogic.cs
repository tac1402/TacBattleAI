//#define OnlyLogic

using System.Collections.Generic;
using Tac;
using Tac.Person;


public partial class World
{
	public bool IsFoodDlc = false;

	public Society Society;
	//public DayNightController DayNightController;

	public Vector2Int_ WorldSize = new Vector2Int_(10, 10);
	public Vector2Int_ LandlotSize = new Vector2Int_(100, 100);

	public int HauseCount = 0;
	public int AgentCount = 0;

	//private School School;
	//private Factory Factory;
	//private Dorm Dorm;

	//private Dictionary<string, Landlot> AllLandlot = new Dictionary<string, Landlot>();
	//private Dictionary<PropertyType, bool> isOne = new Dictionary<PropertyType, bool>();
	//private AgentPoint Center;

	private System.Random rnd = new System.Random();


	public void CreateWorld_Logic()
	{
		/*for (int x = 0; x < WorldSize.x; x++)
		{
			for (int y = 0; y < WorldSize.y; y++)
			{
				Vector2Int_ position = new Vector2Int_(x * LandlotSize.x + LandlotSize.x / 2, y * LandlotSize.y + LandlotSize.y / 2);
				string key = x.ToString() + "-" + y.ToString();

				Landlot land = CreateLandlot(position);

				land.Number = key;

				if (key == "5-5")
				{
					if (Center == null)
					{
						Center = land;
						land.IsCenter = true;
					}
					else { land = Center as Landlot; }

				}

				AllLandlot.Add(land.Number, land);
			}
		}

		CreateForest();
		CreateLand();
		CreateCoal();
		CreateOil();
		*/

		List<Person> people1 = Society.AddPeople(1, new Rect_(480, 450, 500, 470), false);
		//people1[0].Age = 18;
		//people1[0].ResidencePlace = Dorm;
		//people1[0].LearningPlace = School;
		//Society.AddAgentPlan(people1[0], true);
		List<Person> people2 = Society.AddPeople(9, new Rect_(480, 450, 500, 470), false);
		foreach (Person person in people2)
		{
			//person.Age = 18;
			//person.ResidencePlace = Dorm;
			//person.LearningPlace = School;
			//Society.AddAgentPlan(person);
		}

		/*foreach (var item in AllLandlot)
		{
			int isType = 0;
			Landlot l = item.Value;

			if (item.Key == "4-5" || item.Key == "5-5")
			{
				l.Type = PropertyType.CityLandlot;
			}
			l.Generate();

			AgentPoint hause = null;
			AgentPoint business = null;
			PropertyType businessType = PropertyType.None;

			Business addBusiness = null;

			if (l.Type != PropertyType.CityLandlot)
			{
				int p = rnd.Next(100);

				bool locIsOne;
				if (IsFoodDlc == false)
				{
					locIsOne = IsOne(PropertyType.Farm, PropertyType.ForestLandlot, PropertyType.CoalLandlot, PropertyType.OilLandlot);
				}
				else
				{
					locIsOne = IsOne(PropertyType.GrainPlantation, PropertyType.ForagePlantation, PropertyType.PotatoPlantation, PropertyType.SunflowerPlantation,
										PropertyType.PigFarm, PropertyType.Cowshed, PropertyType.PoultryFarm,
										PropertyType.ForestLandlot, PropertyType.CoalLandlot, PropertyType.OilLandlot);
				}


				if (p > 50 || locIsOne == false)
				{
					isType = 1;
					hause = l.CreateObject("SmallHause");
					hause.PropertyType = PropertyType.SmallHause;

					int p2 = rnd.Next(100);
					switch (l.Type)
					{
						case PropertyType.LandLandlot:
							isType = 2;

							if (IsFoodDlc == false)
							{
								businessType = PropertyType.Farm;
								business = l.CreateObject("Farm");
							}
							else
							{
								int p3 = rnd.Next(7) + 1;

								switch (p3)
								{
									case 1:
										businessType = PropertyType.GrainPlantation;
										business = l.CreateObject("Plantation");
										break;
									case 2:
										businessType = PropertyType.ForagePlantation;
										business = l.CreateObject("Plantation");
										break;
									case 3:
										businessType = PropertyType.PotatoPlantation;
										business = l.CreateObject("Plantation");
										break;
									case 4:
										businessType = PropertyType.SunflowerPlantation;
										business = l.CreateObject("Plantation");
										break;
									case 5:
										businessType = PropertyType.PigFarm;
										business = l.CreateObject("Husbandry");
										break;
									case 6:
										businessType = PropertyType.Cowshed;
										business = l.CreateObject("Husbandry");
										break;
									case 7:
										businessType = PropertyType.PoultryFarm;
										business = l.CreateObject("Husbandry");
										break;
								}
							}
							break;
						case PropertyType.ForestLandlot:
							if (isOne.ContainsKey(PropertyType.Sawmill) == false || p2 > 50)
							{
								isType = 3;
								businessType = PropertyType.Sawmill;
								business = l.CreateObject("Sawmill");
							}
							break;
						case PropertyType.CoalLandlot:
							if (isOne.ContainsKey(PropertyType.CoalMine) == false || p2 > 50)
							{
								isType = 4;
								businessType = PropertyType.CoalMine;
								business = l.CreateObject("CoalMine");
							}
							break;
						case PropertyType.OilLandlot:
							if (isOne.ContainsKey(PropertyType.OilTank) == false || p2 > 50)
							{
								isType = 5;
								businessType = PropertyType.OilTank;
								business = l.CreateObject("OilTank");
							}
							break;
					}
					if (isOne.ContainsKey(businessType) == false)
					{
						isOne.Add(businessType, true);
					}
				}

				if (IsFoodDlc)
				{
					int p3 = rnd.Next(100);

					if (p3 > 90 || IsOne(PropertyType.Mill, PropertyType.BreadFactory, PropertyType.FoodFactory) == false)
					{
						int p4 = rnd.Next(3) + 1;

						switch (p4)
						{
							case 1:
								addBusiness = l.CreateObject("Mill") as Business;
								addBusiness.PropertyType = PropertyType.Mill;
								break;
							case 2:
								addBusiness = l.CreateObject("BreadFactory") as Business;
								addBusiness.PropertyType = PropertyType.BreadFactory;
								break;
							case 3:
								addBusiness = l.CreateObject("FoodFactory") as Business;
								addBusiness.PropertyType = PropertyType.FoodFactory;
								break;
						}
						if (isOne.ContainsKey(addBusiness.PropertyType) == false)
						{
							isOne.Add(addBusiness.PropertyType, true);
						}
					}
				}
			}

			int peopleCount = 0;
			if (isType > 0)
			{
				peopleCount = rnd.Next(1, 6);

				List<Person> people = Society.AddPeople(peopleCount, l);

				Society.AddOwner(OwnerType.Owner, people[0], l.Type, l);

				// Обучаем и устраиваем владельцев
				Business b = business as Business;
				if (b != null || addBusiness != null)
				{
					people[0].Age = rnd.Next(35, 60);
					people[0].ResidencePlace = hause;

					if (b != null)
					{
						b.PropertyType = businessType;
						Society.AddBusiness(b);

						Society.AddOwner(OwnerType.Owner, people[0], businessType, b);
						LearningOwner(people[0], b);

						b.Owner = people[0];

						people[0].WorkPlace = b;
					}

					if (addBusiness != null)
					{
						Society.AddBusiness(addBusiness);

						Society.AddOwner(OwnerType.Owner, people[0], addBusiness.PropertyType, addBusiness);

						addBusiness.Owner = people[0];

						if (b == null)
						{
							people[0].WorkPlace = addBusiness;
						}
					}

					Society.AddAgentPlan(people[0]);
				}
				else // Владелец без бизнеса
				{
					people[0].Age = rnd.Next(35, 60);
					LearningWorker(people[0]);

					people[0].ResidencePlace = hause;
					people[0].CenterPlace = Center;
					Society.AddAgentPlan(people[0]);
				}

				Society.AddOwner(OwnerType.Owner, people[0], PropertyType.SmallHause, hause);

				for (int i = 1; i < people.Count; i++)
				{
					Society.AddOwner(OwnerType.FamilyUse, people[i], PropertyType.SmallHause, hause);
					people[i].Age = rnd.Next(20, 60);
					LearningWorker(people[i]);

					if (b != null)
					{
						// Устраиваем семью в семейный бизнес
						(JobType jobType, int index) = b.Contest(people[i]);
						if (index >= 0)
						{
							people[i].WorkPlace = b;
							people[i].ResidencePlace = hause;
							Society.AddAgentPlan(people[i]);

							Society.RobotJobSystem.AgreeToWork(people[i].Id, b, index);
						}
						else // Те кто не смог устроится в семейный бизнес
						{
							people[i].CenterPlace = Center;
							Society.AddAgentPlan(people[i]);
						}
					}
					else // Семья, кроме владельца, у которых нет бизнеса
					{
						people[i].CenterPlace = Center;
						Society.AddAgentPlan(people[i]);
					}
				}

				HauseCount++;
				AgentCount += people.Count;
			}

		}*/


	}

	/*
	private void CreateForest()
	{
		int Amount, x, xloc, yloc;

		Amount = rnd.Next(6) + 2;
		for (x = 0; x < Amount; x++)
		{
			xloc = rnd.Next(WorldSize.x - 1);
			yloc = rnd.Next(WorldSize.y - 1);
			treeSplash(xloc, yloc);
		}
	}

	private void CreateLand()
	{
		int Amount = rnd.Next(10) + 20;
		for (int i = 0; i < Amount; i++)
		{
			string position = GetFreePosition();
			if (position != "")
			{
				AllLandlot[position].Type = PropertyType.LandLandlot;
			}
		}
	}

	private void CreateCoal()
	{
		int Amount = rnd.Next(5) + 5;
		for (int i = 0; i < Amount; i++)
		{
			string position = GetFreePosition();
			if (position != "")
			{
				AllLandlot[position].Type = PropertyType.CoalLandlot;
			}
		}
	}

	private void CreateOil()
	{
		int Amount = rnd.Next(5) + 2;
		for (int i = 0; i < Amount; i++)
		{
			string position = GetFreePosition();
			if (position != "")
			{
				AllLandlot[position].Type = PropertyType.OilLandlot;
			}
		}
	}

	private string GetFreePosition()
	{
		string ret = "";
		int xloc, yloc;

		for (int j = 0; j < 10; j++)
		{
			xloc = rnd.Next(WorldSize.x - 1);
			yloc = rnd.Next(WorldSize.y - 1);
			string position = xloc.ToString() + "-" + yloc.ToString();

			if (AllLandlot[position].Type == PropertyType.CityLandlot || AllLandlot[position].Type == PropertyType.ForestLandlot)
			{
				ret = position;
				break;
			}
		}
		return ret;
	}

	private void treeSplash(int xloc, int yloc)
	{
		int numTrees;

		numTrees = rnd.Next(15) + 5;

		Position treePosition = new Position(xloc, yloc, WorldSize.x, WorldSize.y);

		while (numTrees > 0)
		{
			Direction2 dir = Direction2.DIR2_NORTH + rnd.Next(7);
			treePosition.move(dir);

			if (treePosition.testBounds() == false)
			{
				return;
			}

			string position = treePosition.X.ToString() + "-" + treePosition.Y.ToString();
			AllLandlot[position].Type = PropertyType.ForestLandlot;

			numTrees--;
		}
	}

	private bool IsOne(params PropertyType[] argType)
	{
		bool ret = true;
		foreach (PropertyType p in argType)
		{
			if (isOne.ContainsKey(p) == false)
			{
				ret = false;
				break;
			}
		}
		return ret;
	}

	private int LearningWorkerMax = 30;
	private void LearningWorker(Person argPerson)
	{
		int count = rnd.Next(LearningWorkerMax / 10, LearningWorkerMax);
		for (int i = 0; i < count; i++)
		{
			School.Learning(argPerson);
		}
	}

	private int LearningOwnerMax = 0;
	private void LearningOwner(Person argPerson, Business argBusiness)
	{
		if (LearningOwnerMax == 0)
		{
			float bCompetence = argBusiness.CalcBusinessCompetence();
			float pCompetence = 0;

			bool endLearning = false;
			while (endLearning == false)
			{
				School.Learning(argPerson);
				pCompetence = argBusiness.CalcOwnerCompetence();
				LearningOwnerMax++;

				if (bCompetence / 2 < pCompetence)
				{
					endLearning = true;
				}
			}
			LearningOwnerMax = LearningOwnerMax * 2;
		}
		else
		{
			int count = rnd.Next(LearningOwnerMax / 10, LearningOwnerMax);
			for (int i = 0; i < count; i++)
			{
				School.Learning(argPerson);
			}
		}
	}*/



#if OnlyLogic
	public Landlot CreateLandlot(Vector2Int_ position)
	{
		Landlot landlot = new Landlot();
		landlot.DayNightController = DayNightController;
		return landlot;
	}

	public void Init()
	{
		DayNightController = new DayNightController();

		Center = CreateLandlot(new Vector2Int_(0, 0));
		Landlot l = Center as Landlot;
		l.Number = "5-5";
		School = l.CreateObject("School") as School;
		School.DayNightController = DayNightController;
		Factory = l.CreateObject("Factory") as Factory;
		Factory.DayNightController = DayNightController;
		Dorm = l.CreateObject("Dorm") as Dorm;
		Dorm.DayNightController = DayNightController;

		Society = new Society();
		Society.DayNightController = DayNightController;
		Society.Factory = Factory;
		Society.School = School;
		Society.Init();

		DayNightController.NextHour += AgentWalkEmulation;
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
