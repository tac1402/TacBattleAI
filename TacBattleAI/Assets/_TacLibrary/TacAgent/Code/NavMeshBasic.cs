// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2018-24 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

namespace Tac.Agent
{
	public class NavMeshBasic : MonoBehaviour
	{
		public NavMeshSurface Surface;

		private void Start()
		{
			CreateSurface();
			UpdateSurface();
		}

		public void CreateSurface()
		{
			if (Surface != null)
			{
				Surface.BuildNavMesh();
			}
		}
		public void UpdateSurface()
		{
			if (Surface != null)
			{
				Surface.UpdateNavMesh(Surface.navMeshData);
			}
		}
	}
}
