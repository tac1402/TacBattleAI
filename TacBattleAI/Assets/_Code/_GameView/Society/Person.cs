using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tac.Agent;

namespace TacSociety
{

	public partial class Person : Agent
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
