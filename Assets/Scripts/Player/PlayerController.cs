using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject m_cameraContainer;

    public float cameraZoomSpeed = 1.0f;
    public float cameraZoomMax = 5.0f;

    public bool m_functionalityEnabled = true;

    public Texture2D DefaultCursor;
    public Texture2D HandCursor;
    public Texture2D AttackCursor;
    public Texture2D TalkCursor;

    private PlayerMovement playerMovement;
    private PlayerPlacing m_playerPlacing;
    private PlayerInteractor m_playerInteractor;
    private PlayerInventory m_playerInventory;
    private PlayerQuests m_playerQuests;
    private Camera m_playerCamera;
    private Vector2 movementInput;
    private bool jumpInput = false;

    private float m_inputDelay;
    private bool m_showInventory = false;
    [SerializeField] private GameObject m_menu;

    private void Awake()
    {
        m_cameraContainer.transform.parent = null;

        playerMovement = GetComponent<PlayerMovement>();
        m_playerPlacing = GetComponent<PlayerPlacing>();
        m_playerInteractor = GetComponent<PlayerInteractor>();
        m_playerInventory = GetComponent<PlayerInventory>();
        m_playerQuests = GetComponent<PlayerQuests>();
        m_playerCamera = GetCamera();
        Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.Auto);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_menu = HUDManager.instance.GetElementByType(typeof(UI_QuestList)).transform.parent.gameObject;
        m_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CursorUpdate();
        HUDInput();

        if (m_functionalityEnabled)
        {
            CameraControl();
            InteractInput();
            MovementInput();
            CombatInput();
            HotbarInput();
            // Call movement function
            playerMovement.Move(movementInput);
        }
    }

    private void FixedUpdate()
    {
        // Set jump input to off
        jumpInput = false;
    }

    public void CursorUpdate()
    {
        RaycastHit hit;
        Ray ray = m_playerCamera.ScreenPointToRay(InputManager.instance.GetMousePositionInScreen());
        if (Physics.Raycast(ray, out hit, 1000))
        {
            if (hit.collider.tag == "Interactable" || hit.collider.tag == "SerializedObject")
            {
                if (hit.collider.GetComponent<WaterFiller>() != null && m_playerInventory.GetSelectItem()?.GetToolType() == ToolType.WaterCan)
                {
                    if(m_playerInteractor.Contains(hit.collider.gameObject))
                    {
                        Cursor.SetCursor(HandCursor, Vector2.zero, CursorMode.Auto);
                        return;
                    }
                    return;
                }
                if (hit.collider.GetComponent<CropScript>() != null && m_playerInventory.GetSelectItem()?.GetToolType() != ToolType.Null)
                {
                    if(m_playerInteractor.Contains(hit.collider.gameObject))
                    {
                        Cursor.SetCursor(HandCursor, Vector2.zero, CursorMode.Auto);
                        return;
                    }
                    return;
                }
            }
            if (hit.collider.tag == "NPC")
            {
                if (m_playerInteractor.Contains(hit.collider.gameObject))
                {
                    Cursor.SetCursor(TalkCursor, Vector2.zero, CursorMode.Auto);
                    return;
                }
            }

            if (hit.collider.tag == "Enemy")
            {
                if (m_playerInventory.GetSelectItem()?.GetToolType() == ToolType.Shovel)
                {
                    Cursor.SetCursor(AttackCursor, Vector2.zero, CursorMode.Auto);
                    return;
                }
            }
        }

        Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.Auto);
        return;
    }

    private void CameraControl()
    {
        m_cameraContainer.transform.position = Vector3.Lerp(m_cameraContainer.transform.position, transform.position, 1 - Mathf.Pow(2.0f, -Time.deltaTime * 5.0f));

        Camera playerCamera = GetCamera();

        // Camera zoom
        playerCamera.orthographicSize = Mathf.Clamp(playerCamera.orthographicSize - InputManager.instance.GetMouseScrollDelta() * cameraZoomSpeed * Time.deltaTime, 1, cameraZoomMax);
    }

    public Camera GetCamera()
    {
        return m_cameraContainer.GetComponentInChildren<Camera>();
    }

    private void MovementInput()
    {
        movementInput = new Vector2();

        // Move Left
        movementInput.x -= InputManager.instance.IsKeyPressed(KeyType.A) ? 1.0f : 0.0f;

        // Move Right
        movementInput.x += InputManager.instance.IsKeyPressed(KeyType.D) ? 1.0f : 0.0f;

        // Move Forwards
        movementInput.y += InputManager.instance.IsKeyPressed(KeyType.W) ? 1.0f : 0.0f;

        // Move Backwards
        movementInput.y -= InputManager.instance.IsKeyPressed(KeyType.S) ? 1.0f : 0.0f;

        // Get jump input
        if (!jumpInput)
            jumpInput = InputManager.instance.IsKeyDown(KeyType.SPACE);
    }

    private void HotbarInput()
    {
        for (int i = 0; i < 5; i++)
        {
            if (InputManager.instance.IsKeyDown(KeyType.ALP_ONE + i))
            {
                m_playerPlacing.SetSelectedIndex(i);
            }
        }
    }

    private void InteractInput()
    {
        if (InputManager.instance.GetMouseButtonDown(MouseButton.LEFT))
        {
            RaycastHit hit;
            Ray ray = m_playerCamera.ScreenPointToRay(InputManager.instance.GetMousePositionInScreen());
            if (Physics.Raycast(ray, out hit, 1000)) 
            {
                if(m_playerInteractor.Contains(hit.collider.gameObject))
                {
                    hit.collider.GetComponent<Interactable>().Interact();
                }

                //if (hit.collider.tag == "Interactable" || hit.collider.tag == "SerializedObject")
                //{
                //    if (hit.collider.GetComponent<WaterFiller>() != null && m_playerInventory.GetSelectItem()?.GetToolType() == ToolType.WaterCan)
                //    {
                //        Cursor.SetCursor(HandCursor, Vector2.zero, CursorMode.Auto);
                //        return;
                //    }
                //    if (hit.collider.GetComponent<CropScript>() != null && m_playerInventory.GetSelectItem()?.GetToolType() != ToolType.Null)
                //    {
                //        Cursor.SetCursor(HandCursor, Vector2.zero, CursorMode.Auto);
                //        return;
                //    }
                //}
            }
        }
    }

    private void CombatInput()
    {
        if (InputManager.instance.GetMouseButtonDown(MouseButton.LEFT))
        {
            playerMovement.SlamAttack();
        }
        if (InputManager.instance.GetMouseButtonDown(MouseButton.RIGHT))
        {
            playerMovement.SwingAttack();
        }
    }
    private void HUDInput()
    {
        if (m_inputDelay > 0.0f)
        {
            m_inputDelay -= Time.deltaTime;
            return;
        }

        if (InputManager.instance.IsKeyDown(KeyType.I) && m_inputDelay <= 0)
        {
            m_inputDelay = 0.25f;
            m_showInventory = !m_showInventory;

            if (m_showInventory)
            {
                m_functionalityEnabled = false;
                m_playerInventory.GenerateOnDisplay(true);
                m_playerQuests.GenerateOnDisplay(false);
            }
            else
            {
                m_functionalityEnabled = true;
            }

            m_menu.SetActive(m_showInventory);
        }
        if (InputManager.instance.IsKeyDown(KeyType.O) && m_inputDelay <= 0)
        {
            m_inputDelay = 0.25f;
            m_showInventory = !m_showInventory;

            if (m_showInventory)
            {
                m_playerInventory.GenerateOnDisplay(false);
                m_playerQuests.GenerateOnDisplay(true);
                m_functionalityEnabled = false;
            }
            else
            {
                m_functionalityEnabled = true;
            }

            m_menu.SetActive(m_showInventory);
        }
    }
}
