using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour {
    public static DontDestroyOnLoad instance = null;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

	void Start () {
        MyDebug.instance.Log("logo scene...");
        SceneLoader.instance.LoadSceneByName("Main");
	}
	
}
