using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tac.Agent
{

    public class AgentAnimator : MonoBehaviour
    {
        protected Agent agent;
		protected Animator animator;

		public bool IsMove;


		private void Awake()
		{
			agent = GetComponent<Agent>();
			animator = GetComponent<Animator>();
		}


		public void Update()
		{
			if (agent != null && agent.agent != null && agent.agent.enabled == true)
			{
				if (agent.agent.velocity.magnitude > 0.01f)
				{
					Move();
				}
				else
				{
					Stop();
				}
			}
		}

		public virtual void Move()
		{
			IsMove = true;
		}

		public virtual void Stop()
		{
			IsMove = false;
		}

	}
}