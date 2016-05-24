//----------------------------
// 蓝牙创建服务器和连接到服务器时
// 实例化该对象 供双方
// 收发消息时调用
//----------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ActorRPC : MonoBehaviour {

    private NetworkView networkView;

	void Start () {
        networkView = GetComponent<NetworkView>();
	}

    public void OnSendMessage(byte[] data)
    {
        if(networkView == null)
            networkView = GetComponent<NetworkView>();
        if (networkView.isMine)
        {
            networkView.RPC("onGetMessage", RPCMode.Others, data);
        }
    }

    [RPC]
    void onGetMessage(byte[] data)
    {
        Message_Get.Instance().ProcessMessage(data);
    }
}
