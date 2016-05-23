using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using HiData;

public class MainUIManager : MonoBehaviour {

    public InputField my_Input;

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
}
