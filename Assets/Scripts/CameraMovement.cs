using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float edgePanSpeed = 10f;
    public int edgeSize = 20; // pixels from edge to trigger pan
    public float drag = 8f; // higher = snappier, lower = smoother

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        float h = 0f;
        float v = 0f;

        // Keyboard movement
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h += 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v -= 1f;

            // Center camera on C key
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                transform.position = new Vector3(0f, transform.position.y, 0f);
                velocity = Vector3.zero;
            }
        }

        // Mouse edge movement
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            if (mousePos.x <= edgeSize) h -= 1f;
            if (mousePos.x >= screenWidth - edgeSize) h += 1f;
            if (mousePos.y >= screenHeight - edgeSize) v += 1f;
            if (mousePos.y <= edgeSize) v -= 1f;
        }

        Vector3 inputDir = new Vector3(h, 0, v);
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 moveDir = (forward * inputDir.z + right * inputDir.x);

        // Smooth movement with drag
        Vector3 targetVelocity = moveDir.normalized * (inputDir.sqrMagnitude > 0 ? moveSpeed : 0f);
        velocity = Vector3.Lerp(velocity, targetVelocity, drag * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;

        // Clamp camera position to X/Z limits
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -100f, 100f);
        pos.z = Mathf.Clamp(pos.z, -100f, 100f);
        transform.position = pos;
    }
}
