
using System;

namespace Tac
{

	[Serializable]
	public class NamedValue
	{
		public string Name;
		public float Value;

		public NamedValue() { }
		public NamedValue(string argName, float argValue)
		{
			Name = argName;
			Value = argValue;
		}
	}
}
