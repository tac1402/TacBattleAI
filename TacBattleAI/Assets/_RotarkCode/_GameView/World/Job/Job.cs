using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DnaCore;
using UnityEF;

namespace Tac.Society
{
	public class Job : Flow, ICell
	{
		public void AddPersonPlan(Person.Person argPerson) => logic.AddPersonPlan(argPerson);

		protected JobLogic logic;
		protected NavMeshPathExt pathExt;

		public Cell cell { get { return item; } }


		private void Awake()
		{
			pathExt = new NavMeshPathExt();
			logic = new JobLogic(pathExt.CalculatePath);
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
