using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacing : MonoBehaviour
{
    public Material m_validMaterial;
    public Material m_invalidMaterial;

    private Camera m_PlayerCamera;
    private GameObject m_SelectedObject;
    public GameObject m_TargetingArea;
    private PlayerInventory m_PlayerInventory;
    private float m_CurrentRotation = 0;
    private int m_SelectedIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerCamera = GetComponent<PlayerController>().GetCamera();
        m_PlayerInventory = GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_SelectedObject != null && m_TargetingArea.GetComponent<TargetArea>())
        {
            m_TargetingArea.GetComponent<MeshFilter>().sharedMesh = m_SelectedObject.GetComponent<MeshFilter>().sharedMesh;
            m_TargetingArea.GetComponent<MeshCollider>().sharedMesh = m_SelectedObject.GetComponent<MeshFilter>().sharedMesh;
            m_TargetingArea.transform.localScale = m_SelectedObject.GetComponent<MeshFilter>().transform.lossyScale;

            if (m_TargetingArea.GetComponent<TargetArea>().HasObstructions())
            {
                m_TargetingArea.GetComponent<MeshRenderer>().material = m_invalidMaterial;
            }
            else
            {
                m_TargetingArea.GetComponent<MeshRenderer>().material = m_validMaterial;
            }

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
            if (Physics.Raycast(ray, out hit, 1000, 1 << 8))
            {
                m_TargetingArea.SetActive(true);

                Vector3 TargetPoint = hit.point;
                TargetPoint.x = Mathf.Round(TargetPoint.x);
                TargetPoint.z = Mathf.Round(TargetPoint.z);

                m_TargetingArea.transform.position = TargetPoint;
                if (hit.transform.gameObject.layer == 8 && !m_TargetingArea.GetComponent<TargetArea>().HasObstructions())
                {
                    if (InputManager.instance.GetMouseButtonDown(MouseButton.LEFT)) // Need to check for valid placement ground
                    {
                        Instantiate(m_SelectedObject, TargetPoint, Quaternion.Euler(0, m_CurrentRotation, 0));
                        m_PlayerInventory.UseItem(m_SelectedIndex);
                        SetSelectedIndex(m_SelectedIndex);
                    }
                }
            }
            else
            {
                m_TargetingArea.SetActive(false);
            }
        }
        else
        {
            m_TargetingArea.GetComponent<MeshFilter>().sharedMesh = null;
        }
    }

    public void SetSelectedIndex(int _index)
    {
        if(_index == -1)
        {
            m_SelectedIndex = -1;
            m_SelectedObject = null;
            m_PlayerInventory.SelectItem(-1);
            return;
        }

        if (m_SelectedIndex != _index)
        {
            m_SelectedIndex = _index;
            m_PlayerInventory.SelectItem(_index);
            ItemObject item = m_PlayerInventory.GetItemFromHotbar(_index);
            if (item != null)
                m_SelectedObject = item.GetPlaceObject();
            else
                m_SelectedObject = null;
        }
        else
        {
            m_PlayerInventory.SelectItem(-1);
            m_SelectedIndex = -1;
            m_SelectedObject = null;
        }
    }
}
