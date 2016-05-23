/*-------------------------
 *模块名：逻辑模块
 *创建者：Zo
 *创建日期：2015.6.10
 *模块描述：游戏的逻辑部分 （检测，玩法 算法）
 * --------------------------*/

using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏类型   （目前只写蓝牙双人玩法）
/// </summary>
public enum GameType
{ 
    Simple = 0,     //单人简单
    Normal = 1,     //单人正常
    Master = 2,     //单人大师
    Double = 3,     //蓝牙对战（双人）
}

/// <summary>
/// 玩家状态
/// </summary>
public enum State
{ 
    Null   = -1,                 //空
    Wait   = 0,                 //等待
    Start  = 1,                 //开始
    Biaoji = 2,                 //标记
    Choose = 3,                 //选择棋子
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject actor;        //            prefab
    public GameObject black;        //0  黑子     prefab 
    public GameObject white;        //1  白子     prefab
    public GameObject black_C;      //2  标记黑子 prefab
    public GameObject white_C;      //3  标记白子 prefab

    public State state = State.Wait;
    [HideInInspector]
    public int side = -1;           //0 server,1 client

    public GameType gameType = GameType.Double;

    public bool isMove = false;     //false 下子 ; true 移动
    public int[,] grid;             //-1 null; 0 black; 1 white; 2 black_C; 3 white_C

    private int clearCount = 0;     //标记数
    private int destroyCount = 0;   //删除数
    private bool[] isHaveSix;       //已存在的连6
    private int[] checkSix;
    public Vector2 logicGrid = new Vector2(-1,-1);

    void Awake()
    {
        Instance = this;
        
    }
	void Start () {
        InitGrid();
	}

    void InitGrid()
    {
        grid = new int[6, 6];
        isHaveSix = new bool[6] { false, false, false, false, false, false };
        checkSix = new int[6] { 0, 0, 0, 0, 0, 0 };
        isMove = false;
        clearCount = 0;
        destroyCount = 0;
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = -1;

        GameUIManager.Instance.CloseMoveBiaoji();
    }

    // 根据下标找到该棋子
    public GameObject FindQizhi(int x, int y)
    {
        GameObject temp = null;
        temp = GameObject.Find(x.ToString() + "," + y.ToString());
        return temp;
    }

    // 生成对应类型的棋子，以及是否需要检测 
    public void GenerateQizhi(int type,int x, int y,bool needCheck)
    {
        GameObject temp = null;
        switch (type)
        { 
            case 0:
                temp = (GameObject)Instantiate(black, Vector3.zero, Quaternion.identity) as GameObject;
                break;

            case 1:
                temp = (GameObject)Instantiate(white, Vector3.zero, Quaternion.identity) as GameObject;
                break;

        }
        GameObject target = FindQizhi(x, y);

        temp.transform.SetParent(target.transform);
        temp.transform.localPosition = Vector3.zero;
        temp.transform.localScale = new Vector3(1, 1, 1);
        temp.transform.SetAsFirstSibling();
        grid[x, y] = type;
        target.GetComponent<GridButton>().qizhi = temp;

        if (needCheck)
        {
            GenerateCheck(x, y,type);
        }
    }

    // 生成后 检测是否存在可标记
    void GenerateCheck(int i,int j,int type)
    {
        string msg = "";
        if (Check(i, j, type, false)) //检测是否有成立项
        {
            int temp = clearCount;
            clearCount = 0;

            if (IsHaveFree()) //是否存在自由的棋子
            {
                state = State.Biaoji;
                msg = "Grid,0," + i.ToString() + "," + j.ToString();

                GameUIManager.Instance.info.text = "可标记对方" + temp.ToString() + "颗棋子";
            }
            else
            {
                state = State.Biaoji;
                msg = "Grid,3," + i.ToString() + "," + j.ToString();

                MyLog.Instance.Log("对方没有可标记的棋子");

                GameUIManager.Instance.info.text = "等待对方标记受保护的棋子";
            }

            clearCount = temp;
        }
        else
        {
            if (!IsHaveGrid()) //棋盘是否被下满
            {
                msg = "Move_Grid,1," + i.ToString() + "," + j.ToString();
                isMove = true;
                ClearBiaoji();
                GameUIManager.Instance.info.text = "等待对方移动";
            }
            else
            {
                state = State.Wait;
                msg = "Grid,1," + i.ToString() + "," + j.ToString();
                GameUIManager.Instance.info.text = "等待对方落棋";
            }
        }

        NetManager.Instance.onSendMessage(msg);

        GameUIManager.Instance.FindColor();
        Flash();
        
    }

