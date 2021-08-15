using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType
{
    Grass,
    Wood,
    Water,
}
public class SoundField : MonoBehaviour
{
    public MaterialType m_myType;
    public static SoundField fieldWithPriority;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            fieldWithPriority = this;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
            if(fieldWithPriority == this && other.GetComponentInChildren<Animator>().GetBool("IsMoving"))
            {
                MultiAudioAgent agent = HUDManager.instance.GetComponent<MultiAudioAgent>();

                switch (m_myType)
                {
                    case MaterialType.Grass:
                        agent.PlayOnce("Walk_Grass");
                        agent.StopAudio("Walk_Wood");
                        agent.StopAudio("Walk_Water");
                        break;
                    case MaterialType.Wood:
                        agent.PlayOnce("Walk_Wood");
                        agent.StopAudio("Walk_Grass");
                        agent.StopAudio("Walk_Water");
                        break;
                    case MaterialType.Water:
                        agent.PlayOnce("Walk_Water");
                        agent.StopAudio("Walk_Wood");
                        agent.StopAudio("Walk_Grass");
                        other.GetComponent<PlayerController>().SpawnSplashVFX(other.transform.position);
                        break;
                    default:
                        break;
                }

                
            }
            else if(fieldWithPriority == this && !other.GetComponentInChildren<Animator>().GetBool("IsMoving"))
            {
                MultiAudioAgent agent = HUDManager.instance.GetComponent<MultiAudioAgent>();

                agent.StopAudio("Walk_Wood");
                agent.StopAudio("Walk_Grass");
                agent.StopAudio("Walk_Water");
            }
    }
}
