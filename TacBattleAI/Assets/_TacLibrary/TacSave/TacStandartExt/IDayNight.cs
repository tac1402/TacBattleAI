// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

public interface IDayNight
{
	/// <summary>
	/// Текстовое поле в UI в котором будет отображаться текущие время
	/// </summary>
	public string GameTime { get; set; }
	/// <summary>
	/// Текстовое поле в UI в котором будет отображаться текущий номер суток
	/// </summary>
	public string GameDays { get; set; }
}
