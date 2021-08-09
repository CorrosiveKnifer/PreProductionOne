using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCScript : MonoBehaviour
{
    public int id
    { //Order based on scene hierarchy
        get {
            NPCScript[] npcs = FindObjectsOfType<NPCScript>();
            for (int i = 0; i < npcs.Length; i++)
            {
                if (npcs[i] == this)
                {
                    return i;
                }
            }
            return npcs.Length;
        }
    }

    protected virtual void Awake() { }

    protected virtual void Start() { }

    protected virtual void Update() { }

    public abstract string GetExtraData(); //For saveSlot
    public abstract void SetExtraData(string data);

}
