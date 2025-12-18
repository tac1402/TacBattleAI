
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Tac.Wireframe;
using Tac.Camera;

namespace Tac.ItemMove
{
	[Component(typeof(Wireframe.Wireframe), typeof(TopCamera))]
	public class ItemCollision : MonoBehaviour
	{
		public TopCamera TopCamera;
		public LayerMask EntityLayer;

		private Item2 objectToPlace;
		public Item2 ObjectToPlace
		{
			get { return objectToPlace; }
			set
			{
				objectToPlace = value;
				if (objectToPlace != null)
				{
					objectToPlace.Colliders = objectToPlace.GetComponentsInChildren<Collider>();
				}
			}
		}


		private bool isCollissionError = false;

		void Awake()
		{
			// for Debug
			//objectToPlace.Collider = objectToPlace.GetComponentsInChildren<Collider>();

			TopCamera.MoveError.Add("Collision", false);

			//CameraManager.OnShowError += OnShowError;
		}

		private void OnShowError(bool argError)
		{
			if (ObjectToPlace != null)
			{
				ObjectToPlace.ShowError(argError);
			}
		}

		private Queue<bool> Error = new Queue<bool>();

		public bool CollissionError
		{
			get
			{
				bool isError = false;
				List<bool> l = Error.ToList();
				for (int i = 0; i < l.Count; i++)
				{
					if (l[i] == true)
					{
						isError = true;
						break;
					}
				}
				return isError;
			}
		}

		public void OnItemMove(Item2 argItem)
		{
			if (argItem != null)
			{
				if (argItem.AllowMove == false) { return; }

				ObjectToPlace = argItem;
				ControlBox();
				TopCamera.MoveError["Collision"] = isCollissionError;
				OnShowError(isCollissionError);
			}
		}
		private void ControlBox()
		{
			isCollissionError = false;

			List<Collider> entityColliders = new List<Collider>();
			for (int i = 0; i < ObjectToPlace.Colliders.Length; i++)
			{
				GameObject colliderObj = ObjectToPlace.Colliders[i].gameObject;
				//BoxCollider box = colliderObj.GetComponent<BoxCollider>();
				Collider[] c = Physics.OverlapBox(colliderObj.transform.position, ObjectToPlace.Colliders[i].bounds.size / 2f,
													colliderObj.transform.rotation, EntityLayer, QueryTriggerInteraction.Collide);

				for (int j = 0; j < c.Length; j++)
				{
					entityColliders.Add(c[j]);
				}
			}

			Item2 newItem = null;
			int CollissionCount = 0;

			for (int i = 0; i < entityColliders.Count; i++)
			{
				newItem = Item2.GetItem(entityColliders[i].gameObject);

				if (newItem == null)
				{
					isCollissionError = true;
				}
				if (newItem != null && (newItem.ObjectId != ObjectToPlace.ObjectId || newItem.GhostId != ObjectToPlace.GhostId))
				{
					isCollissionError = true;
				}

				if (isCollissionError == true)
				{
					CollissionCount++;
				}
			}

			Error.Enqueue(isCollissionError);
			if (Error.Count >= 10)
			{
				Error.Dequeue();
			}
		}


		void OnDrawGizmos()
		{
			if (ObjectToPlace != null)
			{
				if (isCollissionError)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.blue;
				}

				for (int i = 0; i < ObjectToPlace.Colliders.Length; i++)
				{
					Gizmos.DrawWireCube(ObjectToPlace.Colliders[i].bounds.center, ObjectToPlace.Colliders[i].bounds.size);
				}
			}
		}

	}
}

namespace Tac
{
	public partial class Item2 : Item
	{
		public bool AllowMove = true;
		public Collider[] Colliders;

		/// <summary>
		/// Идентификатор, еще не построенного/ не существующего объекта в мире
		/// </summary>
		public int GhostId;

		public void InitColliders()
		{
			Colliders = GetComponentsInChildren<Collider>();
		}

		public static Item2 GetItem(GameObject go)
		{
			Item2 retItem = null;
			if (go != null)
			{
				retItem = go.GetComponent<Item2>();
				if (retItem == null)
				{
					BuildPart part = go.GetComponent<BuildPart>();
					if (part != null)
					{
						retItem = part.Main;
					}
				}
			}
			return retItem;
		}

	}
}
