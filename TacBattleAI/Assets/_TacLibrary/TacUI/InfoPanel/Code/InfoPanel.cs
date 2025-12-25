using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Tac.UI
{
	public class InfoPanel : MonoBehaviour
	{
		public string Name;
		public TMP_Text Title;
		public TMP_Text Info;

		public void SetTitle(string argTitle)
		{
			Title.text = argTitle;
		}

		public void SetInfo(string argInfo)
		{
			Info.text = argInfo;
		}

		public void Show()
		{
			this.gameObject.SetActive(true);
		}
		public void Hide()
		{
			this.gameObject.SetActive(false);
		}
	}
}
