// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

namespace Tac.Agent
{
	public partial class AgentInPoint : IId
	{
		public Agent Agent;
		public GameTime EnterTime;

		int IId.Id { get { return Agent.Id; } set { } }
	}
}
