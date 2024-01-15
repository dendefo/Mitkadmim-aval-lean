using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovemnt : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float AngularSpeed;
    [SerializeField] Camera _camera;
    [SerializeField] float minOrtoSize;
    [SerializeField] float maxOrtoSize;

    void Update()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += transform.up;
        if (Input.GetKey(KeyCode.S)) direction += -transform.up;
        if (Input.GetKey(KeyCode.A)) direction += -transform.right;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        if (Input.GetKey(KeyCode.Q)) _camera.transform.Rotate(Vector3.up * AngularSpeed * Time.timeScale, Space.World);
        if (Input.GetKey(KeyCode.E)) _camera.transform.Rotate(Vector3.down * AngularSpeed * Time.timeScale, Space.World);
        if (Input.mouseScrollDelta.y != 0) _camera.orthographicSize = Mathf.Min(Mathf.Max(minOrtoSize, -Input.mouseScrollDelta.y + _camera.orthographicSize), maxOrtoSize);
        if (Input.GetKey(KeyCode.LeftShift)) direction *= 5;
        transform.parent.Translate(direction * speed * Time.deltaTime);
        
    }
}
