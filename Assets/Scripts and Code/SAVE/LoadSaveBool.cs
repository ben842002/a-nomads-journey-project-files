using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSaveBool : MonoBehaviour
{   
    // use a sington to see the bool in the inspector (doesn't change much so don't worry)
    public static LoadSaveBool instance;
    public bool LoadSave;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
