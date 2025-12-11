// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

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
