//----------------------------
// 蓝牙创建服务器和连接到服务器时
// 会Network.Instan... 实例化该对象 供双方
// 收发消息时调用
//----------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Actor : MonoBehaviour {

	void Start () {
	
	}
    /// <summary>
    /// 发出消息时调用 
    /// </summary>
    /// <param name="message"></param>
    public void onSendMessage(string message)
    {
        if (GetComponent<NetworkView>().isMine)
            GetComponent<NetworkView>().RPC("ReciveMessage", RPCMode.Others, message);
    }
    /// <summary>
    /// 接收消息调用
    /// </summary>
    /// <param name="message"></param>
    [RPC]
    void ReciveMessage(string message)
    {
        NetManager.Instance.ReciveMessage(message);
    }
}
