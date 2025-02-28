using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float sprintMultiplier = 2f;
    public float mouseSensitivity = 2f;
    
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Update()
    {
        // Движение игрока
        float moveX = Input.GetAxis("Horizontal"); // A / D
        float moveY = (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0); // E / Q
        float moveZ = Input.GetAxis("Vertical"); // W / S
        
        float currentSpeed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        Vector3 move = transform.right * moveX + transform.up * moveY + transform.forward * moveZ;
        transform.position += move * currentSpeed * Time.deltaTime;

        // Управление камерой (осмотр)
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }
}
