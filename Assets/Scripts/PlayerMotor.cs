using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{   
    [SerializeField]
    private Camera cam;

    private Vector3 velocity;
    private Vector3 rotation;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterVelocity;

    [SerializeField]
    private float cameraRotationLimit = 85f;
    private Rigidbody rb;
    
    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move( Vector3 _velocity) 
    {
        velocity = _velocity;
    }

    public void Rotate( Vector3 _rotation) 
    {
        rotation = _rotation;
    }

    public void RotateCamera( float _cameraRotationX) 
    {
        cameraRotationX = _cameraRotationX;
    }
    public void ApplyThruster(Vector3 _thrusterVelocity) 
    {
        thrusterVelocity = _thrusterVelocity;
    }

    // Delay between each update is fixed not relative to FPS
    private void FixedUpdate() {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement() {
        if(velocity != Vector3.zero) 
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(thrusterVelocity != Vector3.zero) 
        {
            rb.AddForce(thrusterVelocity * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    private void PerformRotation() {
        // Quaternion.Euler transform Vector3 to Quaternion required to Rotate
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        // Calculate camera Rotation on X axis
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        // Apply camera Rotation
        cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}
