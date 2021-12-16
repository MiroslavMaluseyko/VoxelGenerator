using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position,target.position,Time.deltaTime);
    }
}
