using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    private Transform m_Focus;
    [SerializeField, Min(0f)]
    private float m_FocusRadius = 1f;
    [SerializeField, Range(0f, 1f)]
    private float m_FocusCentering = 0.5f;

    [SerializeField, Range(1f, 50f)]
    private float m_Zoom = 10.0f;
    [SerializeField, Min(1f)]
    private float m_ZoomSpeed = 10f;

    [SerializeField, Min(0f)]
    private float m_RotationSpeed = 90f;

    private Vector2 m_OrbitAngles = new Vector2(45f, 0f);

    private Vector3 m_FocalPoint;

    private void InputRotation()
    {
        float vertical = Input.GetAxis("Vertical Camera");
        float horiz = Input.GetAxis("Horizontal Camera");
        Vector2 input = new Vector2(vertical, horiz);

        const float minChange = 0.001f;

        if(input.x < -minChange
            || input.x > minChange
            || input.y < -minChange
            || input.y > minChange)
        {
            m_OrbitAngles += m_RotationSpeed * Time.deltaTime * input;
        }
    }

    private void InputZoom()
    {
        if(Input.mouseScrollDelta.y == 0f)
          return;

        float zoomDelta = m_ZoomSpeed * Input.mouseScrollDelta.y * Time.deltaTime;

        m_Zoom += zoomDelta;
        if(m_Zoom > 50f)
          m_Zoom = 50f;
        else if(m_Zoom < 1f)
          m_Zoom = 1f;
    }

    private void UpdateFocalPoint()
    {
        Vector3 targetPos = m_Focus.position;
        if(m_FocusRadius > 0)
        {
            float dist = Vector3.Distance(targetPos, m_FocalPoint);
            float t = 1f;
            if(dist > 0.01f && m_FocusCentering > 0f)
            {
                t = Mathf.Pow(1f - m_FocusCentering, Time.deltaTime);
            }
            if(dist > m_FocusRadius)
            {
                t = Mathf.Min(t, m_FocusRadius / dist);
            }

            m_FocalPoint = Vector3.Lerp(targetPos, m_FocalPoint, t);
        } 
        else 
        {
            m_FocalPoint = targetPos;
        }
    }

    private void Awake()
    {
        m_FocalPoint = m_Focus.position;
    }

    private void LateUpdate()
    {
        UpdateFocalPoint();
        InputRotation();
        InputZoom();

        Quaternion lookRot = Quaternion.Euler(m_OrbitAngles);
        Vector3 lookDir = lookRot * Vector3.forward;
        Vector3 lookPos = m_FocalPoint - lookDir * m_Zoom;

        transform.SetPositionAndRotation(lookPos, lookRot);
    }
}
