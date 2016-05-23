using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GridButton : MonoBehaviour {
    private string grid;        //按钮的名字
    public Text xiabiao;        //对应的下标显示


    [HideInInspector]
    public GameObject qizhi;    //存储生成的棋子

    private int x;              //下标  横坐标
    private int y;              //下标  纵坐标

	void Start () {
        grid = transform.name;
        x = (int)GetGrid().x;
        y = (int)GetGrid().y;

        xiabiao = transform.GetChild(0).gameObject.GetComponent<Text>();
	}

    //按钮点击时 调用
    public void OnClick()
    {
        if (GameManager.Instance.gameType == GameType.Double)
        {
            NetClickButton();
        }
        else
        {
            SingleClickButton();
        }
    }

    //通过按钮本身的name 返回2维坐标
    Vector2 GetGrid()
    {
        return new Vector2(int.Parse(grid.Split(',')[0]),int.Parse(grid.Split(',')[1]));
    }

    //连接对战时点击
    void NetClickButton()
    {
        if (GameManager.Instance.state == State.Wait)
        {
            MyLog.Instance.Log("等.......");
        }
        else if (GameManager.Instance.state == State.Start)
        {
            if (GameManager.Instance.grid[x, y] == -1)
            {
                if (!GameManager.Instance.isMove)
                {
                    MyLog.Instance.Log("落棋" + grid);
                    GameManager.Instance.GenerateQizhi(GameManager.Instance.side, x, y, true);
                }
                else
                {
                    GameManager.Instance.MoveQizhi(x, y);
                }
            }
            else
            {
                if (GameManager.Instance.isMove)
                {
                    if (GameManager.Instance.side == GameManager.Instance.grid[x, y])
                        GameManager.Instance.ChooseLogic(x, y);
                }
            }
        }
        else if (GameManager.Instance.state == State.Biaoji)
        {
            if (!GameManager.Instance.isMove)
            {
                if (GameManager.Instance.side == 0 && GameManager.Instance.grid[x, y] == 1)
                {
                    if(!GameManager.Instance.IsProtect(x,y,1))
                        GameManager.Instance.BiaojiQizhi(3, x, y, true);
                    else
                        GameUIManager.Instance.info.text = "该棋子受到保护不可被标记";
                }
                else if (GameManager.Instance.side == 1 && GameManager.Instance.grid[x, y] == 0)
                {
                    if (!GameManager.Instance.IsProtect(x, y, 0))
                        GameManager.Instance.BiaojiQizhi(2, x, y, true);
                    else
                        GameUIManager.Instance.info.text = "该棋子受到保护不可被标记";
                }
            }
            else
            {
                if (GameManager.Instance.side == 0 && GameManager.Instance.grid[x, y] == 1)
                    GameManager.Instance.DestroyQizhi(x, y, true);
                else if (GameManager.Instance.side == 1 && GameManager.Instance.grid[x, y] == 0)
                    GameManager.Instance.DestroyQizhi(x, y, true);
            }
        }
        else if (GameManager.Instance.state == State.Choose)
        {
            MyLog.Instance.Log("请选择一个棋子");
        }
    }

    //单人时点击 （未做）
    void SingleClickButton()
    {
        Debug.Log("-------点击棋子-------" + transform.name);
    }
}
