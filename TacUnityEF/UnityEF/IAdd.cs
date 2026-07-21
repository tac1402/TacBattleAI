// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

namespace Tac
{
	public interface IAdd<T> : IAdd where T : Item
	{
		T Add(T obj);
	}

	public interface IAdd { }
}
