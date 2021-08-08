using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject m_cameraContainer;

    public float cameraZoomSpeed = 1.0f;
    public float cameraZoomMax = 5.0f;

    public bool m_functionalityEnabled = true;

    private PlayerMovement playerMovement;
    private PlayerPlacing m_playerPlacing;
    private PlayerInteractor m_playerInteractor;
    private PlayerInventory m_playerInventory;
    private Vector2 movementInput;
    private bool jumpInput = false;

    private void Awake()
    {
        m_cameraContainer.transform.parent = null;

        playerMovement = GetComponent<PlayerMovement>();
        m_playerPlacing = GetComponent<PlayerPlacing>();
        m_playerInteractor = GetComponent<PlayerInteractor>();
        m_playerInventory = GetComponent<PlayerInventory>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraControl();
        HotbarInput();
        InteractInput();

        if (m_functionalityEnabled)
        {
            MovementInput();
            CombatInput();

            // Call movement function
            playerMovement.Move(movementInput);
        }

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
