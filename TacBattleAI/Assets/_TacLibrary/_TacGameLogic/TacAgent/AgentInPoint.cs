// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using UnityEF;

namespace Tac.Agent
{
	public class AgentInPoint : Logic
	{
		public override int Id
		{
			get 
			{
				if (Agent != null)
				{
					return Agent.Id;
				}
				return 0;
			}
			set { }
		}

		public Agent Agent;
		public GameTime EnterTime;
	}
}
