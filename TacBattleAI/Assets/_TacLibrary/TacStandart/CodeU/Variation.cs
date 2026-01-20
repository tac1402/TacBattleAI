using System.Collections.Generic;
using UnityEngine;

namespace Tac
{
	public class Variation : MonoBehaviour
	{
		public List<GameObject> Item;
		public int SizeFrom;
		public int SizeTill;
		public bool AllowRotate180;
		public Vector3 MaxRotateAngle;
		public bool RemoveMode;

		public int VariationId;
		
		private bool IsInit;
		private System.Random rnd = new System.Random();


		void Start()
		{
			Calc();
		}

		public void SetVariation(int argVariationId)
		{
			VariationId = argVariationId;
			for (int i = 0; i < Item.Count; i++)
			{
				if (i != VariationId)
				{
					if (RemoveMode == true)
					{
						Destroy(Item[i]);
					}
					else
					{
						Item[i].SetActive(false);
					}
				}
			}

			if (Item[VariationId] != null)
			{
				Item[VariationId].SetActive(true);
			}

			IsInit = true;
		}

		public void Calc()
		{
			if (IsInit) { return; }

			VariationId = rnd.Next(Item.Count);
			SetVariation(VariationId);

			if (SizeFrom != 0 && SizeTill != 0)
			{
				int num = SizeFrom + rnd.Next(SizeTill - SizeFrom);
				Vector3 vector3 = Item[VariationId].transform.localScale / 100f;
				Item[VariationId].transform.localScale = new Vector3(vector3.x * (float)num, vector3.y * (float)num, vector3.z * (float)num);
			}
			int num1 = 1;
			float angle1 = 0.0f;
			if (AllowRotate180)
			{
				num1 = rnd.Next(2) != 0 ? 1 : -1;
			}

			if (MaxRotateAngle.y != 0.0)
			{
				int num2 = rnd.Next(2) != 0 ? 1 : -1;
				angle1 = (float)(rnd.Next((int)MaxRotateAngle.y) * num2);
			}
			if (num1 == -1)
			{
				Item[VariationId].transform.Rotate(Vector3.up, 180f + angle1);
				Item[VariationId].transform.localPosition = new Vector3(Item[VariationId].transform.localPosition.x,
					Item[VariationId].transform.localPosition.y * -1f, Item[VariationId].transform.localPosition.z);
			}
			else
			{
				Item[VariationId].transform.Rotate(Vector3.up, angle1);
			}

			if (MaxRotateAngle.x != 0)
			{
				int num2 = rnd.Next(2) != 0 ? 1 : -1;
				float angle2 = (float)(rnd.Next((int)MaxRotateAngle.x) * num2);
				Item[VariationId].transform.Rotate(Vector3.right, angle2);
			}
			if (MaxRotateAngle.z != 0)
			{
				int num2 = rnd.Next(2) != 0 ? 1 : -1;
				float angle2 = (float)(rnd.Next((int)MaxRotateAngle.z) * num2);
				Item[VariationId].transform.Rotate(Vector3.back, angle2);
			}
		}



	}
}
