using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacing : MonoBehaviour
{
    private Camera m_PlayerCamera;
    public GameObject m_SelectedObject;
    public GameObject m_TargetingArea;
    private float m_CurrentRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_SelectedObject != null)
        {
            m_TargetingArea.GetComponent<MeshFilter>().sharedMesh = m_SelectedObject.GetComponent<MeshFilter>().sharedMesh;

            RaycastHit hit;
            Ray ray = m_PlayerCamera.ScreenPointToRay(InputManager.instance.GetMousePositionInScreen());
            

            if (InputManager.instance.GetMouseButtonDown(MouseButton.RIGHT)) // Need to check for valid placement ground
            {
                m_CurrentRotation += 90.0f;
                if (m_CurrentRotation > 360.0f)
                {
                    m_CurrentRotation -= 360.0f;
                }
            }
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 TargetPoint = hit.point;
                m_TargetingArea.transform.position = TargetPoint;
                if (hit.transform.gameObject.layer == 8)
                {
                    if (InputManager.instance.GetMouseButtonDown(MouseButton.LEFT)) // Need to check for valid placement ground
                    {
                        Instantiate(m_SelectedObject, TargetPoint, Quaternion.Euler(0, m_CurrentRotation, 0));
                        //m_SelectedObject = null;
                    }
                }
            }
        }
    }

    public void SetSelectedObject(GameObject _Prefab)
    {
        m_SelectedObject = _Prefab;
    }
}
