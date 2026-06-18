using DnaCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEF
{

	public interface IAspect
	{
		public Aspect aspect { get; }
	}

	public class Aspect : Framework2, IAspect
	{
		public Aspect aspect
		{
			get { return this; }
		}
	}

	public interface IAspect2
	{
		public Aspect2 aspect2 { get; }
	}

	public class Aspect2 : Framework3, IAspect2
	{
		public Aspect2 aspect2
		{
			get { return this; }
		}
	}

	public interface IAspect3
	{
		public Framework4 aspect3 { get; }
	}



	public class Framework1 { }

	public class Framework2 { }

	public class Framework3 : Framework4 { }

	public class Framework4 : IAspect3 
	{
		public Framework4 aspect3 { get { return this; } }
	}


	public class Adapter : Framework1, IAspect, IAspect2
	{
		private Aspect aspect_ = new Aspect();
		public Aspect aspect { get { return aspect_; } }

		private Aspect2 aspect2_ = new Aspect2();
		public Aspect2 aspect2 { get { return aspect2_; } }
	}

}
