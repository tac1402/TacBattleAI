// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using UnityEngine;
using UnityEngine.UI;

namespace Tac
{
    public abstract partial class DayNight0 : MonoBehaviour
    {
		/// <summary>
		/// “екстовое поле в UI в котором будет отображатьс€ текущие врем€
		/// </summary>
		public Text gameTime;
		/// <summary>
		/// “екстовое поле в UI в котором будет отображатьс€ текущий номер суток
		/// </summary>
		public Text gameDays;

		/// <summary>
		/// ѕауза полной остановки
		/// </summary>
		public static bool PauseCompleteStop;

	}
}
