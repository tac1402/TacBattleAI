// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using UnityEngine;

namespace Tac.Camera
{
	public class AgentCamera : MonoBehaviour
	{
		public float Smoothness = 4;
		public Vector2 Sensitivity = new Vector2(4, 4);
		public Vector2 Limit = new Vector2(-70, 80);
		public Vector3 CameraShift;

		//[HideInInspector]
		public Vector2 CurrentCoord;
		public FastController FastController;

		public LayerMask cullingLayer = (LayerMask)1;
		public float CollisionKoef = 0.8f;
		public float ForwardOffset = 0.5f;
		public float clipPlaneMargin;

		private UnityEngine.Camera camera;
		private float ScreenKoef = 1;
		private float MaxDistance = 1f;
		private float AgentHeight = 0;

		private Vector2 vel;
		private Vector2 NewCoord;


		Vector3 idealPosition;
		Vector3 tempBackPosition;
		Vector3 tempForwardPosition;
		public Vector3 tempPosition;
		Vector3 tempCollision;

		private ClipPlanePoints plane;
		private Color[] colors = new Color[4];


		private void Start()
		{
			camera = GetComponent<UnityEngine.Camera>();
			ScreenKoef = camera.aspect * 3.0f;

			NewCoord = CurrentCoord;
			UpdateCamera();
		}


		void LateUpdate()
		{
			//if (Time.deltaTime != 0) // не пауза
			{
				if (Input.GetMouseButton(1))
				{
					UpdateCameraInput();
				}
				UpdateCamera();
			}
		}

		public void UpdateCameraInput()
		{
			float locMouseX = Input.GetAxisRaw("Mouse X");
			float locMouseY = Input.GetAxisRaw("Mouse Y");
			NewCoord.x -= locMouseY * Sensitivity.x;
			NewCoord.y += locMouseX * Sensitivity.y;

			NewCoord.x = Mathf.Clamp(NewCoord.x, Limit.x, Limit.y);
		}

		public int IsCollision;


		public void UpdateCamera()
		{
			CurrentCoord.x = Mathf.SmoothDamp(CurrentCoord.x, NewCoord.x, ref vel.x, Smoothness / 100, 1000, Time.unscaledDeltaTime);
			CurrentCoord.y = Mathf.SmoothDamp(CurrentCoord.y, NewCoord.y, ref vel.y, Smoothness / 100, 1000, Time.unscaledDeltaTime);

			//Debug.Log(CurrentCoord.x.ToString() + " - " + CurrentCoord.y.ToString());

			Quaternion newRotation = Quaternion.Euler(CurrentCoord.x, CurrentCoord.y, 0);
			idealPosition = transform.parent.position - (newRotation * Vector3.forward * -CameraShift.z
					+ newRotation * Vector3.right * -CameraShift.x
					+ new Vector3(0, -CameraShift.y, 0));


			AgentHeight = 0.01f;

			IsCollision = chkCameraCollision(idealPosition, transform.parent.position + transform.parent.up * AgentHeight,
						NearClipPlanePoints(camera, idealPosition, clipPlaneMargin), MaxDistance, cullingLayer) ? 1 : 0;

			//transform.parent.rotation = newRotation;
			if (IsCollision != 0)
			{
				//transform.parent.position = tempPosition;
				//FastController.AllowMove = false;
			}
			else
			{
				transform.parent.position = idealPosition;
				//FastController.AllowMove = true;
			}

		}

		private void SetColor(int argNumber)
		{
			colors[0] = Color.white;
			colors[1] = Color.white;
			colors[2] = Color.white;
			colors[3] = Color.white;
			colors[argNumber] = Color.black;
		}

