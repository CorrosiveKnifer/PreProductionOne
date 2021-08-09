using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject m_cameraContainer;

    public float cameraZoomSpeed = 1.0f;
    public float cameraZoomMax = 5.0f;

    private PlayerMovement playerMovement;
    private PlayerPlacing m_playerPlacing;
    private PlayerInteractor m_playerInteractor;
    private PlayerInventory m_playerInventory;
    private PlayerQuests m_playerQuests;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        m_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CameraControl();
        MovementInput();
        HotbarInput();
        InteractInput();
        HUDInput();
        CombatInput();

        // Call movement function
        playerMovement.Move(movementInput);
    }

    private void FixedUpdate()
    {

        // Set jump input to off
        jumpInput = false;
    }

    private void CameraControl()
    {
        m_cameraContainer.transform.position = Vector3.Lerp(m_cameraContainer.transform.position, transform.position, 1 - Mathf.Pow(2.0f, -Time.deltaTime * 5.0f));

        Camera playerCamera = m_cameraContainer.GetComponentInChildren<Camera>();

        // Camera zoom
        playerCamera.orthographicSize = Mathf.Clamp(playerCamera.orthographicSize - InputManager.instance.GetMouseScrollDelta() * cameraZoomSpeed * Time.deltaTime, 1, cameraZoomMax);
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
                m_playerInventory.SelectItem(i);
            }
        }
    }

    private void InteractInput()
    {
        if (InputManager.instance.IsKeyDown(KeyType.E))
        {
            m_playerInteractor.InteractWithObject();
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

            if(m_showInventory)
            {
                m_playerInventory.GenerateOnDisplay(true);
                m_playerQuests.GenerateOnDisplay(false);
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
            }

            m_menu.SetActive(m_showInventory);

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
}
