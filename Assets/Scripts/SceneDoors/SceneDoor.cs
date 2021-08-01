using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DoorDirection
{
    NORTH,
    SOUTH,
    WEST,
    EAST,
    INTERIOR,
}
public class SceneDoor : MonoBehaviour
{
    public DoorDirection m_DoorDirection;
    public string m_TargetScene;
    private DoorDirection m_DirectionTarget;
    

    // Start is called before the first frame update
    void Awake()
    {
        switch (m_DoorDirection)
        {
            case DoorDirection.NORTH:
                m_DirectionTarget = DoorDirection.SOUTH;
                break;
            case DoorDirection.SOUTH:
                m_DirectionTarget = DoorDirection.NORTH;
                break;
            case DoorDirection.WEST:
                m_DirectionTarget = DoorDirection.EAST;
                break;
            case DoorDirection.EAST:
                m_DirectionTarget = DoorDirection.WEST;
                break;
            case DoorDirection.INTERIOR:
                m_DirectionTarget = DoorDirection.INTERIOR;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerController>())
            return;

        // Begin scene transition.

        // Store other end of the door direction.
        GameManager.instance.m_TargetDoor = m_DirectionTarget;

        // Prevent player movement.

        // Prevent player from opening menus.

        LevelLoader.instance.LoadNewLevel(m_TargetScene);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2.0f);
    }
}
