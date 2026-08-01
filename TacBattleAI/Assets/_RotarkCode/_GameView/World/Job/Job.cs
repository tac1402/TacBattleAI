using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Tac.Agent;
using UnityEF;
using UnityEngine;
using UnityEngine.AI;

namespace Tac.Society
{
	public class Job : Flow, ICell
	{
		protected new JobLogic logic => baseLogic as JobLogic;
		protected override void CreateLogic()
		{
			pathExt = new NavMeshPathExt();
			baseLogic = new JobLogic(pathExt.CalculatePath);
		}

		public void AddPersonPlan(Person.Person argPerson) => logic.AddPersonPlan(argPerson);

		protected NavMeshPathExt pathExt;

		public Cell cell { get { return item; } }


		private void Awake()
		{
			StartCoroutine(CalcPath());
		}

		protected IEnumerator CalcPath()
		{
			while (true)
			{
				logic.CalcPath();
				yield return new WaitForSeconds(0.1f);
			}
		}
	}
}
