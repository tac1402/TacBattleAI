// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

namespace Tac.DConvert
{
	public class NamedValue_ : ICustomConvert<NamedValue_, NamedValue>
	{
		public string Name;
		public float Value;

		public NamedValue_() { }

		public NamedValue_(string argName, float argValue)
		{
			Name = argName; Value = argValue;
		}

		public void ConvertFrom(NamedValue v)
		{
			Name = v.Name; Value = v.Value;
		}

		public NamedValue ConvertTo()
		{
			return new NamedValue(Name, Value);
		}
	}
}

