/*-------------------------
 *模块名：网络蓝牙连接模块
 *创建者：Zo
 *创建日期：2015.6.10
 *模块描述：通过蓝牙连接两个安卓设备 相互发送消息
 * --------------------------*/

using UnityEngine;
using System.Collections;

public class NetManager : MonoBehaviour
{
    public static NetManager Instance;

    private bool initResult;
    private BluetoothMultiplayerMode desiredMode = BluetoothMultiplayerMode.None;

    public Actor myActor;
    private bool isDisConnectServer = false;
    void OnEnable()
    {
        initResult = BluetoothMultiplayerAndroid.Init("1185bc83-37d6-4b43-9e29-5d608fa1197c");
        BluetoothMultiplayerAndroid.SetVerboseLog(true);

        BluetoothMultiplayerAndroidManager.onBluetoothListeningStartedEvent += onBluetoothListeningStarted;
        BluetoothMultiplayerAndroidManager.onBluetoothListeningCanceledEvent += onBluetoothListeningCanceled;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterEnabledEvent += onBluetoothAdapterEnabled;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterEnableFailedEvent += onBluetoothAdapterEnableFailed;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterDisabledEvent += onBluetoothAdapterDisabled;
        BluetoothMultiplayerAndroidManager.onBluetoothConnectedToServerEvent += onBluetoothConnectedToServer;
        BluetoothMultiplayerAndroidManager.onBluetoothConnectToServerFailedEvent += onBluetoothConnectToServerFailed;
        BluetoothMultiplayerAndroidManager.onBluetoothDisconnectedFromServerEvent += onBluetoothDisconnectedFromServer;
        BluetoothMultiplayerAndroidManager.onBluetoothClientConnectedEvent += onBluetoothClientConnected;
        BluetoothMultiplayerAndroidManager.onBluetoothClientDisconnectedEvent += onBluetoothClientDisconnected;
        BluetoothMultiplayerAndroidManager.onBluetoothDevicePickedEvent += onBluetoothDevicePicked;
    }

    void OnDisable()
    {
        BluetoothMultiplayerAndroidManager.onBluetoothListeningStartedEvent -= onBluetoothListeningStarted;
        BluetoothMultiplayerAndroidManager.onBluetoothListeningCanceledEvent -= onBluetoothListeningCanceled;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterEnabledEvent -= onBluetoothAdapterEnabled;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterEnableFailedEvent -= onBluetoothAdapterEnableFailed;
        BluetoothMultiplayerAndroidManager.onBluetoothAdapterDisabledEvent -= onBluetoothAdapterDisabled;
        BluetoothMultiplayerAndroidManager.onBluetoothConnectedToServerEvent -= onBluetoothConnectedToServer;
        BluetoothMultiplayerAndroidManager.onBluetoothConnectToServerFailedEvent -= onBluetoothConnectToServerFailed;
        BluetoothMultiplayerAndroidManager.onBluetoothDisconnectedFromServerEvent -= onBluetoothDisconnectedFromServer;
        BluetoothMultiplayerAndroidManager.onBluetoothClientConnectedEvent -= onBluetoothClientConnected;
        BluetoothMultiplayerAndroidManager.onBluetoothClientDisconnectedEvent -= onBluetoothClientDisconnected;
        BluetoothMultiplayerAndroidManager.onBluetoothDevicePickedEvent -= onBluetoothDevicePicked;
    }

    void Awake()
    {
        Instance = this;
    }
	void Start () {
	
	}

    void Update()
    {
        if (isDisConnectServer)
        {
            DisConnectServer();
            isDisConnectServer = false;
        }
    }   

    #region 创建-连接
    //创建服务器
    public void CreateServer()
    {
        if (initResult)
            if (BluetoothMultiplayerAndroid.CurrentMode() == BluetoothMultiplayerMode.None)
                if (BluetoothMultiplayerAndroid.IsBluetoothEnabled()){
                    Network.Disconnect();
                    BluetoothMultiplayerAndroid.InitializeServer((ushort)Common.Port);
                } else{
                    desiredMode = BluetoothMultiplayerMode.Server;
                    BluetoothMultiplayerAndroid.RequestBluetoothEnable();
                }
    }
    //连接到服务器
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