    //标记
    public void BiaojiQizhi(int type, int x,int y,bool sendMsg)
    {
        GameObject target = FindQizhi(x,y);
        Destroy(target.GetComponent<GridButton>().qizhi.gameObject);
        grid[x, y] = -1;

        GameObject temp = null;
        switch (type)
        { 
            case 2:
                temp = (GameObject)Instantiate(black_C, Vector3.zero, Quaternion.identity) as GameObject;
                break;
            case 3:
                temp = (GameObject)Instantiate(white_C, Vector3.zero, Quaternion.identity) as GameObject;
                break;
        }
        temp.transform.SetParent(target.transform);
        temp.transform.localPosition = Vector3.zero;
        temp.transform.localScale = new Vector3(1, 1, 1);
        temp.transform.SetAsFirstSibling();
        grid[x, y] = type;
        target.GetComponent<GridButton>().qizhi = temp;
        
        if (sendMsg)
        {
            clearCount--;

            string msg = "";
            if (clearCount > 0)
            {
                msg = "Biaoji,0," + x.ToString() + "," + y.ToString();

                GameUIManager.Instance.info.text = "可标记对方" + clearCount.ToString() + "颗棋子";
            }
            else
            {
                state = State.Wait;
                clearCount = 0;

                if (!IsHaveGrid())
                {
                    msg = "Move_Biaoji,1," + x.ToString() + "," + y.ToString();
                    isMove = true;
                    ClearBiaoji();
                    GameUIManager.Instance.info.text = "等待对方移动";
                }
                else
                {
                    msg = "Biaoji,1," + x.ToString() + "," + y.ToString();
                    GameUIManager.Instance.info.text = "等待对方落棋";
                }
            }

            MyLog.Instance.Log(msg);
            NetManager.Instance.onSendMessage(msg);
        }

        GameUIManager.Instance.FindColor();
        Flash();
    }

