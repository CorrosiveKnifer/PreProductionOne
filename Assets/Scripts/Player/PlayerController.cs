using System;
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
        WATERING,
        DEAD
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
    [SerializeField] private Animator m_waterplayerAnimator;
    private PlayerAnimationModel m_player;
    private PlayerAnimationModel m_shovelplayer;
    private PlayerAnimationModel m_carryplayer;
    private PlayerAnimationModel m_waterplayer;
    public GameObject m_carryItem;

    private PlayerState m_currentState = PlayerState.DEFAULT;

    public bool m_canAttack = true;

    public enum DisplayState
    {
        NONE,
        INVENTORY,
        QUESTS,
    }

    [Header("Other")]
    public GameObject m_cameraContainer;

    public float cameraZoomSpeed = 1.0f;
    public float cameraZoomMax = 5.0f;

    public bool m_functionalityEnabled = true;

    public Texture2D DefaultCursor;
    public Texture2D HandCursor;
    public Texture2D AttackCursor;
    public Texture2D TalkCursor;

    private DisplayState m_state;
    private PlayerMovement playerMovement;
    private PlayerPlacing m_playerPlacing;
    private PlayerVitality m_playerVitality;
    private PlayerInteractor m_playerInteractor;
    private PlayerInventory m_playerInventory;
    private PlayerQuests m_playerQuests;
    private Camera m_playerCamera;
    private Vector2 movementInput;
    private bool jumpInput = false;

    private float m_inputDelay;
    private bool m_showInventory = false;
    [SerializeField] private GameObject m_menu;
    private bool isAttacking = false;
    private GameObject m_actionObject;
    private bool m_cancelAttack = false;

    private void Awake()
    {
        m_cameraContainer.transform.parent = null;

        playerMovement = GetComponent<PlayerMovement>();
        m_playerPlacing = GetComponent<PlayerPlacing>();
        m_playerVitality = GetComponent<PlayerVitality>();
        m_playerInteractor = GetComponent<PlayerInteractor>();
        m_playerInventory = GetComponent<PlayerInventory>();
        m_playerQuests = GetComponent<PlayerQuests>();
        m_playerCamera = GetCamera();
        Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.Auto);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_player.SetAnimator(m_playerAnimator);
        m_shovelplayer.SetAnimator(m_shovelplayerAnimator);
        m_carryplayer.SetAnimator(m_carryplayerAnimator);
        m_waterplayer.SetAnimator(m_waterplayerAnimator);

        m_player.SetActive(true);

        m_menu = HUDManager.instance.GetElementByType(typeof(UI_QuestList)).transform.parent.gameObject;
        m_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        m_cancelAttack = false;
        CursorUpdate();
        HUDInput();

        if (m_functionalityEnabled)
        {
            CameraControl();
            isAttacking = m_shovelplayerAnimator.GetBool("Mutex") || m_waterplayerAnimator.GetBool("Mutex");
            if (!isAttacking)
            {
                InteractInput();
                MovementInput();
                CombatInput();
                HotbarInput();
            }
            else
            {
                movementInput = Vector2.zero;
            }

            // Call movement function
            playerMovement.Move(movementInput);

            m_player.animator.SetBool("IsMoving", movementInput != Vector2.zero);
            m_shovelplayer.animator.SetBool("IsMoving", movementInput != Vector2.zero);
            m_carryplayer.animator.SetBool("IsMoving", movementInput != Vector2.zero);
            m_waterplayer.animator.SetBool("IsMoving", movementInput != Vector2.zero);
        }

        if (m_playerVitality.m_hunger <= 0.0f && m_currentState != PlayerState.DEAD)
            StartCoroutine(Die());

        AnimationHandler();
    }

    private void FixedUpdate()
    {
        // Set jump input to off
        jumpInput = false;
    }

    IEnumerator Die()
    {
        m_currentState = PlayerState.DEAD;
        m_functionalityEnabled = false;
        // Die animation here
        yield return new WaitForSecondsRealtime(1.0f);
        GameManager.instance.SkipTime(24);
        GameManager.instance.SkipTime(24);
        GameManager.instance.SkipTime(24);

        LevelLoader.instance.ReloadLevel();
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

    private void AnimationHandler()
    {
        if (m_currentState != PlayerState.DEAD)
        {
            if (m_playerInventory.GetSelectItem() != null)
            {
                if (m_playerInventory.GetSelectItem().GetToolType() == ToolType.Shovel)
                {
                    m_currentState = PlayerState.ATTACKING;
                }
                else if (m_playerInventory.GetSelectItem().GetToolType() == ToolType.WaterCan)
                {
                    m_currentState = PlayerState.WATERING;
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
        }

        switch (m_currentState)
        {
            case PlayerState.ATTACKING:
                if (!m_shovelplayer.isActive)
                {
                    m_carryItem.SetActive(false);
                    m_shovelplayer.SetActive(true);

                    m_waterplayer.SetActive(false);
                    m_player.SetActive(false);
                    m_carryplayer.SetActive(false);
                }
                break;
            case PlayerState.CARRYING:
                if (!m_carryplayer.isActive)
                {
                    m_carryItem.SetActive(true);
                    m_carryplayer.SetActive(true);

                    m_waterplayer.SetActive(false);
                    m_shovelplayer.SetActive(false);
                    m_player.SetActive(false);
                }
                break;
            case PlayerState.WATERING:
                if (!m_waterplayer.isActive)
                {
                    m_waterplayer.SetActive(true);

                    m_carryItem.SetActive(false);
                    m_carryplayer.SetActive(false);
                    m_shovelplayer.SetActive(false);
                    m_player.SetActive(false);
                }
                break;
            default:
                if (!m_player.isActive)
                {
                    m_carryItem.SetActive(false);
                    m_player.SetActive(true);

                    m_waterplayer.SetActive(false);
                    m_shovelplayer.SetActive(false);
                    m_carryplayer.SetActive(false);
                }
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
        if (InputManager.instance.GetMouseButtonDown(MouseButton.LEFT))
        {
            RaycastHit hit;
            Ray ray = m_playerCamera.ScreenPointToRay(InputManager.instance.GetMousePositionInScreen());
            if (Physics.Raycast(ray, out hit, 1000)) 
            {
                if(m_playerInteractor.Contains(hit.collider.gameObject))
                {
                    hit.collider.GetComponent<Interactable>().Interact();
                    playerMovement.RotateToFaceTarget(hit.collider.gameObject.transform.position);
                    m_cancelAttack = true;
                }
            }
        }
    }

    public void StartDigActionOn(GameObject gameObject)
    {
        m_actionObject = gameObject;

        m_shovelplayer.animator.SetTrigger("Dig");
    }
    public void StartWaterActionOn(GameObject gameObject)
    {
        m_actionObject = gameObject;

        m_waterplayerAnimator.SetTrigger("Water");
    }

    public void DigAction()
    {
        Destroy(m_actionObject);
        m_actionObject = null;
    }

    public void WaterAction()
    {
        
        if(m_actionObject.GetComponent<CropScript>() != null)
        {
            float amount = Mathf.Clamp(m_playerInventory.GetSelectItem().m_amount - 1, 0, 20.0f);

            m_playerInventory.RemoveItem(7, (int)amount);
            m_actionObject.GetComponent<CropScript>().Water(amount);
        }
        else if(m_actionObject.GetComponent<WaterFiller>() != null)
        {
            m_actionObject.GetComponent<WaterFiller>().FillWater();
        }
        m_actionObject = null;
    }

    private void CombatInput()
    {
        if (!m_canAttack)
            return;
        if (m_cancelAttack)
            return;

        if (m_playerInventory.GetSelectItem() != null)
        {
            if (m_playerInventory.GetSelectItem().GetToolType() == ToolType.Shovel)
            {
                if (InputManager.instance.GetMouseButtonDown(MouseButton.LEFT))
                {
                    m_shovelplayer.animator.SetTrigger("Slam");
                }
                if (InputManager.instance.GetMouseButtonDown(MouseButton.RIGHT))
                {
                    m_shovelplayer.animator.SetTrigger("Swing");
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

        if (InputManager.instance.IsKeyDown(KeyType.ESC) && m_inputDelay <= 0)
        {
            switch (m_state)
            {
                case DisplayState.NONE:
                    break;
                case DisplayState.INVENTORY:
                    OpenInventory();
                    break;
                case DisplayState.QUESTS:
                    OpenQuests();
                    break;
                default:
                    break;
            }
        }

        if (InputManager.instance.IsKeyDown(KeyType.I) && m_inputDelay <= 0)
        {
            m_inputDelay = 0.25f;
            OpenInventory();
        }

        if (InputManager.instance.IsKeyDown(KeyType.O) && m_inputDelay <= 0)
        {
            m_inputDelay = 0.25f;
            OpenQuests();
        }
    }

    public void OpenInventory()
    {      
        if(m_state != DisplayState.INVENTORY)
        {
            m_showInventory = true;
            m_functionalityEnabled = false;
            m_playerInventory.GenerateOnDisplay(true);
            m_playerQuests.GenerateOnDisplay(false);
            m_state = DisplayState.INVENTORY;
        }
        else
        {
            m_state = DisplayState.NONE;
            m_showInventory = false;
            m_functionalityEnabled = true;
        }

        m_menu.SetActive(m_showInventory);
    }
    public void OpenQuests()
    {
        if (m_state != DisplayState.QUESTS)
        {
            m_showInventory = true;
            m_functionalityEnabled = false;
            m_playerInventory.GenerateOnDisplay(false);
            m_playerQuests.GenerateOnDisplay(true);
            m_state = DisplayState.QUESTS;
        }
        else
        {
            m_state = DisplayState.NONE;
            m_showInventory = false;
            m_functionalityEnabled = true;
        }

        m_menu.SetActive(m_showInventory);
    }
}
