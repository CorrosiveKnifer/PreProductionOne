using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerPlacing m_playerPlacing;
    private PlayerInteractor m_playerInteractor;
    private Vector2 movementInput;
    private bool jumpInput = false;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        m_playerPlacing = GetComponent<PlayerPlacing>();
        m_playerInteractor = GetComponent<PlayerInteractor>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MovementInput();
        HotbarInput();
        InteractInput();
    }
    private void FixedUpdate()
    {
        // Call movement function
        playerMovement.Move(movementInput, jumpInput);

        // Set jump input to off
        jumpInput = false;
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
            if (InputManager.instance.IsKeyDown(KeyType.NUM_ONE + i))
            {
                m_playerPlacing.SetSelectedIndex(i);
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
}
