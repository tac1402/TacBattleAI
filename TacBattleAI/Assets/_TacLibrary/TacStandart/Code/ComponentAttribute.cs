using System;
using System.Collections.Generic;
using System.Linq;


public class ComponentAttribute : Attribute
{
	public Type Type;
	public List<Type> Required;

	public ComponentAttribute(Type argType) => Type = argType;
	public ComponentAttribute(Type argType, params Type[] argRequired)
	{ 
		Type = argType;
		Required = argRequired.ToList();
	}

}
