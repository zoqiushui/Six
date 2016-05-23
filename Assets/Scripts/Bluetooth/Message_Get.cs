using HiData;
using System.Collections.Generic;

public class Message_Get
{
    private static Message_Get instance;
    public static Message_Get Instance()
    {
        if (instance == null)
            instance = new Message_Get();
        return instance;
    }

    public void ProcessMessage(string message)
    {
        int messageId = int.Parse(message.Split('/')[0]);
        switch (messageId)
        { 
            case (int) MessageId.ConnectToServer:
                ConnectCallBack(message);
                break;
            case (int)MessageId.Chat:
                ChatCallBack(message);
                break;
            default:
                break;
        }
    }

    void ConnectCallBack(string message)
    {
        //MyData reader = new MyData(data);
        //ConnectToServer handler = new ConnectToServer();
        //handler.result = reader.Read_Byte();

    }

    void ChatCallBack(string message)
    {
        //MyData reader = new MyData(data);
        //Chat handler = new Chat();
        //int id = reader.Read_Int();
        //short playerLength = reader.Read_Short();
        //handler.player = reader.Read_String(playerLength);
        //short msgLength = reader.Read_Short();
        //handler.msg = reader.Read_String(msgLength);
        Chat handler = new Chat();
        handler.player = message.Split('/')[1];
        handler.msg = message.Split('/')[2];
        MyDebug.instance.Log("Player:" + handler.player + ";msg:" + handler.msg);
    }

}
