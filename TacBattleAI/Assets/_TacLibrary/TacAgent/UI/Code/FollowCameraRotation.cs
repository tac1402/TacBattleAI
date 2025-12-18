using UnityEngine;

public class FollowCameraRotation : MonoBehaviour
{
    [SerializeField] Transform target;

    void Start()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }
    }

    void LateUpdate()
    {

        Vector3 direction = transform.position + target.forward;
        //direction.y = transform.position.y;
		transform.LookAt(direction);
    }
}
