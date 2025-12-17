// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tac.Camera
{
    public class FastController : MonoBehaviour
    {
        public Vector3 Speed;
        public bool AllowMove = true;

        public bool UseLimit;
        public LimitType Limit;

        public Vector2 TopWorld;
        public Vector2 SizeWorld;

		/// <summary>
		/// Настоящая структура террайна, может использоваться .SampleHeight() чтобы получить именно высоту террайна, а не соответствующего слоя
		/// </summary>
		public Terrain WorldTerrain;
		
        public List<LimitCircle> WorldLimit;

        public UnityEngine.Camera CurrentCamera;
        private TopCamera currentFastCamera;

        public Transform CameraTarget;

        //public NotificationTrigger NotificationT;

        void Start()
        {
            //NotificationT = UI.GetComponent<NotificationTrigger>();
        }


        public float ZoomY = 50;
        public float ZoomRate = 10;
        public Vector2 ZoomLimit = Vector2.zero;
        private float ZoomChangeY = 0;

        public Light light;
        public Vector2 LightIntensity = Vector2.zero;

        void Update()
        {
            //if (IsPointerOverUI() == false)
            {
                float mouseScrollWheel = InputScroll.y; // Input.GetAxisRaw("Mouse ScrollWheel");
                float zoomChange = 0.0f;
                if (mouseScrollWheel != 0.0f)
                {
                    if (mouseScrollWheel > 0.0) zoomChange = -1;
                    else zoomChange = 1;
                }

                ZoomChangeY += zoomChange * ZoomRate * Time.unscaledDeltaTime;
                if (ZoomLimit != Vector2.zero)
                {
                    if (ZoomChangeY < ZoomLimit.x)
                    {
                        ZoomChangeY = ZoomLimit.x;
                    }
                    else if (ZoomChangeY > ZoomLimit.y)
                    {
                        ZoomChangeY = ZoomLimit.y;
                    }
                }

                Speed.y = ZoomY + ZoomChangeY;
            }

            float currentHeight = WorldTerrain.SampleHeight(transform.position);
            transform.position = new Vector3(transform.localPosition.x, Speed.y + currentHeight, transform.localPosition.z);

            if (light != null)
            {
                float p = ZoomLimit.y - ZoomLimit.x;
                int s = 1; if (ZoomLimit.x < 0) { s = -1; }
                float t = (ZoomLimit.x * s + ZoomChangeY) / p;
                float l = Mathf.Lerp(LightIntensity.x, LightIntensity.y, t);
                light.intensity = l;
            }

            if (AllowMove)
            {
                if (currentFastCamera == null)
                {
                    if (CurrentCamera == null)
                    {
                        CurrentCamera = UnityEngine.Camera.main;
                    }
                    if (CurrentCamera != null)
                    {
                        currentFastCamera = CurrentCamera.gameObject.GetComponent<TopCamera>();
                    }
                }

                if (currentFastCamera != null)
                {
                    transform.rotation = Quaternion.Euler(currentFastCamera.CurrentCoord.x, currentFastCamera.CurrentCoord.y, 0);

                    Vector2 newPosition;
                    if (currentFastCamera.IsCollision != 0)
                    {
                        newPosition = new Vector2(InputMove.x * -2 * Speed.x * Time.unscaledDeltaTime,
                            InputMove.y * -2 * Speed.x * Time.unscaledDeltaTime);
                        //NotificationT.AddNotification(Notification.AvoidCollisions);
                    }
                    else
                    {
                        newPosition = new Vector2(InputMove.x * Speed.x * Time.unscaledDeltaTime,
							InputMove.y * Speed.x * Time.unscaledDeltaTime);
                    }

                    if (newPosition.x != 0 || newPosition.y != 0)
                    {
                        float inputModifyFactor = (newPosition.x != 0.0f && newPosition.y != 0.0f) ? .7071f : 1.0f;

                        Vector3 newPosition2 = new Vector3(newPosition.x * inputModifyFactor, 0, newPosition.y * inputModifyFactor);

                        Vector3 nextPosition = transform.rotation * newPosition2 + transform.position;

                        float y = WorldTerrain.SampleHeight(nextPosition);

                        if (UseLimit == true)
                        {
                            if (Limit == LimitType.Square)
                            {
                                if (nextPosition.x >= TopWorld.x && nextPosition.x <= TopWorld.x + SizeWorld.x &&
                                    nextPosition.z >= TopWorld.y && nextPosition.z <= TopWorld.y + SizeWorld.y)
                                {
                                    transform.Translate(newPosition2);
                                    transform.position = new Vector3(transform.position.x, Speed.y + y, transform.position.z);
                                }
                                else
                                {
                                    if (nextPosition.x < TopWorld.x) { transform.position = new Vector3(TopWorld.x, transform.position.y, transform.position.z); }
                                    else if (nextPosition.x > TopWorld.x + SizeWorld.x) { transform.position = new Vector3(TopWorld.x + SizeWorld.x, transform.position.y, transform.position.z); }
                                    else if (nextPosition.y < TopWorld.y) { transform.position = new Vector3(transform.position.x, transform.position.y, TopWorld.y); }
                                    else if (nextPosition.y > TopWorld.y + SizeWorld.y) { transform.position = new Vector3(transform.position.x, transform.position.y, TopWorld.y + SizeWorld.y); }
                                }
                            }

                            if (Limit == LimitType.Сircle)
                            {
                                bool allowMove = true;
                                for (int i = 0; i < WorldLimit.Count; i++)
                                {
                                    Vector2 p = new Vector2(nextPosition.x, nextPosition.z);
                                    float d = Vector2.Distance(p, WorldLimit[i].Center);

                                    if (d >= WorldLimit[i].Radius)
                                    {
                                        allowMove = false;
                                        break;
                                    }
                                }

                                if (allowMove)
                                {
                                    transform.Translate(newPosition2);
                                    transform.position = new Vector3(transform.position.x, Speed.y + y, transform.position.z);
                                }
                                else
                                {
                                    //NotificationT.AddNotification(Notification.UnknownTerritory);
                                }
                            }
                        }
                        else
                        {
                            transform.Translate(newPosition2);
                            transform.position = new Vector3(transform.position.x, Speed.y + y, transform.position.z);
                        }
                    }
                }
            }
        }


		/*public bool IsPointerOverUI()
		{
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

            Vector3 mousePosition = Mouse.current.position.ReadValue();
			eventDataCurrentPosition.position = new Vector2(mousePosition.x, mousePosition.y);
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			return results.Count > 0;
		}*/

		public Vector2 InputMove = Vector2.zero;
		public Vector2 InputLook = Vector2.zero;
		public Vector2 InputScroll = Vector2.zero;

		public void OnMove(InputAction.CallbackContext value)
        {
			InputMove = value.ReadValue<Vector2>();
		}
		public void OnLook(InputAction.CallbackContext value)
		{
            InputLook = value.ReadValue<Vector2>();
		}

        public void OnScroll(InputAction.CallbackContext value)
        {
			InputScroll = value.ReadValue<Vector2>();
		}
	}

	[System.Serializable]
    public enum LimitType
    {
        Square,
        Сircle
    }

    [System.Serializable]
    public class LimitCircle
    {
        public Vector2 Center;
        public float Radius;
    }
}
