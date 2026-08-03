// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tac
{
	public abstract class Logic : IItemDb
	{
		/// <summary>
		/// Уникальный индентификатор объекта в мире
		/// </summary>
		//[DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public virtual int Id { get { return itemDb.Id; } set { itemDb.Id = value; } }

		/*public static int IdCounter = 0;

		public Logic()
		{
			SetId();
		}

		public void SetId()
		{
			IdCounter++;
			Id = IdCounter;
		}*/

		private ItemDb itemDb = new ItemDb();
		public ItemDb item { get { return itemDb; } }
		public virtual void InitLogic() { }

		[NotMapped]
		public bool RecoverMode
		{
			get { return ItemDb.RecoverMode; }
		}

		public static readonly RegCollection Reg = new RegCollection();
		public static void Register<T>(IAdd<T> iadd) where T : IItemDb
			=> Reg.Register(iadd);
	}
}
