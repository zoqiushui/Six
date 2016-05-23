#region
//*------------------------
//*模块名：蓝牙传输协议
//*创建者：Zo
//*创建日期：2016.05.11
//*模块描述：传输协议消息头
//* --------------------------
#endregion

using System.Collections.Generic;

public enum MessageId
{ 
    ConnectToServer = 1,

    Chat = 100,
}

/// <summary>
/// 连接到服务器
/// </summary>
public class ConnectToServer
{
    public byte result; //1 成功；0失败
    public ConnectToServer()
    {
        result = 0;
    }
}

/// <summary>
/// 聊天 （私聊）
/// </summary>
public class Chat
{
    public string player;
    public string msg;          //消息

    public Chat()
    {
        player = "";
        msg = "";
    }
}
