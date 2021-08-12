using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    #region Singleton

    public static DamageNumberManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Second Instance of DamageNumberManager was created, this instance was destroyed.");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    #endregion

    public GameObject m_prefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDamageNumber(Vector3 _position, int _damage)
    {
        GameObject newNumber = Instantiate(m_prefab, transform);
        newNumber.GetComponent<DamageNumbers>().SetPosition(_position);
        newNumber.GetComponent<DamageNumbers>().UpdateDamageValue(_damage);
    }
}
