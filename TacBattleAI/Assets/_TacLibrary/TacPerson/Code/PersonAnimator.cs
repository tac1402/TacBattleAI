// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using Tac.Agent;
using UnityEngine;


namespace Tac.Person
{
	public class PersonAnimator : AgentAnimator
	{
		public override void Move()
		{
			base.Move();

			animator.SetFloat("MoveX", 1);
			animator.SetFloat("MoveY", 0);

		}

		public override void Stop()
		{
			base.Stop();

			animator.SetFloat("MoveX", 0);
			animator.SetFloat("MoveY", 0);
		}
	}
}

