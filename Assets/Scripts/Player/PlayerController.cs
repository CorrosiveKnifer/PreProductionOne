using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum PlayerState
    {
        DEFAULT,
        ATTACKING,
        CARRYING,
    }

    public struct PlayerAnimationModel
    {
        public Animator animator;
        public SkinnedMeshRenderer[] renderers { get; private set; }
        public bool isActive { get; private set; }

        public void SetAnimator(Animator _animator)
        {
            animator = _animator;
            renderers = animator.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            SetActive(false);
        }

        public void SetActive(bool _active)
        {
            isActive = _active;
            foreach (var renderer in renderers)
            {
                renderer.enabled = _active;
            }
        }
    }

    [Header("Player Animators")]
    [SerializeField] private Animator m_playerAnimator;
    [SerializeField] private Animator m_shovelplayerAnimator;
    [SerializeField] private Animator m_carryplayerAnimator;
    private PlayerAnimationModel m_player;
    private PlayerAnimationModel m_shovelplayer;
    private PlayerAnimationModel m_carryplayer;
    public GameObject m_carryItem;

    private PlayerState m_currentState = PlayerState.DEFAULT;

    public bool m_canAttack = true;

    [Header("Other")]
    public GameObject m_cameraContainer;

    public float cameraZoomSpeed = 1.0f;
    public float cameraZoomMax = 5.0f;

    public bool m_functionalityEnabled = true;

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
        m_player.SetAnimator(m_playerAnimator);
        m_shovelplayer.SetAnimator(m_shovelplayerAnimator);
        m_carryplayer.SetAnimator(m_carryplayerAnimator);

        m_player.SetActive(true);

        m_menu = HUDManager.instance.GetElementByType(typeof(UI_QuestList)).transform.parent.gameObject;
        m_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
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

            m_player.animator.SetBool("IsMoving", movementInput != Vector2.zero);
            m_shovelplayer.animator.SetBool("IsMoving", movementInput != Vector2.zero);
            m_carryplayer.animator.SetBool("IsMoving", movementInput != Vector2.zero);
        }
        AnimationHandler();
    }
    private void FixedUpdate()
    {

        // Set jump input to off
        jumpInput = false;
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

    private void AnimationHandler()
    { 
        if (m_playerInventory.GetSelectItem() != null)
        {
            if (m_playerInventory.GetSelectItem().GetToolType() == ToolType.Shovel)
            {
                m_currentState = PlayerState.ATTACKING;
            }
            else
            {
                m_currentState = PlayerState.CARRYING;
            }
        }
        else
        {
            m_currentState = PlayerState.DEFAULT;
        }

        switch (m_currentState)
        {
            case PlayerState.DEFAULT:
                if (!m_player.isActive)
                {
                    m_carryItem.SetActive(false);
                    m_player.SetActive(true);

                    m_shovelplayer.SetActive(false);
                    m_carryplayer.SetActive(false);
                }
                break;
            case PlayerState.ATTACKING:
                if (!m_shovelplayer.isActive)
                {
                    m_carryItem.SetActive(false);
                    m_shovelplayer.SetActive(true);

                    m_player.SetActive(false);
                    m_carryplayer.SetActive(false);
                }
                break;
            case PlayerState.CARRYING:
                if (!m_carryplayer.isActive)
                {
                    m_carryItem.SetActive(true);
                    m_carryplayer.SetActive(true);

                    m_shovelplayer.SetActive(false);
                    m_player.SetActive(false);
                }
                break;
            default:
                break;
        }
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
        if (InputManager.instance.IsKeyDown(KeyType.E))
        {
            m_playerInteractor.InteractWithObject();
        }
    }

    private void CombatInput()
    {
        if (!m_canAttack)
            return;

        if (m_playerInventory.GetSelectItem() != null)
        {
            if (m_playerInventory.GetSelectItem().GetToolType() == ToolType.Shovel)
            {
                if (InputManager.instance.GetMouseButtonDown(MouseButton.LEFT))
                {
                    m_shovelplayer.animator.SetTrigger("Slam");
                    playerMovement.SlamAttack();
                }
                if (InputManager.instance.GetMouseButtonDown(MouseButton.RIGHT))
                {
                    m_shovelplayer.animator.SetTrigger("Swing");
                    playerMovement.SwingAttack();
                }
            }
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
