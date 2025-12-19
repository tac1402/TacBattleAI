// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tac.Agent;

namespace Tac.Person
{

	public partial class Person : Agent.Agent
	{

		#region  Stats & Skills

		public string DebugName;
		public string Name
		{
			get { return name; }
			set 
			{
				name = value;
				DebugName = name;
				/*StatusBar statusBar = SelectionUI.GetComponent<StatusBar>();
				if (statusBar != null)
				{ 
					statusBar.HealthBar.Text.text = name;
				}*/
			}
		}


		#endregion

	}

}
