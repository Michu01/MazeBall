using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float mouseSpeed = 100f;
    public float mouseScrollSpeed = 1000f;

    private Vector3 rotation = new();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");

        rotation += Time.deltaTime * (mouseSpeed * new Vector3(y, 0, -x) + mouseScrollSpeed * new Vector3(0, Input.mouseScrollDelta.y, 0));
    }

    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.Euler(rotation);
    }
}
