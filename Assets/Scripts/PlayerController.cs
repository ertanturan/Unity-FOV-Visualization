using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 10f;


    private Rigidbody _rb;
    private Camera _cam;

    private Vector3 _mousePosition;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _mousePosition.x = Input.mousePosition.x;
        _mousePosition.y = Input.mousePosition.y;
        //z is the distance in case using perspective camera 
        _mousePosition.z = _cam.transform.position.y;
        Vector3 mousePos = _cam.ScreenToWorldPoint(_mousePosition);

        transform.LookAt(mousePos + Vector3.up * transform.position.y);
        _velocity.x = Input.GetAxisRaw("Horizontal");
        _velocity.z = Input.GetAxisRaw("Vertical");
        _velocity = _velocity.normalized * MoveSpeed;

    }


    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }
}
