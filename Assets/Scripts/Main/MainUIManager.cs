using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using HiData;

public class MainUIManager : MonoBehaviour {
    public static MainUIManager instance;

    public InputField my_Input;
    public GameObject cube;
    void Awake()
    {
        instance = this;
    }

	void Start () {
        MyDebug.instance.Log("main scene...");
	}

    public void OnCreateClick()
    {
        MyDebug.instance.Log("create server.");
        BluetoothManager.instance.CreateServer();
    }

    public void OnJoinClick()
    {
        MyDebug.instance.Log("join server.");
        BluetoothManager.instance.ConnectToServer();
    }

    public void OnSendClick()
    {
        if (my_Input.text != "")
        {
            if (BluetoothManager.instance.myActor != null)
            {
                Message_Send.Instance().Chat(BluetoothManager.instance.myName, my_Input.text);
            }
        }
    }

    public void Open()
    {
        if (KGFDebugGUI.itsInstance.itsOpen)
        {
            KGFDebugGUI.itsInstance.itsOpen = false;
        }
        else
        {
            KGFDebugGUI.itsInstance.itsOpen = true;
        }
    }

    public void TestAsy()
    {
        Instantiate(cube, Vector3.zero, Quaternion.identity);

        int leng = GameObject.FindGameObjectsWithTag("cube").Length;
        MyDebug.instance.Log("cube:"+leng.ToString());
    }
}
