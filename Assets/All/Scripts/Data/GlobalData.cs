/*-------------------------
 *模块名：全局数据
 *创建者：Zo
 *创建日期：2015.6.13
 *模块描述：存储玩家在游戏时 产生的一些数据
 * --------------------------*/
using UnityEngine;
using System.Collections;

public class GlobalData : MonoBehaviour {
    public static GlobalData instance;

    public string player;

    public bool isClear = false;
    void Awake()
    {
        instance = this;
        if (isClear)
            PlayerPrefs.DeleteAll();

        ReadData();
    }

    public void ReadData()
    {
        player = PlayerPrefs.GetString("player");
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("player", player);
    }

    public void OnSure()
    {
        player = GameUIManager.Instance.inputName.text;
        if (player != "")
        {
            SaveData();
            GameUIManager.Instance.OpenMain();
        }
    }
}

