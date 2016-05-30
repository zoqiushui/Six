using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class uCenterOnChild : MonoBehaviour
{
    public int screenItem = 3;
    ScrollRect scrollRect;
    GameObject mCenterObject;

    public GameObject centeredObject { get { return mCenterObject; } }


    void OnEnable() { Recenter(); }

    public void Recenter()
    {
        
    }

    private bool isMove = false;
    private float valuechange = 0;
    public void OnValueChange(Vector2 temp)
    {
        float tt = temp.x - valuechange;
        Debug.Log("value" + tt.ToString());
        valuechange = temp.x;
    }
}
