
using System.Collections.Generic;

using Tac.Person;

public partial class Society
{
	/// <summary>
	/// Все персонажи в игре
	/// </summary>
	public Dictionary<int, Person> People = new Dictionary<int, Person>();

	private PersonName PersonName = new PersonName();


}