		private bool chkCameraCollision(Vector3 argStart, Vector3 argEnd, ClipPlanePoints argTo, float distance, LayerMask cullingLayer)
		{
			bool retIsCollision = false;
			plane = argTo;
			float cullingDistance = distance;
			RaycastHit hitInfo;
			if (Physics.Raycast(argEnd, argTo.LowerLeft - argEnd, out hitInfo, distance, (int)cullingLayer))
			{
				retIsCollision = true;
				cullingDistance = hitInfo.distance;
				tempCollision = hitInfo.point;
				SetColor(0);
			}
			if (Physics.Raycast(argEnd, argTo.LowerRight - argEnd, out hitInfo, distance, (int)cullingLayer))
			{
				retIsCollision = true;
				if (cullingDistance > hitInfo.distance)
				{
					cullingDistance = hitInfo.distance;
					tempCollision = hitInfo.point;
					SetColor(1);
				}
			}
			if (Physics.Raycast(argEnd, argTo.UpperLeft - argEnd, out hitInfo, distance, (int)cullingLayer))
			{
				retIsCollision = true;
				if (cullingDistance > hitInfo.distance)
				{
					cullingDistance = hitInfo.distance;
					tempCollision = hitInfo.point;
					SetColor(2);
				}
			}
			if (Physics.Raycast(argEnd, argTo.UpperRight - argEnd, out hitInfo, distance, (int)cullingLayer))
			{
				retIsCollision = true;
				if (cullingDistance > hitInfo.distance)
				{
					cullingDistance = hitInfo.distance;
					tempCollision = hitInfo.point;
					SetColor(3);
				}
			}
			if (retIsCollision)
			{
				Vector3 center = transform.parent.position + transform.parent.up * AgentHeight;
				float num = cullingDistance * CollisionKoef;
				Vector3 vector3_1 = center - argStart;
				Vector3 vector3_2 = vector3_1 / vector3_1.magnitude;
				tempBackPosition = center - vector3_2 * num;

				if (Vector3.Distance(center, tempBackPosition) < ForwardOffset)
				{
					tempForwardPosition = center + transform.parent.forward * ForwardOffset;
					tempPosition = tempForwardPosition;
				}
				else
				{
					tempPosition = tempBackPosition;
				}
			}

#if UNITY_EDITOR
			debugStart = argStart;
			debugEnd = argEnd;
			debugIsCollider = retIsCollision;
#endif

			return retIsCollision;
		}


#if UNITY_EDITOR

		Vector3 debugStart;
		Vector3 debugEnd;
		bool debugIsCollider;

		public void OnDrawGizmos()
		{
			Debug.DrawLine(idealPosition, debugEnd, debugIsCollider ? Color.red : Color.yellow);

			if (camera != null)
			{
				Color color = Color.yellow;
				color.a = 0.1f;
				UnityEditor.Handles.color = color;

				var halfFOV = camera.fieldOfView;
				var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * transform.forward;

				UnityEditor.Handles.DrawSolidArc(transform.position,
					transform.up, beginDirection, camera.fieldOfView * 2, Vector3.Distance(idealPosition, debugEnd));

				Debug.DrawLine(idealPosition +
					transform.forward * camera.nearClipPlane + transform.right * camera.nearClipPlane * ScreenKoef,
					debugEnd, debugIsCollider ? Color.red : Color.yellow);
				Debug.DrawLine(idealPosition +
					transform.forward * camera.nearClipPlane + transform.right * -1 * camera.nearClipPlane * ScreenKoef,
					debugEnd, debugIsCollider ? Color.red : Color.yellow);
			}

			if (debugIsCollider)
			{
				Gizmos.DrawSphere(tempCollision, 0.05f);

				Gizmos.color = colors[0];
				Gizmos.DrawSphere(plane.LowerLeft, 0.05f);
				Gizmos.color = colors[1];
				Gizmos.DrawSphere(plane.LowerRight, 0.05f);
				Gizmos.color = colors[2];
				Gizmos.DrawSphere(plane.UpperLeft, 0.05f);
				Gizmos.color = colors[3];
				Gizmos.DrawSphere(plane.UpperRight, 0.05f);

				Debug.DrawLine(tempBackPosition, debugEnd, Color.blue);
				Debug.DrawLine(tempPosition, debugEnd, Color.green);
			}
		}
#endif


		public ClipPlanePoints NearClipPlanePoints(UnityEngine.Camera camera, Vector3 pos, float clipPlaneMargin)
		{
			var clipPlanePoints = new ClipPlanePoints();

			var transform = camera.transform;
			var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
			var aspect = camera.aspect;
			var distance = camera.nearClipPlane;
			var height = distance * Mathf.Tan(halfFOV);
			var width = height * aspect;
			height *= 1 + clipPlaneMargin;
			width *= 1 + clipPlaneMargin;
			clipPlanePoints.LowerRight = pos + transform.right * width;
			clipPlanePoints.LowerRight -= transform.up * height;
			clipPlanePoints.LowerRight += transform.forward * distance;

			clipPlanePoints.LowerLeft = pos - transform.right * width;
			clipPlanePoints.LowerLeft -= transform.up * height;
			clipPlanePoints.LowerLeft += transform.forward * distance;

			clipPlanePoints.UpperRight = pos + transform.right * width;
			clipPlanePoints.UpperRight += transform.up * height;
			clipPlanePoints.UpperRight += transform.forward * distance;

			clipPlanePoints.UpperLeft = pos - transform.right * width;
			clipPlanePoints.UpperLeft += transform.up * height;
			clipPlanePoints.UpperLeft += transform.forward * distance;

			return clipPlanePoints;
		}

	}

	public struct ClipPlanePoints
	{
		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;
	}


}




