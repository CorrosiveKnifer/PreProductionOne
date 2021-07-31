using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class statusEffect
{
    public statusType effect;
    public float duration;
    public float startDuration;

    public bool UpdateDuration()
    {
        duration -= Time.deltaTime;
        return (duration <= 0.0f);
    }
}
public enum statusType
{
    TEST,
    OVER_FILLED,
}

public class PlayerVitality : MonoBehaviour
{
    [SerializeField] private UI_HungerBar m_hungerBar;
    public float m_hunger;
    public List<statusEffect> playerEffects = new List<statusEffect>();

    // Start is called before the first frame update
    void Awake()
    {
        m_hunger = GameManager.instance.m_hunger;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.IsKeyDown(KeyType.V))
            AddStatus(statusType.TEST, 12.0f, false);

        if (InputManager.instance.IsKeyDown(KeyType.F))
            Eat(30.0f);

        if (!IsStatusActive(statusType.OVER_FILLED))
            m_hunger -= Time.deltaTime;

        StatusUpdate();
    }

    public float GetHunger()
    {
        return m_hunger;
    }

    public void Eat(float _foodValue)
    {
        m_hunger += _foodValue;

        if (m_hunger > 110.0f)
        {
            AddStatus(statusType.OVER_FILLED, (m_hunger - 100.0f) / 2.0f, false);
            m_hunger = 100.0f;
        }
    }

    public bool IsStatusActive(statusType _status)
    {
        foreach (var status in playerEffects)
        {
            if (status.effect == _status)
                return true;
        }
        return false;
    }

    public void AddStatus(statusType _status, float _duration, bool _addititve)
    {
        if (_addititve)
        {
            foreach (var status in playerEffects)
            {
                if (status.effect == _status)
                {
                    status.duration += _duration;
                    status.startDuration = status.duration;
                }
            }
        }
        else
        {
            RemoveStatus(_status);

            statusEffect newStatus = new statusEffect();
            newStatus.effect = _status;
            newStatus.duration = _duration;
            newStatus.startDuration = newStatus.duration;

            playerEffects.Add(newStatus);
            m_hungerBar.CreateTimer(_status);
        }
    }

    public void RemoveStatus(statusType _status)
    {
        List<statusEffect> removeList = new List<statusEffect>();
        foreach (var status in playerEffects)
        {
            if (status.effect == _status)
            {
                removeList.Add(status); // Add effects to be removed
            }
        }

        foreach (var status in removeList)
        {
            m_hungerBar.DestroyTimer(status.effect);
            playerEffects.Remove(status); // Remove effects
        }
    }

    private void StatusUpdate()
    {
        m_hungerBar.UpdateTimers(playerEffects);
        List<statusEffect> removeList = new List<statusEffect>();
        foreach (var item in playerEffects)
        {
            switch (item.effect) // During effect
            {
                case statusType.OVER_FILLED: 
                    break;
                default:
                    break;
            }
            if (item.UpdateDuration())
            {
                switch (item.effect) // Finish effect
                {
                    case statusType.OVER_FILLED:
                        break;
                    default:
                        break;
                }
                removeList.Add(item); // Add effects to be removed
            }
        }
        foreach (var item in removeList)
        {
            m_hungerBar.DestroyTimer(item.effect);
            playerEffects.Remove(item); // Remove effects
        }
    }
}
