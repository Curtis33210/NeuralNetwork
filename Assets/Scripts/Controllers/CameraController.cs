using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private int _moveSpeed, _zoomSpeed, _dragSpeed;

    [SerializeField]
    private bool _invertX, _invertY;

    private Vector3 _lastFramePos;
    
    private void Update()
    {
        Vector2 movementVector = Vector2.zero;
        Camera camera = Camera.main;

        var orthogSize = camera.orthographicSize;

        orthogSize -= camera.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;

        camera.orthographicSize = Mathf.Clamp(orthogSize, 1, Mathf.Infinity);
        
        if (Input.GetMouseButton(2)) {
            Vector3 currPosition = camera.ScreenToWorldPoint(Input.mousePosition);

            camera.transform.Translate(_lastFramePos - currPosition);
        }

        _lastFramePos = camera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKey(KeyCode.W))
            movementVector += new Vector2(0, 1);
        if (Input.GetKey(KeyCode.S))
            movementVector += new Vector2(0, -1);
        if (Input.GetKey(KeyCode.A))
            movementVector += new Vector2(-1, 0);
        if (Input.GetKey(KeyCode.D))
            movementVector += new Vector2(1, 0);
        
        camera.transform.Translate(movementVector.normalized * Time.deltaTime * _moveSpeed);
    }
}