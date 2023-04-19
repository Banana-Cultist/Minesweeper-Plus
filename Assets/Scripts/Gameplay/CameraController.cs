using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float zoomSpeed;
    public float zoomFactor;
    private float zoomAmount = 0;
    public RectTransform board;
    public float maxBoardZoom;

    private Camera cam;
    public bool movable = true;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        zoomAmount = Mathf.Log(Mathf.Min(
            board.rect.width / Screen.width * Screen.height,
            board.rect.height
        ), zoomFactor);
    }

    // Update is called once per frame
    void Update()
    {
        if (!movable) return;

        float nextX = Mathf.Clamp(
            transform.position.x + (Input.GetAxis("Horizontal") * Time.deltaTime * cam.orthographicSize * moveSpeed),
            maxBoardZoom * board.rect.xMin + (cam.orthographicSize * Screen.width / Screen.height),
            maxBoardZoom * board.rect.xMax - (cam.orthographicSize * Screen.width / Screen.height)
        );
        float nextY = Mathf.Clamp(
            transform.position.y + (Input.GetAxis("Vertical") * Time.deltaTime * cam.orthographicSize * moveSpeed),
            maxBoardZoom * board.rect.yMin + cam.orthographicSize,
            maxBoardZoom * board.rect.yMax - cam.orthographicSize
        );

        transform.position = new Vector3(nextX, nextY, transform.position.z);


        float maxZoom = Mathf.Log(
            Mathf.Min(
                board.rect.width / Screen.width * Screen.height,
                board.rect.height
            ) * maxBoardZoom / 2,
            zoomFactor
        );

        zoomAmount = Mathf.Min(zoomAmount + Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed, maxZoom);

        cam.orthographicSize = Mathf.Pow(zoomFactor, zoomAmount);
    }
}
