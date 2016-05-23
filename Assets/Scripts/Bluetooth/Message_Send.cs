using HiData;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class Message_Send
{
    private static Message_Send instance;
    public static Message_Send Instance()
    {
        if (instance == null)
            instance = new Message_Send();
        return instance;
    }

    public void Chat(string player,string message)
    {
        string sender = ((int)MessageId.Chat).ToString() +"/"+ player + "/" + message;
        if (BluetoothManager.instance.myActor != null)
            BluetoothManager.instance.myActor.OnSendMessage(sender);
        else
        {
            MyDebug.instance.Log("Actor is null");
            BluetoothManager.instance.FindActor();
        }
    }
}
