// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using UnityEngine.UI;

public interface IDayNight
{
	/// <summary>
	/// Текстовое поле в UI в котором будет отображаться текущие время
	/// </summary>
	public Text GameTime { get; }
	/// <summary>
	/// Текстовое поле в UI в котором будет отображаться текущий номер суток
	/// </summary>
	public Text GameDays { get; }
}
