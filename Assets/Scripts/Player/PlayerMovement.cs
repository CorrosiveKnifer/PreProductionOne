using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerVitality m_playerVitality;
    private PlayerController m_playerController;

    private CharacterController characterController;
    private float yVelocity = 0.0f;
    private Vector3 storedMovement;
    private Vector3 lastMovement;

    public GameObject m_playerModel;

    [Header("Movement")]
    public bool grounded = true;
    public float movementSpeed = 2.0f;
    public float jumpSpeed = 4.0f;
    public float gravity = 9.81f;

    public float ledgeForgiveDelay = 0.0f;
    private float ledgeForgiveTimer = 0.0f;

    private MultiAudioAgent m_audioAgent;

    [Header("Combat")]
    public Transform m_slamPoint;
    public float m_slamRange = 0.5f;
    public Transform m_swingPoint;
    public float m_swingRange = 0.5f;
    public LayerMask m_enemyLayer;

    private void Awake()
    {
        m_audioAgent = GetComponent<MultiAudioAgent>();
        characterController = GetComponent<CharacterController>();
        m_playerController = GetComponent<PlayerController>();
        m_playerVitality = GetComponent<PlayerVitality>();
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController.enabled = false;
        transform.position = FindObjectOfType<SceneDoorManager>().GetDoorSpawnPosition(DoorDirection.INTERIOR);
        m_playerModel.transform.rotation = Quaternion.Euler(0.0f, 135.0f, 0.0f);
        characterController.enabled = true;
    }

    public GameObject TEMPTEMPTEMPDELETE;
    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.IsKeyDown(KeyType.K))
        {
            RotateToFaceTarget(TEMPTEMPTEMPDELETE.transform.position);
        }
    }

    private void FixedUpdate()
    {
        // Check if character is touching the ground
        if (characterController.isGrounded)
        {
            ledgeForgiveTimer = 0;
            grounded = true;

            if (yVelocity < 0.0f) // Checking if character is not going upwards
            {
                // Snap character to ground
                yVelocity = 0.0f;
            }
        }
        else
        {
            // If not grounded apply gravity
            yVelocity -= gravity * Time.fixedDeltaTime;

            if (ledgeForgiveTimer < ledgeForgiveDelay)
            {
                ledgeForgiveTimer += Time.fixedDeltaTime;
            }
            else if (grounded)
            {
                grounded = false;
                storedMovement = lastMovement;
            }
        }
    }
    public void Move(Vector2 _move)
    {
        float speed = movementSpeed;
        if (m_playerVitality.IsStatusActive(statusType.OVER_FILLED))
            speed *= 0.5f;
        if (m_playerVitality.IsStatusActive(statusType.SPEED))
            speed *= 2.0f;

        Vector3 normalizedMove = new Vector3(0, 0, 0);

        // Movement
        normalizedMove += _move.y * transform.forward;
        normalizedMove += _move.x * transform.right;


        // Apply movement
        Vector3 movement = normalizedMove.normalized * speed * Time.deltaTime;
        lastMovement = movement;

        if (!grounded)
        {
            movement = storedMovement;
        }

        // Apply Y velocity
        movement.y += yVelocity * Time.deltaTime;

        // Apply movement to character controller
        characterController.Move(movement);

        Vector3 direction;
        direction.x = _move.x;
        direction.y = 0;
        direction.z = _move.y;
        // Rotate player model
        if (direction != new Vector3(0,0,0) && m_playerModel != null)
        {
            direction = direction.normalized;
            float angle = Vector3.SignedAngle(transform.right, direction, transform.up);

            //angle *= Mathf.Sign(direction.x);

            m_playerModel.transform.rotation = Quaternion.Lerp(m_playerModel.transform.rotation, 
                Quaternion.Euler(0, angle, 0),
                1 - Mathf.Pow(2.0f, -Time.deltaTime * 20.0f));
        }
    }

    public void RotateToFaceTarget(Vector3 _point)
    {
        Vector3 direction = _point - transform.position;
        direction.y = 0;
        direction = direction.normalized;
        if (direction != new Vector3(0, 0, 0) && m_playerModel != null)
        {
            direction = direction.normalized;
            float angle = Vector3.SignedAngle(transform.right, direction, transform.up);

            StartCoroutine(RotateCoroutine(angle));
        }
    }


    IEnumerator RotateCoroutine(float _angle)
    {
        float frameLength = 0.01f;
        float speed = 2.0f;
        float time = 0.0f;

        float maxTime = 0.3f;

        Quaternion initalRotation = m_playerModel.transform.rotation;

        while (time < maxTime)
        {
            yield return new WaitForSecondsRealtime(frameLength);
            time += frameLength * speed;

            m_playerModel.transform.rotation = Quaternion.Lerp(initalRotation,
                Quaternion.Euler(0, _angle + 45.0f, 0),
                time / maxTime);
        }
        
    }
    public void SwingAttack()
    {
        m_audioAgent.Play("AttackSwing");

        // Detect enemies in range of attacks
        Collider[] hits = Physics.OverlapSphere(m_swingPoint.position, m_swingRange, m_enemyLayer);

        bool hitEnemy = false;

        // Damage them
        foreach (var enemy in hits)
        {
            Debug.Log("Detected!");
            if (enemy.GetComponentInParent<Slime>())
            {
                hitEnemy = true;
                Vector3 direction = enemy.transform.position - transform.position;
                direction.y = 0;
                direction = direction.normalized;
                enemy.GetComponentInParent<Slime>().Knockback(direction, 7.0f);
                enemy.GetComponentInParent<Slime>().DamageEnemy(2);
                Debug.Log("Damage Enemy");
            }
        }

        if (hitEnemy)
            m_audioAgent.Play("ShovelHit");
    }

    public void SlamAttack()
    {
        // Detect enemies in range of attacks
        Collider[] hits = Physics.OverlapSphere(m_slamPoint.position, m_slamRange, m_enemyLayer);

        m_audioAgent.Play("SlamHit");
        m_playerController.GetCamera().GetComponent<ScreenShake>().StartScreenShake();

        // Damage them
        foreach (var enemy in hits)
        {
            Debug.Log("Detected!");
            if (enemy.GetComponentInParent<Slime>())
            {
                enemy.GetComponentInParent<Slime>().DamageEnemy(5);
                Debug.Log("Damage Enemy");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(m_swingPoint.position, m_swingRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_slamPoint.position, m_slamRange);
    }

}
