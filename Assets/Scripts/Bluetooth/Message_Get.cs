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

    public void ProcessMessage(byte[] data)
    {
        MyData reader = new MyData(data, 0);
        int messageId = reader.Read_Int();
        switch (messageId)
        { 
            case (int) MessageId.ConnectToServer:
                ConnectCallBack(data);
                break;
            case (int)MessageId.Chat:
                ChatCallBack(data);
                break;
            default:
                break;
        }
    }

    void ConnectCallBack(byte[] data)
    {
        MyData reader = new MyData(data,4);
        ConnectToServer handler = new ConnectToServer();
        handler.result = reader.Read_Byte();

    }

    void ChatCallBack(byte[] data)
    {
        MyData reader = new MyData(data,4);
        Chat handler = new Chat();
        short playerLength = reader.Read_Short();
        handler.player = reader.Read_String(playerLength);
        short msgLength = reader.Read_Short();
        handler.msg = reader.Read_String(msgLength);

        MyDebug.instance.Log("MSG:" + handler.msg);
        MainUIManager.instance.TestAsy();
    }

}
