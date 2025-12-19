using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogBox : MonoBehaviour
{
	public Text Title;
	public Text Text;
	public Button Button;

	private List<Button> tmpButton = new List<Button>();

	public virtual void Show(string title, string text, params string[] buttons)
	{
		Show(title, text, null, buttons);
	}

	public void Show(string title, string text, UnityAction<int> result, params string[] buttons)
	{
		for (int i = 0; i < tmpButton.Count; i++)
		{
			if (tmpButton[i] != null)
			{
				tmpButton[i].gameObject.SetActive(false);
				Destroy(tmpButton[i].gameObject);
			}
		}
		tmpButton.Clear();

		if (Title != null)
		{
			if (!string.IsNullOrEmpty(title))
			{
				Title.text = title;
				Title.gameObject.SetActive(true);
			}
			else
			{
				Title.gameObject.SetActive(false);
			}
		}
		if (Text != null)
		{
			Text.text = text;
		}

		gameObject.SetActive(true);
		Button.gameObject.SetActive(false);
		for (int i = 0; i < buttons.Length; i++)
		{
			string caption = buttons[i];
			int index = i;
			AddButton(caption).onClick.AddListener(delegate () {
				gameObject.SetActive(false);
				if (result != null)
				{
					result.Invoke(index);
				}
			});
		}
	}

	private Button AddButton(string text)
	{
		Button mButton = Instantiate(Button) as Button;
		tmpButton.Add(mButton);

		mButton.gameObject.SetActive(true);
		mButton.onClick.RemoveAllListeners();
		mButton.transform.SetParent(Button.transform.parent, false);
		Text[] buttonTexts = mButton.GetComponentsInChildren<Text>(true);
		if (buttonTexts.Length > 0)
		{
			buttonTexts[0].text = text;
		}
		return mButton;
	}
}
