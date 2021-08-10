using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    private void Update()
    {
        if(InputManager.instance.IsKeyDown(KeyType.P))
        {
            SpawnSlime(transform.position, Random.RandomRange(1, 2));
        }
    }

    public void SpawnSlime(Vector3 _position, int _size = 1)
    {
        // Create greater slime.
        GameObject newSlime = Instantiate(Resources.Load<GameObject>("Prefabs/Slime"), _position, transform.rotation);
        newSlime.GetComponent<Slime>().SetSize(_size);
    }
}
