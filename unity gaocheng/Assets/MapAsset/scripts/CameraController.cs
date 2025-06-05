using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 5f; // 缩放速度
    public float minZoom = 5f;   // 最小缩放值
    public float maxZoom = 20f;  // 最大缩放值

    public float dragSpeed = 2f; // 拖动速度

    public Vector2 minBounds ; // 拖动的最小边界
    public Vector2 maxBounds; // 拖动的最大边界

    private Vector3 dragOrigin; // 记录鼠标拖动的起始位置


    void Update()
    {
        HandleZoom();
        HandleDrag();
    }

    private void HandleZoom()
    {
        // 获取鼠标滚轮输入
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
    }

    private void HandleDrag()
    {
        // 鼠标左键按下时记录起始位置
        if (Input.GetMouseButtonDown(0)) // 左键
        {
            dragOrigin = Input.mousePosition;
      
        }

        // 鼠标左键拖动时移动摄像头
        if (Input.GetMouseButton(0))
        {
            if (Vector3.Distance(dragOrigin, Input.mousePosition) > 1f) // 判断是否拖动
            {
                Vector3 difference = Camera.main.ScreenToWorldPoint(dragOrigin) - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 newPosition = transform.position + difference;

                // 限制摄像头位置在边界范围内
                newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
                newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

                transform.position = newPosition;
                dragOrigin = Input.mousePosition;
  
            }
        }

        // 鼠标左键抬起时重置拖动状态
        if (Input.GetMouseButtonUp(0))
        {

        }
    }
}
