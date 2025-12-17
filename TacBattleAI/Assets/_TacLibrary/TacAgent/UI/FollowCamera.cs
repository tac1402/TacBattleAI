using UnityEngine;


/// <summary>
/// Поворачивает UI по направлению к камере
/// </summary>
public class FollowCamera : MonoBehaviour
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
