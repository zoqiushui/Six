/*-------------------------
 *模块名：帮助
 *创建者：Zo
 *创建日期：2015.6.17
 *模块描述：为游戏玩法和 规则做介绍
 * --------------------------*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour {

    public GameObject[] item;
    public Text btn_Continue;
    private int count = 0;

	void Start () {
	
	}

    void OnEnable()
    {
        InitHelp();
    }

    void InitHelp()
    {
        count = 0;
        item[0].SetActive(true);
        for (int i = 1; i < item.Length; i++)
        {
            item[i].SetActive(false);
        }
        btn_Continue.text = "继续";
    }

    public void OnContinue()
    {
        count++;
        if (count < item.Length - 1)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (i == count)
                    item[i].SetActive(true);
                else
                    item[i].SetActive(false);
            }
        }
        else
        {
            if (count == item.Length - 1)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    if (i == count)
                        item[i].SetActive(true);
                    else
                        item[i].SetActive(false);
                }
                btn_Continue.text = "确定";
            }
            else
            {
                Quit();
            }
        }
    }

    public void Quit()
    {
        this.gameObject.SetActive(false);
        GameUIManager.Instance.OpenMain();
    }
}
