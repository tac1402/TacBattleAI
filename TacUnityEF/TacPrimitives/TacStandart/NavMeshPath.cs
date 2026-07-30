using System;

namespace Tac
{
	public class NavMeshPath
	{
		public Vector3_[] corners;
		public NavMeshPathStatus status;

		public enum NavMeshPathStatus
		{
			PathComplete,
			PathPartial,
			PathInvalid
		}
	}
}
