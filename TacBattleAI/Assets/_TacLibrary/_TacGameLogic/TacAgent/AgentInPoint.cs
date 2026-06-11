// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using UnityEF;

namespace Tac.Agent
{
	public partial class AgentInPoint : ItemDb, IId
	{
		public Agent Agent;
		public GameTime EnterTime;
	}
}
