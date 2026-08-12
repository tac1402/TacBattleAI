// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using UnityEF;

namespace Tac
{
	public interface IAdd<T> : IAdd where T : IItemDb
	{
		T Add(T obj);
	}

	public interface IAdd { }
}
