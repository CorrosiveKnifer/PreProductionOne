using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    public SunScript m_sunScript;
    private float m_spawnTimer = 0.5f;

    private void Start()
    {
        m_sunScript = FindObjectOfType<SunScript>();
        SlimeSpawner[] spawners = FindObjectsOfType<SlimeSpawner>();
    }

    private void Update()
    {
        m_spawnTimer -= Time.deltaTime * 0.05f * (GameManager.instance.m_gameTimer.m_isRaining ? 2.0f : 1.0f);
        if (m_spawnTimer < 0 && FindObjectsOfType<Slime>().Length < 10)
        {
            int amount = Random.Range(2, 7);
            Debug.Log(amount);
            for (int i = 0; i < amount; i++)
            {
                SpawnSlime(transform.position, Random.Range(1, 3));
            }

            m_spawnTimer = Random.Range(0.2f, 2.0f) + 0.35f * amount;
        }

        if(InputManager.instance.IsKeyDown(KeyType.P))
        {
            SpawnSlime(transform.position, Random.Range(1, 2));
        }
    }

    public void SpawnSlime(Vector3 _position, int _size = 1)
    {
        // Create greater slime.
        GameObject newSlime = Instantiate(Resources.Load<GameObject>("Prefabs/Slime"), _position, transform.rotation);
        newSlime.GetComponent<Slime>().SetSize(_size);
    }
}
