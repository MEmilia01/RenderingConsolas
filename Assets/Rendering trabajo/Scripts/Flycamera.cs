using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 10f;        
    public float sprintMultiplier = 3f;  
    public float smoothTime = 0.1f;      

    [Header("Ratón")]
    public float mouseSensitivity = 2f;
    public float maxPitch = 89f;

    private float yaw;
    private float pitch;
    private Vector3 velocity;        
    private Vector3 velocitySmooth;  
                                     
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x > 180f ? angles.x - 360f : angles.x;

        SetCursorLocked(true);
    }

    void Update()
    {
        // Esc libera/bloquea el cursor
        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursorLocked(Cursor.lockState != CursorLockMode.Locked);

        if (Cursor.lockState == CursorLockMode.Locked)
            Look();

        Move();
    }

    void Look()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void Move()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) dir += transform.forward;
        if (Input.GetKey(KeyCode.S)) dir -= transform.forward;
        if (Input.GetKey(KeyCode.D)) dir += transform.right;
        if (Input.GetKey(KeyCode.A)) dir -= transform.right;

        // Subir / bajar en el eje Y del mundo
        if (Input.GetKey(KeyCode.Space)) dir += Vector3.up;
        if (Input.GetKey(KeyCode.LeftControl)) dir += Vector3.down;

        if (dir.sqrMagnitude > 1f) dir.Normalize();

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= sprintMultiplier;

        Vector3 targetVelocity = dir * speed;

        // Suavizado de aceleración/frenado
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref velocitySmooth, smoothTime);

        transform.position += velocity * Time.deltaTime;
    }

    void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}