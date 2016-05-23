/*-------------------------
 *模块名：负责安卓蓝牙连接模块
 *创建者：Zo
 *创建日期：2016.05.11
 *模块描述：通过蓝牙连接两个安卓设备
 * --------------------------*/


using UnityEngine;
using System.Collections;

public class BluetoothManager : MonoBehaviour {
    public static BluetoothManager instance = null;

    private bool initResult;
    private BluetoothMultiplayerMode desiredMode = BluetoothMultiplayerMode.None;

    public string myName = "";
    public ActorRPC myActor;
    private bool isDisConnectServer = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	void Start () {
	
	}

    #region bluetooth event
    void OnEnable()
    {
        initResult = BluetoothMultiplayerAndroid.Init("1185bc83-37d6-4b43-9e29-5d608fa1197c");
        BluetoothMultiplayerAndroid.SetVerboseLog(true);

        BluetoothMultiplayerAndroidManager.onBluetoothAdapterDisabledEvent += onBluetoothAdapterDisabledEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterEnabledEvent += onBluetoothAdapterEnabledEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterEnableFailedEvent += onBluetoothAdapterEnableFailedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothClientConnectedEvent += onBluetoothClientConnectedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothClientDisconnectedEvent += onBluetoothClientDisconnectedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothConnectedToServerEvent += onBluetoothConnectedToServerEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothConnectToServerFailedEvent += onBluetoothConnectToServerFailedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDevicePickedEvent += onBluetoothDevicePickedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDisconnectedFromServerEvent += onBluetoothDisconnectedFromServerEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDiscoveryDeviceFoundEvent += onBluetoothDiscoveryDeviceFoundEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDiscoveryFinishedEvent += onBluetoothDiscoveryFinishedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDiscoveryStartedEvent += onBluetoothDiscoveryStartedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothListeningCanceledEvent += onBluetoothListeningCanceledEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothListeningStartedEvent += onBluetoothListeningStartedEvent;
    }

    void OnDisable()
    {
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterDisabledEvent -= onBluetoothAdapterDisabledEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterEnabledEvent -= onBluetoothAdapterEnabledEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterEnableFailedEvent -= onBluetoothAdapterEnableFailedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothClientConnectedEvent -= onBluetoothClientConnectedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothClientDisconnectedEvent -= onBluetoothClientDisconnectedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothConnectedToServerEvent -= onBluetoothConnectedToServerEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothConnectToServerFailedEvent -= onBluetoothConnectToServerFailedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDevicePickedEvent -= onBluetoothDevicePickedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDisconnectedFromServerEvent -= onBluetoothDisconnectedFromServerEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDiscoveryDeviceFoundEvent -= onBluetoothDiscoveryDeviceFoundEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDiscoveryFinishedEvent -= onBluetoothDiscoveryFinishedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothDiscoveryStartedEvent -= onBluetoothDiscoveryStartedEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothListeningCanceledEvent -= onBluetoothListeningCanceledEvent;
        BluetoothMultiplayerAndroidManager.onBluetoothListeningStartedEvent -= onBluetoothListeningStartedEvent;
    }

    void onBluetoothAdapterDisabledEvent()
    {

    }

    void onBluetoothAdapterEnabledEvent()
    {
        switch (desiredMode)
        {
            case BluetoothMultiplayerMode.Server:
                Network.Disconnect();
                BluetoothMultiplayerAndroid.InitializeServer((ushort)GlobleData.Port);
                break;
            case BluetoothMultiplayerMode.Client:
                Network.Disconnect();
                BluetoothMultiplayerAndroid.ShowDeviceList();
                break;
        }
        desiredMode = BluetoothMultiplayerMode.None;
    }

    void onBluetoothAdapterEnableFailedEvent()
    { 
    
    }

    void onBluetoothClientConnectedEvent(BluetoothDevice device)
    { 
    
    }

    void onBluetoothClientDisconnectedEvent(BluetoothDevice device)
    { 
    
    }

    void onBluetoothConnectedToServerEvent(BluetoothDevice device)
    {
        Network.Connect(GlobleData.LocalIp, GlobleData.Port);
    }

