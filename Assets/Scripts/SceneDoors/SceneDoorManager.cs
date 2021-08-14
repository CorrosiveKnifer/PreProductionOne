using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDoorManager : MonoBehaviour
{
    public SceneDoor[] m_Doors;

    // Start is called before the first frame update
    void Awake()
    {
        m_Doors = FindObjectsOfType<SceneDoor>();

        // Check for direction duplicates.
        foreach (var Door1 in m_Doors)
        {
            foreach (var Door2 in m_Doors)
            {
                if (Door1 != Door2 && Door1.m_DoorDirection == Door2.m_DoorDirection)
                {
                    Debug.LogError("Multiple doors in the same direction");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetDoorSpawnPosition(DoorDirection _Direction)
    {
        foreach (var Door in m_Doors)
        {
            if (Door.m_DoorDirection == _Direction)
            {
                Vector3 SpawnOffset = new Vector3();
                switch (_Direction)
                {
                    case DoorDirection.NORTH:
                        SpawnOffset = -Vector3.forward; 
                        break;
                    case DoorDirection.SOUTH:
                        SpawnOffset = Vector3.forward;
                        break;
                    case DoorDirection.EAST:
                        SpawnOffset = -Vector3.right;
                        break;
                    case DoorDirection.WEST:
                        SpawnOffset = Vector3.right;
                        break;
                    case DoorDirection.INTERIOR:
                        SpawnOffset = Door.transform.forward;
                        break;
                }

                return Door.transform.position + SpawnOffset * 1.5f;
            }
        }
        if (m_Doors[0] != null)
        {
            Debug.LogWarning("The door trying to be obtained does not exist. Will use default instead.");
            return m_Doors[0].transform.position + transform.forward;
        }
        else
        {
            Debug.LogError("No doors exist in current scene. Deploying player in center of scene.");
            return new Vector3(0, 2, 0);
        }
    }
}
