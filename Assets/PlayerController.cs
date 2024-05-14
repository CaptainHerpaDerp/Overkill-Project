using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float sensitivityX, sensitivityY;

    public Transform orientation;

    float xRotation, yRotation;

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Hide the cursor
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal and vertical input
        float horizontalInput = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivityX;
        float verticalInput = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivityY;

        yRotation += horizontalInput;
        xRotation -= verticalInput;

        // Clamp the xRotation to prevent the player from looking behind them
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
