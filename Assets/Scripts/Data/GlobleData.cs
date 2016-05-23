using UnityEngine;
using System.Collections;

public class GlobleData : MonoBehaviour {
    public static GlobleData instance;
    public static readonly string LocalIp = "127.0.0.1";
    public static readonly int Port = 8080;

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

	void Start () {
	
	}
	

}
