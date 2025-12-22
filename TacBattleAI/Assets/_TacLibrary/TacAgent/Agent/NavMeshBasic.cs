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
		public NavMeshSurface surface;

		private void Start()
		{
			CreateSurface();
			UpdateSurface();
		}

		public void CreateSurface()
		{
			if (surface != null)
			{
				surface.BuildNavMesh();
			}
		}
		public void UpdateSurface()
		{
			if (surface != null)
			{
				surface.UpdateNavMesh(surface.navMeshData);
			}
		}
	}
}
