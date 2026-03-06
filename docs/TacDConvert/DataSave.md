---
title: DataSave
---

Расширения классов, которые описывают как сохранять данные, рекоммендуется делать в одном .cs файле, чтобы видеть вместе весь _аспектный_ слой сохранений. Такой файл мы будем называть DataSave.cs и его содержимое называть _инструкции сохранения_ . У вас могут быть как минимум два таких файла, один относящийся к вашему проекту, в котором описаны инструкции сохранения для ваших классов. А также может потребоваться расширить уже имеющиеся классы в пакете TacLibrary. Его описание появится в проекте юнити после того как вы через Unity Package Manager установили пакет TacSave. Далее вам нужно сделать на него ссылку TacLibrary.asmref. 

В итоге у вас будет например, следующая структура директорий в проекте:
```csharp
Assets/
  Code/
    Save/
      DataSave.cs
      SaveManager.cs
    TacLibraryExt/
      Save/
        DataSave.cs
      TacLibrary.asmref // Выберете ссылку на TacLibrary.asmdef и отключите галочку "Use GUID"
```

Теперь поговорим о том, что будет содержать файл DataSave.cs. Ниже полный пример двух вариантов класса DataSave.cs. 

```csharp
// Assets/Code/Save/DataSave.cs
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
```

```csharp
// Assets/Code/TacLibraryExt/Save/DataSave.cs
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
```
