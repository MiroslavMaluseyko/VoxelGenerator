using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        _rigidbody.velocity = new Vector3(0,_rigidbody.velocity.y,0);
        if (Input.GetKey(KeyCode.W))
            _rigidbody.velocity += Vector3.forward*speed;
        if (Input.GetKey(KeyCode.S))
            _rigidbody.velocity += Vector3.back*speed;
        if (Input.GetKey(KeyCode.D))
            _rigidbody.velocity += Vector3.right*speed;
        if (Input.GetKey(KeyCode.A))
            _rigidbody.velocity += Vector3.left*speed;
        if (Input.GetKeyDown(KeyCode.Space))
            _rigidbody.velocity += Vector3.up * speed;
    }
}
