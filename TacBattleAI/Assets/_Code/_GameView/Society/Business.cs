using UnityEngine;

using Tac.Agent;

namespace Tac.Society
{
	public partial class Business : AgentPoint
	{
		public override void Init()
		{
			base.Init();
			Title = "Частный дом";
		}
	}
}
