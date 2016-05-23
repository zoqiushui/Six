
/*-------------------------
 *模块名：界面模块
 *创建者：Zo
 *创建日期：2015.6.10
 *模块描述：处理游戏各个界面显示和隐藏，以及面板信息的显示处理
 * --------------------------*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    public GameObject panelMain;
    public GameObject panelMenu;
    public GameObject panelGame;
    public GameObject panelHelp;

    public GameObject input;
    public GameObject main;
    public GameObject moveBiaoji;
    public InputField inputName;
    public Text status;
    public Text info;
    public Text qizhi;
    public Image leftIcon;
    public Image rightIcon;
    private Color black = new Color(0, 0, 0, 1);
    private Color white = new Color(1, 1, 1, 1);


    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        OpenMain();
    }

    void CloseAllPanel()                                    //关闭所有面板
    {
        panelMain.SetActive(false);
        panelMenu.SetActive(false);
        panelGame.SetActive(false);
        panelHelp.SetActive(false);
    }

    void OpenPanel(GameObject obj)                          //打开面板、对象
    {
        if (!obj.activeSelf)
            obj.SetActive(true);

    }                               

    public void SingleGame()                                //单人模式（未做）
    {
        CloseAllPanel();
        OpenPanel(panelGame);

        leftIcon.color = black;
        rightIcon.color = white;

        GameManager.Instance.gameType = GameType.Simple;
    }

    public void DoubleGame()                                //双人蓝牙对战
    {
        OpenMenu();

        GameManager.Instance.gameType = GameType.Double;
    }

    public void OpenMain()                                  //打开Main界面
    {
        CloseAllPanel();
        OpenPanel(panelMain);
        if (GlobalData.instance.name == "")
        {
            input.SetActive(true);
            main.SetActive(false);
        }
        else
        {
            input.SetActive(false);
            main.SetActive(true);
        }
    }

    public void OpenMenu()                                  //打开Menu界面
    {
        CloseAllPanel();
        OpenPanel(panelMenu);
    }

    public void OpenGame()                                  //打开Game界面
    {
        CloseAllPanel();
        OpenPanel(panelGame);
        SetIcon();
    }

    public void OpenHelp()                                  //打开Help界面
    {
        CloseAllPanel();
        OpenPanel(panelHelp);
    }

    public void OnCreate()                                  //创建对战房间
    {
        NetManager.Instance.CreateServer();
    }

    public void OnJion()                                    //加入对战
    {
        NetManager.Instance.ConnectToServer();
    }

    public void SetIcon()                                   //设置玩家图标
    {
        if (GameManager.Instance.side == 0)
        {
            leftIcon.color = black;
            rightIcon.color = white;
        }
        else
        {
            leftIcon.color = white;
            rightIcon.color = black;
        }
    }

    public void FindColor()                                 //查找所有棋子
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("color");
        qizhi.text = "当前有棋子：" + temp.Length.ToString();
    }

    public void ChangeStatus()                              //改变状态显示
    {
        switch (GameManager.Instance.state)
        { 
            case State.Wait:
                status.text = "等待";
                break;
            case State.Start:
                status.text = "开始";
                break;
            case State.Biaoji:
                status.text = "标记";
                break;
            case State.Choose:
                status.text = "选择";
                break;
        }
    }

    public void OpenMoveBiaoji(int x, int y)                //打开需要移动棋子的标记
    {
        moveBiaoji.SetActive(true);
        moveBiaoji.transform.SetParent(GameManager.Instance.FindQizhi(x, y).transform);
        moveBiaoji.transform.localPosition = Vector3.zero;
        moveBiaoji.transform.SetAsLastSibling();
    }

    public void CloseMoveBiaoji()                           //关闭移动标记
    {
        moveBiaoji.SetActive(false);
    }

    public void SetTitleInfo()                              //设置游戏标题信息
    { 
        
    }
}
