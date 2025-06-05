using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 5f; // �����ٶ�
    public float minZoom = 5f;   // ��С����ֵ
    public float maxZoom = 20f;  // �������ֵ

    public float dragSpeed = 2f; // �϶��ٶ�

    public Vector2 minBounds ; // �϶�����С�߽�
    public Vector2 maxBounds; // �϶������߽�

    private Vector3 dragOrigin; // ��¼����϶�����ʼλ��


    void Update()
    {
        HandleZoom();
        HandleDrag();
    }

    private void HandleZoom()
    {
        // ��ȡ����������
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
    }

    private void HandleDrag()
    {
        // ����������ʱ��¼��ʼλ��
        if (Input.GetMouseButtonDown(0)) // ���
        {
            dragOrigin = Input.mousePosition;
      
        }

        // �������϶�ʱ�ƶ�����ͷ
        if (Input.GetMouseButton(0))
        {
            if (Vector3.Distance(dragOrigin, Input.mousePosition) > 1f) // �ж��Ƿ��϶�
            {
                Vector3 difference = Camera.main.ScreenToWorldPoint(dragOrigin) - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 newPosition = transform.position + difference;

                // ��������ͷλ���ڱ߽緶Χ��
                newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
                newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

                transform.position = newPosition;
                dragOrigin = Input.mousePosition;
  
            }
        }

        // ������̧��ʱ�����϶�״̬
        if (Input.GetMouseButtonUp(0))
        {

        }
    }
}
