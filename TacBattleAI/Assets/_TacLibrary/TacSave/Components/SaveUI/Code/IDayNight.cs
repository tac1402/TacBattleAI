using UnityEngine;
using UnityEngine.UI;

public interface IDayNight
{
	int CurrentDay { get; set; }

	/// <summary>
	/// Текстовое поле в UI в котором будет отображаться текущие время
	/// </summary>
	public Text GameTime { get; }

}
