// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Linq;


public class ComponentAttribute : Attribute
{
	public List<Type> Required;

	public ComponentAttribute(params Type[] argRequired)
	{ 
		Required = argRequired.ToList();
	}

}
