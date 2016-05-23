/*-------------------------
 *模块名：Debug模块
 *创建者：Zo
 *创建日期：2015.6.11
 *模块描述：由于蓝牙连接在电脑上Log不出信息 所以需要
 *新建一个Text提供在真机上 Log信息 提供调试
 * --------------------------*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MyLog : MonoBehaviour
{
    public static MyLog Instance;
    public Text log;
    void Awake()
    {
        Instance = this;
    }
    void Start () {
        log.gameObject.SetActive(true);
	}
    public void Log(string info)
    {
        log.text = info;
    }
}

