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
        short short1 = (short)Encoding.UTF8.GetByteCount(player);
        short short2 = (short)Encoding.UTF8.GetByteCount(message);
        int length = 8 + short1 + short2;
        MyData sender = new MyData(length);
        sender.Write_Int((int)MessageId.Chat);
        sender.Write_Short(short1);
        sender.Write_String(player);
        sender.Write_Short(short2);
        sender.Write_String(message);

        if (BluetoothManager.instance.myActor != null)
            BluetoothManager.instance.myActor.OnSendMessage(sender.dataForSend);
        else
        {
            MyDebug.instance.Log("Actor is null");
            BluetoothManager.instance.FindActor();
        }
    }
}