    //清楚被标记的棋子
    public void ClearBiaoji()
    { 
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == 2 || grid[i, j] == 3)
                {
                    GameObject target = FindQizhi(i, j);
                    Destroy(target.GetComponent<GridButton>().qizhi.gameObject);
                    grid[i, j] = -1;
                }
            }
    }

    //选择需要移动的棋子
    public void ChooseLogic(int x,int y)
    {
        logicGrid = new Vector2(x, y);
        GameUIManager.Instance.OpenMoveBiaoji(x, y);
        string msg = "Choose_Move,0," + x.ToString() + "," + y.ToString();

        GameUIManager.Instance.info.text = "选择移动棋子";
        MyLog.Instance.Log(msg);
        NetManager.Instance.onSendMessage(msg);

    }

    //移动棋子
    public void MoveQizhi(int x,int y)
    {
        if (CheckCanMove((int)logicGrid.x, (int)logicGrid.y, x, y))
        {
            MoveTo((int)logicGrid.x, (int)logicGrid.y, x, y,true);

            string msg = "";
            if (Check(x, y, side, false))
            {
                state = State.Biaoji;
                msg = "Move,0," + x.ToString() + "," + y.ToString();
                GameUIManager.Instance.info.text = "可消除对方"+destroyCount.ToString()+"颗棋子";
            }
            else
            {
                state = State.Wait;
                msg = "Move,1," + x.ToString() + "," + y.ToString();
                GameUIManager.Instance.info.text = "移动...";
            }

            MyLog.Instance.Log(msg);
            NetManager.Instance.onSendMessage(msg);
        }
    }

    //移动
    public void MoveTo(int x1,int y1,int x2, int y2,bool isSelf)
    {
        GameObject btn1 = FindQizhi(x1, y1);
        GameObject btn2 = FindQizhi(x2, y2);
        GameObject qizhi = btn1.GetComponent<GridButton>().qizhi;

        btn1.GetComponent<GridButton>().qizhi = null;
        btn2.GetComponent<GridButton>().qizhi = qizhi;
        grid[x1, y1] = -1;

        if (isSelf)
            grid[x2, y2] = side;
        else
        {
            grid[x2, y2] = (side == 0) ? 1 : 0;
        }
        qizhi.transform.SetParent(btn2.transform);
        MoveCompleted();

        Hashtable temp = new Hashtable();
        temp.Add("position",Vector3.zero);
        temp.Add("time", 0.1f);
        temp.Add("islocal", true);
        temp.Add("easetype", iTween.EaseType.linear);
        temp.Add("oncomplete", "MoveCompleted");
        iTween.MoveTo(qizhi, temp);
    }

    //移动完成
    void MoveCompleted()
    {
        logicGrid = new Vector2(-1, -1);
        GameUIManager.Instance.CloseMoveBiaoji();

        GameUIManager.Instance.FindColor();
        GameManager.Instance.Flash();
    }

    //删除对应下标的棋子
    public void DestroyQizhi(int x, int y,bool sendMsg)
    {
        GameObject target = FindQizhi(x, y);
        Destroy(target.GetComponent<GridButton>().qizhi.gameObject);
        grid[x, y] = -1;

        if (sendMsg)
        {
            destroyCount--;

            string msg = "";
            if (destroyCount > 0)
            {
                msg = "Destroy,0," + x.ToString() + "," + y.ToString();

                GameUIManager.Instance.info.text = "可删除对方" + clearCount.ToString() + "颗棋子";
            }
            else
            {
                state = State.Wait;
                destroyCount = 0;

                msg = "Destroy,1," + x.ToString() + "," + y.ToString();

                GameUIManager.Instance.info.text = "等待对方移动";
            }

            MyLog.Instance.Log(msg);
            NetManager.Instance.onSendMessage(msg);
        }

        GameUIManager.Instance.FindColor();
        Flash();
    }

    //删除所有的棋子
    public void DestroyAllQizhi()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != -1)
                {
                    DestroyQizhi(i, j, false);
                }
            }
    }

    //是否可移动？
    bool CheckCanMove(int x1,int y1,int x2, int y2)
    {
        if (x1 == x2 && Mathf.Abs(y1 - y2) == 1)
            return true;
        else if (y1 == y2 && Mathf.Abs(x1 - x2) == 1)
            return true;
        else
            return false;
    }

    //------------tttttttttttttttttttttttttttt测试移动被选择的棋子
    public void Flash()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                GameObject temp = FindQizhi(i, j);
                temp.transform.GetComponent<GridButton>().xiabiao.text = grid[i, j].ToString();
            }
        }
    }

    #region 检测逻辑

    public bool Check(int i,int j,int type,bool justCheck)
    {
        for (int x = 0; x < checkSix.Length; x++)
            checkSix[x] = 0;
        clearCount = 0;
        destroyCount = 0;

        CheckSquare(i, j, type);
        CheckOblique3(i, j, type);
        CheckOblique4(i, j, type);
        CheckOblique5(i, j, type);
        CheckOblique6(i, j, type);
        Checkline(i, j, type);

        if (!isMove)
        {
            if (justCheck)
            {
                if(clearCount > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                clearCount -= Dazhou(checkSix);

                if(clearCount > 0)
                    return true;
                else
                    return false;
            }
                
        }else {
            if (justCheck)
            {
                if (destroyCount > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                destroyCount -= Dazhou(checkSix);

                if (destroyCount > 0)
                    return true;
                else
                    return false;
            }
        }
    }

    //口
    private void CheckSquare(int x, int y, int type)
    {
        #region x==0
        if (x == 0)
        {
            if (y == 0)
                if (grid[x, y] == type && grid[x + 1, y] == type &&
                    grid[x + 1, y + 1] == type && grid[x, y + 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            if(y == 5)
                if (grid[x, y] == type && grid[x + 1, y] == type &&
                     grid[x + 1, y - 1] == type && grid[x, y - 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            if (y > 0 && y < 5)
            {
                if (grid[x, y] == type && grid[x + 1, y] == type &&
                     grid[x + 1, y - 1] == type && grid[x, y - 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
                if (grid[x, y] == type && grid[x + 1, y] == type &&
                        grid[x + 1, y + 1] == type && grid[x, y + 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            }

        }
        #endregion
        #region x == 5
        if (x == 5)
        {
            if (y == 0)
            {
                if (grid[x, y] == type && grid[x - 1, y] == type &&
                        grid[x - 1, y + 1] == type && grid[x, y + 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            }
            if (y == 5)
            {
                if (grid[x, y] == type && grid[x - 1, y] == type &&
                        grid[x - 1, y - 1] == type && grid[x, y - 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            }
            if (y > 0 && y < 5)
            {
                if (grid[x, y] == type && grid[x - 1, y] == type &&
                        grid[x - 1, y + 1] == type && grid[x, y + 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
                if (grid[x, y] == type && grid[x - 1, y] == type &&
                        grid[x - 1, y - 1] == type && grid[x, y - 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            }
        }
        #endregion
        #region x>0  x<5 
        if (x > 0 && x < 5)
        {
            if (y == 0)
            {
                if (grid[x, y] == type && grid[x - 1, y] == type &&
                        grid[x - 1, y + 1] == type && grid[x, y + 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
                if (grid[x, y] == type && grid[x + 1, y] == type &&
                        grid[x + 1, y + 1] == type && grid[x, y + 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            }
            if (y == 5)
            {
                if (grid[x, y] == type && grid[x - 1, y] == type &&
                        grid[x - 1, y - 1] == type && grid[x, y - 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
                if (grid[x, y] == type && grid[x + 1, y] == type &&
                        grid[x + 1, y - 1] == type && grid[x, y - 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            }
            if (y > 0 && y < 5)
            {
                if (grid[x, y] == type && grid[x - 1, y] == type &&
                        grid[x - 1, y + 1] == type && grid[x, y + 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
                if (grid[x, y] == type && grid[x + 1, y] == type &&
                        grid[x + 1, y + 1] == type && grid[x, y + 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
                if (grid[x, y] == type && grid[x - 1, y] == type &&
                        grid[x - 1, y - 1] == type && grid[x, y - 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
                if (grid[x, y] == type && grid[x + 1, y] == type &&
                        grid[x + 1, y - 1] == type && grid[x, y - 1] == type)
                {
                    if (!isMove)
                        clearCount++;
                    else
                        destroyCount++;
                }
            }
        }
        #endregion
    }

    //斜3
    private void CheckOblique3(int i, int j, int type)
    {
        if ((i == 0 && j == 2) || (i == 1 && j == 1) || (i == 2 && j == 0))
        {
            if (grid[0, 2] == type && grid[1, 1] == type && grid[2, 0] == type)
            {
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;
            }
        }
        if ((i == 0 && j == 3) || (i == 1 && j == 4) || (i == 2 && j == 5))
        {
            if (grid[0, 3] == type && grid[1, 4] == type && grid[2, 5] == type)
            {
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;
            }
        }
        if ((i == 3 && j == 0) || (i == 4 && j == 1) || (i == 5 && j == 2))
        {
            if (grid[3, 0] == type && grid[4, 1] == type && grid[5, 2] == type)
            {
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;
            }
        }
        if ((i == 3 && j == 5) || (i == 4 && j == 4) || (i == 5 && j == 3))
        {
            if (grid[3, 5] == type && grid[4, 4] == type && grid[5, 3] == type)
            {
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;
            }
        }
    }

    //斜4
    private void CheckOblique4(int i, int j, int type)
    {
        //左下
        if ((i == 0 && j == 3) || (i == 1 && j == 2) || (i == 2 && j == 1) || (i == 3 && j == 0))
            if (grid[0, 3] == type && grid[1, 2] == type && grid[2, 1] == type && grid[3, 0] == type)
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;

        //左上
        if ((i == 0 && j == 2) || (i == 1 && j == 3) || (i == 2 && j == 4) || (i == 3 && j == 5))
            if (grid[0, 2] == type && grid[1, 3] == type && grid[2, 4] == type && grid[3, 5] == type)
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;

        //右下
        if ((i == 2 && j == 0) || (i == 3 && j == 1) || (i == 4 && j == 2) || (i == 5 && j == 3))
            if (grid[2, 0] == type && grid[3, 1] == type && grid[4, 2] == type && grid[5, 3] == type)
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;

        //右上
        if ((i == 2 && j == 5) || (i == 3 && j == 4) || (i == 4 && j == 3) || (i == 5 && j == 2))
            if (grid[2, 5] == type && grid[3, 4] == type && grid[4, 3] == type && grid[5, 2] == type)
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;
    }

    //斜5
    private void CheckOblique5(int i, int j, int type)
    {
        //左下
        if ((i == 0 && j == 4) || (i == 1 && j == 3) || (i == 2 && j == 2) || (i == 3 && j == 1) || (i == 4 && j == 0))
            if (grid[0, 4] == type && grid[1, 3] == type && grid[2, 2] == type && grid[3, 1] == type && grid[4, 0] == type)
                if (!isMove)
                    clearCount += 2;
                else
                    destroyCount += 2;

        //左上
        if ((i == 0 && j == 1) || (i == 1 && j == 2) || (i == 2 && j == 3) || (i == 3 && j == 4) || (i == 4 && j == 5))
            if (grid[0, 1] == type && grid[1, 2] == type && grid[2, 3] == type && grid[3, 4] == type && grid[4, 5] == type)
                if (!isMove)
                    clearCount += 2;
                else
                    destroyCount += 2;

        //右下
        if ((i == 1 && j == 0) || (i == 2 && j == 1) || (i == 3 && j == 2) || (i == 4 && j == 3) || (i == 5 && j == 4))
            if (grid[1, 0] == type && grid[2, 1] == type && grid[3, 2] == type && grid[4, 3] == type && grid[5, 4] == type)
                if (!isMove)
                    clearCount += 2;
                else
                    destroyCount += 2;

        //右上
        if ((i == 1 && j == 5) || (i == 2 && j == 4) || (i == 3 && j == 3) || (i == 4 && j == 2) || (i == 5 && j == 1))
            if (grid[1, 5] == type && grid[2, 4] == type && grid[3, 2] == type && grid[4, 2] == type && grid[5, 1] == type)
                if (!isMove)
                    clearCount += 2;
                else
                    destroyCount += 2;
    }

    //斜6
    private void CheckOblique6(int i, int j, int type)
    {
        if (i == j)
            if (grid[0, 0] == type && grid[1, 1] == type && grid[2, 2] == type && grid[3, 3] == type && grid[4, 4] == type && grid[5, 5] == type)
            {
                if (!isMove)
                    clearCount += 3;
                else
                    destroyCount += 3;

                checkSix[0] = 1;
            }

        if(i + j == 5)
            if (grid[0, 5] == type && grid[1, 4] == type && grid[2, 3] == type && grid[3, 2] == type && grid[4, 1] == type && grid[5, 0] == type)
            {
                if (!isMove)
                    clearCount += 3;
                else
                    destroyCount += 3;

                checkSix[1] = 1;
            }
    }

    //横竖
    private void Checkline(int i, int j, int type)
    {
        if(i == 0 || i == 5)
            if (grid[i, 0] == type && grid[i, 1] == type && grid[i, 2] == type && grid[i, 3] == type && grid[i, 4] == type && grid[i, 5] == type)
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;

        if(i == 1 || i == 4)
            if (grid[i, 0] == type && grid[i, 1] == type && grid[i, 2] == type && grid[i, 3] == type && grid[i, 4] == type && grid[i, 5] == type)
                if (!isMove)
                    clearCount += 2;
                else
                    destroyCount += 2;
        if (i == 2 || i == 3)
            if (grid[i, 0] == type && grid[i, 1] == type && grid[i, 2] == type && grid[i, 3] == type && grid[i, 4] == type && grid[i, 5] == type)
            {
                if (!isMove)
                    clearCount += 3;
                else
                    destroyCount += 3;

                checkSix[i] = 1;
            }

        if(j == 0 || j == 5)
            if (grid[0, j] == type && grid[1, j] == type && grid[2, j] == type && grid[3, j] == type && grid[4, j] == type && grid[5, j] == type)
                if (!isMove)
                    clearCount++;
                else
                    destroyCount++;
        if (j == 1 || j == 4)
            if (grid[0, j] == type && grid[1, j] == type && grid[2, j] == type && grid[3, j] == type && grid[4, j] == type && grid[5, j] == type)
                if (!isMove)
                    clearCount += 2;
                else
                    destroyCount += 2;
        if (j == 2 || j == 3)
            if (grid[0, j] == type && grid[1, j] == type && grid[2, j] == type && grid[3, j] == type && grid[4, j] == type && grid[5, j] == type)
            {
                if (!isMove)
                    clearCount += 3;
                else
                    destroyCount += 3;

                checkSix[i+2] = 1;
            }
    }

    //空余格子
    private bool IsHaveGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                if (grid[i, j] == -1)
                    return true;


        return false;
    }

    //检测对方是否存在空闲棋子
    //提供标记 或者 删除
    private bool IsHaveFree()
    {
        int type = (side == 0) ? 1 : 0;

        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == type)
                {
                    if (!IsProtect(i, j, type))
                        return true;
                }
            }
        return false;
    }
    //保护
    public bool IsProtect(int x, int y ,int type)
    {
        if (Check(x, y, type, true))
            return true;

        return false;
    }

    #endregion

    //处理已经存在的斜6 消除数量 
    int Dazhou(int[] six)
    {
        int temp = 0;
        for (int i = 0; i < six.Length; i++)
        {
            if (six[i] == 1)
            {
                if (!isHaveSix[i])
                    isHaveSix[i] = true;
                else
                    temp += 3;
            }

        }
        return temp;
    }
}
