using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tac.DConvert
{

	public interface IEntity : IPrefab
	{
        /// <summary>
        /// —брасывает игру до начального состо€ни€ перед загрузкой сохранени€
        /// </summary>
        public void ResetGame();
        /// <summary>
        /// ¬осстанавливает производные данные и все процессы после загрузки сохранени€
        /// </summary>
		//public void RecoverGame();

		//public int DebugCount { get; set; }
	}

	public interface IPrefab
	{
		public GameObject GetObject(int argObjectId);

		public GameObject CreatePrefab(int argObjectId, string argModelName);
	}



}
