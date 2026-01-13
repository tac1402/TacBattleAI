// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

namespace Tac.DConvert
{
	public interface IPrefabId : IId
	{
		public string PrefabName { get; set; }
	}

	public interface IId
	{
		public int Id { get; set; }
	}
}
