// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2024-26 Sergej Jakovlev

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tac
{
	public class DiscreteMap_<T> : DiscreteMap
		where T : struct
	{
		private Dictionary<Vector2, T> Map = new Dictionary<Vector2, T>();
		public int Count { get { return Map.Count; } }
		public T this[int x, int y]
		{
			get { return Map[new Vector2(x, y)]; }
			set { Map[new Vector2(x, y)] = value; }
		}
		public T this[Vector2 v]
		{
			get { return Map[v]; }
			set { Map[v] = value; }
		}
		public Vector2 this[int index]
		{
			get
			{
				return Map.Keys.ToList()[index];
			}
		}


		public DiscreteMap_(Vector3 argCenter, Vector2Int argHalfSize, float argDiscreteStep, int argNumber = 0) : base(argCenter, argHalfSize, argDiscreteStep, argNumber) { }

		public override void AddMap(Vector2 argPoint) 
		{
			Map.Add(argPoint, default(T));
		}
		public override bool ContainMap(Vector2 argPoint) 
		{ 
			return Map.ContainsKey(argPoint); 
		}
		public override bool IsNull(Vector2 argPoint) 
		{
			T? t = Map[argPoint] as T?;
			if (t == null) { return true; } else { return false; }
		}
	}

	public class DiscreteMap<T> : DiscreteMap
		where T : class
	{
		private Dictionary<Vector2, T> Map = new Dictionary<Vector2, T>();
		public int Count { get { return Map.Count; } }
		public T this[int x, int y]
		{
			get { return Map[new Vector2(x, y)]; }
			set { Map[new Vector2(x, y)] = value; }
		}
		public T this[Vector2 v]
		{
			get { return Map[v]; }
			set { Map[v] = value; }
		}
		public Vector2 this[int index]
		{
			get
			{
				return Map.Keys.ToList()[index];
			}
		}


		public DiscreteMap(Vector3 argCenter, Vector2Int argHalfSize, float argDiscreteStep, int argNumber = 0) : base(argCenter, argHalfSize, argDiscreteStep, argNumber) { }

		public override void AddMap(Vector2 argPoint)
		{
			if (Map.ContainsKey(argPoint) == false)
			{
				Map.Add(argPoint, null);
			}
		}
		public override bool ContainMap(Vector2 argPoint)
		{
			return Map.ContainsKey(argPoint);
		}
		public override bool IsNull(Vector2 argPoint)
		{
			T t = Map[argPoint];
			if (t == null) { return true; } else { return false; }
		}
	}

	public class DiscreteMap
	{
		public Vector2Int HalfSize;
		public Vector3 Center;
		public float DiscreteStep;

		public virtual void AddMap(Vector2 argPoint) { }
		public virtual bool ContainMap(Vector2 argPoint) { return false; }
		public virtual bool IsNull(Vector2 argPoint) { return true; }

		public DiscreteMap(Vector3 argCenter, Vector2Int argHalfSize, float argDiscreteStep, int argNumber = 0)
		{ 
			HalfSize = argHalfSize;
			DiscreteStep = argDiscreteStep;
			Center = Discrete.Get2D(argCenter, argDiscreteStep);

			if (argNumber == 0) { CreateFull(argHalfSize, argDiscreteStep); }
			else { Create(argHalfSize, argDiscreteStep, argNumber); }
		}

		private void CreateFull(Vector2Int argHalfSize, float argDiscreteStep)
		{
			AddMap(new Vector2(0, 0));
			for (int i = 1; i < argHalfSize.x + 1; i++)
			{
				float xp = i * argDiscreteStep;
				float xm = -i * argDiscreteStep;

				AddMap(new Vector2(xp, 0));
				AddMap(new Vector2(xm, 0));
			}
			for (int j = 1; j < argHalfSize.y + 1; j++)
			{
				float yp = j * argDiscreteStep;
				float ym = -j * argDiscreteStep;

				AddMap(new Vector2(0, yp));
				AddMap(new Vector2(0, ym));
			}

			for (int i = 1; i < argHalfSize.x + 1; i++)
			{
				float xp = i * argDiscreteStep;
				float xm = -i * argDiscreteStep;

				for (int j = 1; j < argHalfSize.y + 1; j++)
				{
					float yp = j * argDiscreteStep;
					float ym = -j * argDiscreteStep;

					AddMap(new Vector2(xp, yp));
					AddMap(new Vector2(xm, yp));
					AddMap(new Vector2(xp, ym));
					AddMap(new Vector2(xm, ym));
				}
			}
		}

		private void Create(Vector2Int argHalfSize, float argDiscreteStep, int argNumber)
		{
			float xp = argHalfSize.x * argDiscreteStep;
			float xm = -argHalfSize.x * argDiscreteStep;
			AddMap(new Vector2(xp, 0));
			AddMap(new Vector2(xm, 0));

			float yp = argHalfSize.y * argDiscreteStep;
			float ym = -argHalfSize.y * argDiscreteStep;
			AddMap(new Vector2(0, yp));
			AddMap(new Vector2(0, ym));

			for (int i = 1; i < argHalfSize.x + 1; i++)
			{
				for (int j = 1; j < argHalfSize.y + 1; j++)
				{
					if (i == argNumber || j == argNumber)
					{
						xp = i * argDiscreteStep;
						xm = -i * argDiscreteStep;
						yp = j * argDiscreteStep;
						ym = -j * argDiscreteStep;

						AddMap(new Vector2(xp, yp));
						AddMap(new Vector2(xm, yp));
						AddMap(new Vector2(xp, ym));
						AddMap(new Vector2(xm, ym));
					}
				}
			}
		}


		public Vector3? GetNearestEmpty(Vector3 argPosition)
		{ 
			Vector3? ret = null;

			Vector2 point = (Discrete.Get2D(argPosition, DiscreteStep) - Center).To2();
			if (ContainMap(point) && IsNull(point)) { return point.To3(); }

			if (argPosition.x >= Center.x - HalfSize.x && argPosition.x <= Center.x + HalfSize.x &&
				argPosition.y >= Center.y - HalfSize.y && argPosition.y <= Center.y + HalfSize.y)
			{
				for (int i = 1; i < Mathf.Max(HalfSize.x, HalfSize.y) + 1; i++)
				{
					DiscreteMap_<int> mask = new DiscreteMap_<int>(Vector3.zero, new Vector2Int(i, i) , DiscreteStep, i);
					if (i > 1) 
					{
						int a = 1;
					}
					for (int j = 0; j < mask.Count; j++)
					{
						Vector2 nextPoint = point + mask[j];
						if (ContainMap(nextPoint) && IsNull(nextPoint))
						{
							ret = nextPoint.To3();
							break;
						}
					}
					if (ret != null) { break; }
				}
			}

			return ret;
		}

	}


	public static class Discrete
	{
		public static Vector3 Get2D(Vector3 argValue, float argDiscreteStep)
		{
			return Get(argValue, new Vector3(argDiscreteStep, 0, argDiscreteStep), XYZ.XZ);
		}

		public static Vector3 Get2D(Vector3 argValue, Vector3 argDiscreteStep)
		{
			return Get(argValue, argDiscreteStep, XYZ.XZ);
		}

		public static Vector3 Get3D(Vector3 argValue, float argDiscreteStep)
		{
			return Get(argValue, new Vector3(argDiscreteStep, argDiscreteStep, argDiscreteStep), XYZ.XYZ);
		}

		public static Vector3 Get3D(Vector3 argValue, Vector3 argDiscreteStep)
		{
			return Get(argValue, argDiscreteStep, XYZ.XYZ);
		}

		public static Vector3 Get(Vector3 argValue, Vector3 argDiscreteStep, XYZ argXYZ)
		{
			Vector3 ret = argValue;

			if (XYZ_.IsX(argXYZ))
			{
				ret.x = ((int)(argValue.x / argDiscreteStep.x)) * argDiscreteStep.x;
			}
			if (XYZ_.IsY(argXYZ))
			{
				ret.y = ((int)(argValue.y / argDiscreteStep.y)) * argDiscreteStep.y;
			}
			if (XYZ_.IsZ(argXYZ))
			{
				ret.z = ((int)(argValue.z / argDiscreteStep.z)) * argDiscreteStep.z;
			}
			return ret;
		}
	}
}