using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovemnt : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Camera _camera;
    [SerializeField] float minOrtoSize;
    [SerializeField] float maxOrtoSize;

    void Update()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.up;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.down;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
        if (Input.mouseScrollDelta.y != 0) _camera.orthographicSize = Mathf.Min(Mathf.Max(minOrtoSize, Input.mouseScrollDelta.y + _camera.orthographicSize), maxOrtoSize);

        transform.Translate(direction * speed * Time.deltaTime);
    }
}
