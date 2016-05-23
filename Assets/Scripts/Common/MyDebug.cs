using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyDebug : MonoBehaviour {
    public static MyDebug instance;
    public bool isLog = false;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
   
	void Start () 
    {
        
	}

    public void Log(string msg)
    {
        if (isLog)
        {
            Debug.Log(msg);
        }
    }
}
