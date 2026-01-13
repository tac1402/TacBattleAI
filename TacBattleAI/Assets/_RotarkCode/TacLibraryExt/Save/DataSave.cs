
using System.Collections.Generic;
using UnityEngine;

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
			IsBusy = SaveQ(IsBusy, () => IsBusy);
			TargetId = SaveQ(TargetId, () => TargetId);

			Stats = SaveQ(Stats, () => Stats);
			Skills = SaveQ(Skills, () => Skills);
			Info = SaveQ(Info, () => Info);
			PlacesId = SaveQ(PlacesId, () => PlacesId);
		}
	}
}