    void onBluetoothConnectToServerFailedEvent(BluetoothDevice device)
    { 
        
    }

    void onBluetoothDevicePickedEvent(BluetoothDevice device)
    {
        BluetoothMultiplayerAndroid.Connect(device.address, (ushort)GlobleData.Port);
    }

    void onBluetoothDisconnectedFromServerEvent(BluetoothDevice device)
    {
        Network.Disconnect();
    }

    void onBluetoothDiscoveryDeviceFoundEvent(BluetoothDevice device)
    { 
        
    }

    void onBluetoothDiscoveryFinishedEvent()
    { 
    
    }

    void onBluetoothDiscoveryStartedEvent()
    { 
        
    }

    void onBluetoothListeningCanceledEvent()
    {
        BluetoothMultiplayerAndroid.Disconnect();
    }

    void onBluetoothListeningStartedEvent()
    {
        var useNet = !Network.HavePublicAddress();
        Network.InitializeServer(2, GlobleData.Port, useNet);
    }
    #endregion

    #region create server or connect
    /// <summary>
    /// 创建服务器
    /// </summary>
    public void CreateServer()
    {
        if (initResult)
        {
            if (BluetoothMultiplayerAndroid.CurrentMode() == BluetoothMultiplayerMode.None)
            {
                if (BluetoothMultiplayerAndroid.IsBluetoothEnabled())
                {
                    Network.Disconnect();
                    BluetoothMultiplayerAndroid.InitializeServer((ushort)GlobleData.Port);
                }
                else
                {
                    desiredMode = BluetoothMultiplayerMode.Server;
                    BluetoothMultiplayerAndroid.RequestBluetoothEnable();
                }
            }
        }
    }

    void OnServerInitialized()
    {
        if (Network.isServer)
        {
            GameObject temp = (GameObject)Network.Instantiate(Resources.Load("ActorRPC"), Vector3.zero, Quaternion.identity, 0) as GameObject;
            temp.transform.SetParent(transform.parent);

            this.myActor = temp.GetComponent<ActorRPC>();
            myName = "Server";
            MyDebug.instance.Log("create server succeed.");
        }
    }

    public void ConnectToServer()
    {
        if (initResult)
        {
            if (BluetoothMultiplayerAndroid.CurrentMode() == BluetoothMultiplayerMode.None)
            {
                if (BluetoothMultiplayerAndroid.IsBluetoothEnabled())
                {
                    Network.Disconnect();
                    BluetoothMultiplayerAndroid.ShowDeviceList();
                }
                else
                {
                    desiredMode = BluetoothMultiplayerMode.Client;
                    BluetoothMultiplayerAndroid.RequestBluetoothEnable();
                }
            }
        }
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        MyDebug.instance.Log("client connected.");
    }

    void OnConnectedToServer()
    {
        GameObject temp = (GameObject)Network.Instantiate(Resources.Load("ActorRPC"), Vector3.zero, Quaternion.identity, 0) as GameObject;
        temp.transform.SetParent(transform.parent);

        this.myActor = temp.GetComponent<ActorRPC>();
        myName = "Client";
        MyDebug.instance.Log(GameObject.FindGameObjectsWithTag("actor").Length.ToString());
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);

        MyDebug.instance.Log("player disconnectd.");
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        BluetoothMultiplayerAndroid.Disconnect();

        MyDebug.instance.Log("connect server failed:" + error.ToString());
    }

    void OnDisconnectedFromServer()
    {
        BluetoothMultiplayerAndroid.Disconnect();
        isDisConnectServer = true;

        MyDebug.instance.Log("server disconnected.");
    }


    void DisConnectServer()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("actor");

        MyDebug.instance.Log("disconnected server and player num is ：" + objs.Length.ToString());

        if (objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                Destroy(objs[i].gameObject);
            }
        }
    }

    public void FindActor()
    {
        myActor = GameObject.FindGameObjectWithTag("actor").GetComponent<ActorRPC>();
    }
    #endregion
}
