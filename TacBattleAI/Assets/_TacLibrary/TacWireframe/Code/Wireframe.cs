// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using Tac.Wireframe;
using UnityEngine;

namespace Tac.Wireframe
{
    public class Wireframe : MonoBehaviour
    {
		public List<GameObject> MainRender;
		public List<GameObject> WireframeRender;

		public Material WireframeRed;
        public Material WireframeGreen;
		public Material WireframeGray;

		public WireframeMode CurrentMode;

        public void Hide()
        {
            CurrentMode = WireframeMode.Hide;
            for (int i = 0; i < WireframeRender.Count; i++)
            {
                WireframeRender[i].SetActive(false);
            }
			for (int i = 0; i < MainRender.Count; i++)
			{
				MainRender[i].SetActive(true);
			}
		}

        public bool IsMaterial(WireframeMode argMode)
        {
            bool ret = false;
			switch (argMode)
			{
				case WireframeMode.Red:
                    if (WireframeRed != null) { ret = true; }
					break;
				case WireframeMode.Green:
					if (WireframeGreen != null) { ret = true; }
					break;
				case WireframeMode.Gray:
					if (WireframeGray != null) { ret = true; }
					break;
			}
			return ret;
        }

		public void Show(WireframeMode argMode)
        {
            CurrentMode = argMode;
			for (int i = 0; i < MainRender.Count; i++)
			{
				MainRender[i].SetActive(false);
			}
			for (int i = 0; i < WireframeRender.Count; i++)
            {
                MeshRenderer[] meshRenderer = WireframeRender[i].GetComponentsInChildren<MeshRenderer>();
                for (int j = 0; j < meshRenderer.Length; j++)
                {
                    switch (argMode)
                    {
                        case WireframeMode.Red:
							meshRenderer[j].material = WireframeRed;
                            break;
                        case WireframeMode.Green:
							meshRenderer[j].material = WireframeGreen;
                            break;
                        case WireframeMode.Gray:
							meshRenderer[j].material = WireframeGray;
                            break;
                    }
                }
				WireframeRender[i].SetActive(true);
            }
        }
    }
	public enum WireframeMode
	{
		Hide = 0,
		Red = 1,
		Green = 2,
		Gray = 3
	}

}

namespace Tac
{
	public partial class Item2 : Item
	{
		public Wireframe.Wireframe Wireframe;


		public void InitWireframe()
		{
			Wireframe = gameObject.GetComponent<Wireframe.Wireframe>();
		}

		public void ShowError(bool argIsError)
		{
			if (Wireframe != null)
			{
				if (argIsError == true)
				{
					WireframeShow(WireframeMode.Red);
				}
				else
				{
					WireframeShow(WireframeMode.Green);
				}
			}
		}

		public void WireframeShow(WireframeMode argMode)
		{
			if (gameObject != null)
			{
				if (Wireframe != null)
				{
					if (Wireframe.IsMaterial(argMode))
					{
						Wireframe.Show(argMode);
					}
					else
					{
						Wireframe.Hide();
					}
				}
			}
		}
	}
}
