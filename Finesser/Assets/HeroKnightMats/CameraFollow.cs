using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    //public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float damping;

    private Vector3 velocity = Vector3.zero;
    private void LateUpdate()
    {
        Vector3 movePosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);
    }


}