    void OnServerInitialized()
    {
        if (Network.isServer)
        {
            MyLog.Instance.Log("创建服务器成功");

            GameObject temp = (GameObject)Network.Instantiate(GameManager.Instance.actor, Vector3.zero, Quaternion.identity, 0)as GameObject;
            this.myActor = temp.GetComponent<Actor>();

            GameManager.Instance.side = 0;
            GameUIManager.Instance.OpenGame();
            GameUIManager.Instance.info.text = "等待玩家加入";
        }
    }
    /// <summary>
    /// 新玩家连接成功在服务器上调用这个函数
    /// </summary>
    /// <param name="player"></param>
    void OnPlayerConnected(NetworkPlayer player)
    {
        onSendMessage("Start,0," + GlobalData.instance.player);
        GameUIManager.Instance.info.text = "玩家加入";
    }
    /// <summary>
    /// 成功连接到服务器时在客户端调用这个函数
    /// </summary>
    void OnConnectedToServer()
    {
        GameObject temp = (GameObject)Network.Instantiate(GameManager.Instance.actor, Vector3.zero, Quaternion.identity,0) as GameObject;
        this.myActor = temp.GetComponent<Actor>();
        onSendMessage("Start,1,"+GlobalData.instance.player);

        GameManager.Instance.side = 1;
        GameUIManager.Instance.info.text = "加入房间";
        GameUIManager.Instance.OpenGame();
    }
    /// <summary>
    /// 玩家从服务器断开时 在服务器上调用这个函数
    /// </summary>
    /// <param name="player"></param>
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);

        GameUIManager.Instance.OpenMain();
        GameManager.Instance.DestroyAllQizhi();

        MyLog.Instance.Log("玩家断开连接");
    }
    /// <summary>
    /// 某个原因连接失败 在客服端调用这个函数
    /// </summary>
    /// <param name="error"></param>
    void OnFailedToConnect(NetworkConnectionError error)
    {
        BluetoothMultiplayerAndroid.Disconnect();

        GameUIManager.Instance.OpenMain();
        GameManager.Instance.DestroyAllQizhi();

        MyLog.Instance.Log("服务器连接失败"+error.ToString());
    }

    /// <summary>
    /// 在服务器上连接断开 在客户端调用这个函数
    /// </summary>
    void OnDisconnectedFromServer()
    {
        BluetoothMultiplayerAndroid.Disconnect();
        isDisConnectServer = true;

        MyLog.Instance.Log("与服务器断开连接，请重新连接到服务器");
        
    }

    void DisConnectServer()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("actor");

        MyLog.Instance.Log("断开连接删除：" + objects.Length.ToString());
        if (objects != null)
            foreach (var obj in objects)
            {
                Destroy(obj.gameObject);
            }

        GameUIManager.Instance.OpenMain();
        GameManager.Instance.DestroyAllQizhi();
    }
    #endregion

    #region  消息处理
    /// <summary>
    /// 处理接收的消息
    /// </summary>
    /// <param name="message"></param>
    public void ReciveMessage(string message)
    {
        MyLog.Instance.Log(message);
        string[] splitMsg = message.Split(',');

        switch (splitMsg[0])
        {
            case "Start":
                Start_Message(splitMsg[1],splitMsg[2]);
                break;
            case "Grid":
                GetGridMsg(splitMsg[1], splitMsg[2], splitMsg[3]);
                break;
            case "Biaoji":
                GetBiaojiMsg(splitMsg[1], splitMsg[2], splitMsg[3]);
                break;
            case "Move_Grid":
                GetMoveGridMsg(splitMsg[1], splitMsg[2], splitMsg[3]);
                break;
            case "Move_Biaoji":
                GetMoveBiaojiMsg(splitMsg[1], splitMsg[2], splitMsg[3]);
                break;
            case "Choose_Move":
                GetMoveChooseMsg(splitMsg[1], splitMsg[2], splitMsg[3]);
                break;
            case "Move":
                GetMoveMsg(splitMsg[1], splitMsg[2], splitMsg[3]);
                break;
            case "Destroy":
                GetDestroyMsg(splitMsg[1], splitMsg[2], splitMsg[3]);
                break;
            case "Talk":
                //MyLog.Instance.OnReciveMsg(splitMsg[1]);
                break;
        }

        GameUIManager.Instance.ChangeStatus();
    }
    void Start_Message(string status, string player)
    {
        switch (status)
        {
            case "0":
                GameManager.Instance.state = State.Wait;
                GameUIManager.Instance.info.text = "等待对方落子";
                break;
            case "1":
                GameManager.Instance.state = State.Start;
                GameUIManager.Instance.info.text = "游戏已开始，请落子";
                break;
        }
        MyLog.Instance.Log(player);
    }

    void GetGridMsg(string status, string x, string y)
    {
        switch (status)
        {
            case "0":
                GameManager.Instance.state = State.Wait;
                GameUIManager.Instance.info.text = "等待标记棋子";
                break;
            case "1":
                GameManager.Instance.state = State.Start;
                GameUIManager.Instance.info.text = "请落棋";
                break;
        }

        if (GameManager.Instance.side == 0)
            GameManager.Instance.GenerateQizhi(1, int.Parse(x), int.Parse(y),false);
        else
            GameManager.Instance.GenerateQizhi(0, int.Parse(x), int.Parse(y),false);

        GameUIManager.Instance.FindColor();
        GameManager.Instance.Flash();
    }

    void GetBiaojiMsg(string status, string x, string y)
    {
        switch (status)
        {
            case "0":
                GameManager.Instance.state = State.Wait;
                GameUIManager.Instance.info.text = "等待对方标记棋子";
                break;
            case "1":
                GameManager.Instance.state = State.Start;
                GameUIManager.Instance.info.text = "请落棋";
                break;
        }

        if (GameManager.Instance.side == 0)
        {
            MyLog.Instance.Log("---标记黑棋---:" + x + "/" + y);
            GameManager.Instance.BiaojiQizhi(2, int.Parse(x), int.Parse(y), false);
        }
        else
        {
            MyLog.Instance.Log("---标记白棋---:" + x + "/" + y);
            GameManager.Instance.BiaojiQizhi(3, int.Parse(x), int.Parse(y), false);
        }

        GameUIManager.Instance.FindColor();
        GameManager.Instance.Flash();
    }

    void GetMoveGridMsg(string status, string x, string y)
    {
        if (GameManager.Instance.side == 0)
        {
            MyLog.Instance.Log("---生成黑子---:" + x + "/" + y);
            GameManager.Instance.GenerateQizhi(1, int.Parse(x), int.Parse(y), false);
        }
        else
        {
            MyLog.Instance.Log("---生成白子---:" + x + "/" + y);
            GameManager.Instance.GenerateQizhi(0, int.Parse(x), int.Parse(y), false);
        }

        switch (status)
        {
            case "0":
                GameManager.Instance.state = State.Wait;
                GameUIManager.Instance.info.text = "等待对手移动";
                break;
            case "1":
                GameManager.Instance.state = State.Start;
                GameUIManager.Instance.info.text = "请移动";
                break;
        }

        GameManager.Instance.isMove = true;
        GameManager.Instance.ClearBiaoji();

        GameUIManager.Instance.FindColor();
        GameManager.Instance.Flash();
    }

    void GetMoveBiaojiMsg(string status, string x, string y)
    {
        if (GameManager.Instance.side == 0)
        {
            MyLog.Instance.Log("---标记黑棋---:" + x + "/" + y);
            GameManager.Instance.BiaojiQizhi(2, int.Parse(x), int.Parse(y), false);
        }
        else
        {
            MyLog.Instance.Log("---标记白棋---:" + x + "/" + y);
            GameManager.Instance.BiaojiQizhi(3, int.Parse(x), int.Parse(y), false);
        }

        switch (status)
        {
            case "0":
                GameManager.Instance.state = State.Wait;
                GameUIManager.Instance.info.text = "等待对手移动";
                break;
            case "1":
                GameManager.Instance.state = State.Start;
                GameUIManager.Instance.info.text = "请移动";
                break;
        }

        GameManager.Instance.isMove = true;
        GameManager.Instance.ClearBiaoji();

        GameUIManager.Instance.FindColor();
        GameManager.Instance.Flash();
    }

    void GetMoveChooseMsg(string status, string x, string y)
    {
        switch (status)
        {
            case "0":
                GameManager.Instance.state = State.Wait;
                break;
            case "1":
                GameManager.Instance.state = State.Start;
                break;
        }
        GameManager.Instance.logicGrid = new Vector2(int.Parse(x), int.Parse(y));
        GameUIManager.Instance.info.text = "等待对手移动";
        GameUIManager.Instance.OpenMoveBiaoji(int.Parse(x), int.Parse(y));
    }

    void GetMoveMsg(string status, string x, string y)
    {
        switch (status)
        {
            case "0":
                GameManager.Instance.state = State.Wait;
                GameUIManager.Instance.info.text = "等待对手打子";
                break;
            case "1":
                GameManager.Instance.state = State.Start;
                GameUIManager.Instance.info.text = "开始移动";
                break;
        }

        GameManager.Instance.MoveTo((int)GameManager.Instance.logicGrid.x, (int)GameManager.Instance.logicGrid.y, int.Parse(x), int.Parse(y),false);
        
        GameUIManager.Instance.FindColor();
        GameManager.Instance.Flash();
    }

    void GetDestroyMsg(string status, string x, string y)
    {
        switch (status)
        {
            case "0":
                GameManager.Instance.state = State.Wait;
                GameUIManager.Instance.info.text = "等待对手打子";
                break;
            case "1":
                GameManager.Instance.state = State.Start;
                GameUIManager.Instance.info.text = "开始移动";
                break;
        }

        GameManager.Instance.DestroyQizhi(int.Parse(x), int.Parse(y), false);
        GameUIManager.Instance.FindColor();
        GameManager.Instance.Flash();
    }
    public void onSendMessage(string message)
    {
        myActor.onSendMessage(message);
        GameUIManager.Instance.ChangeStatus();
    }
    #endregion

    #region Bluetooth
    void onBluetoothListeningStarted()
    {
        var useNat = !Network.HavePublicAddress();
        Network.InitializeServer(2, Common.Port, useNat);
    }
    void onBluetoothListeningCanceled()
    {
        BluetoothMultiplayerAndroid.Disconnect();
    }
    void onBluetoothAdapterEnabled()
    {
        switch (desiredMode)
        {
            case BluetoothMultiplayerMode.Server:
                Network.Disconnect();
                BluetoothMultiplayerAndroid.InitializeServer((ushort)Common.Port);
                break;
            case BluetoothMultiplayerMode.Client:
                Network.Disconnect();
                BluetoothMultiplayerAndroid.ShowDeviceList();
                break;
        }
        desiredMode = BluetoothMultiplayerMode.None;
    }
    void onBluetoothAdapterEnableFailed()
    {
    }
    void onBluetoothAdapterDisabled()
    { 
    }
    void onBluetoothConnectedToServer(BluetoothDevice device)
    {
        Network.Connect(Common.LocalIp, Common.Port);
    }
    void onBluetoothConnectToServerFailed(BluetoothDevice device)
    { 
    }
    void onBluetoothDisconnectedFromServer(BluetoothDevice device)
    {
        Network.Disconnect();
    }
    void onBluetoothClientConnected(BluetoothDevice device)
    {
    }
    void onBluetoothClientDisconnected(BluetoothDevice device)
    { 
    }
    void onBluetoothDevicePicked(BluetoothDevice device)
    {
        BluetoothMultiplayerAndroid.Connect(device.address, (ushort)Common.Port);
    }
    #endregion
}
