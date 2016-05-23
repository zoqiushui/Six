using System;
using System.Collections;
using UnityEngine;
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    private bool isLoadingScene = false;
    private float progress;

    public delegate void LoadSceneProgressHandler(float progress);
    public event LoadSceneProgressHandler LoadSceneProgressHandlerEvent; //加载场景进度事件
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        isLoadingScene = false;
        progress = 0;
    }

    /// <summary>
    /// 根据场景名字加载场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    public void LoadSceneByName(string sceneName)
    {
        if (isLoadingScene)
            return;

        progress = 0;
        isLoadingScene = true;
        StartCoroutine(UnloadUselessResource(sceneName));
    }

    /// <summary>
    /// 卸载旧场景加载的资源
    /// </summary>
    private IEnumerator UnloadUselessResource(string sceneWillBeLoad)
    {
        //HiLog.MyLog.Log("Unload resource assets");
        AsyncOperation tempUselessResource = Resources.UnloadUnusedAssets();

        while (!tempUselessResource.isDone)
            yield return new WaitForSeconds(0.1f);
        GC.Collect();
        //HiLog.MyLog.Log("Unload resource assets done.");
        StartCoroutine(LoadSceneWithProgress(sceneWillBeLoad));
    }

    /// <summary>
    /// 加载新场景
    /// </summary>
    private IEnumerator LoadSceneWithProgress(string sceneIsLoading)
    {
        //HiLog.MyLog.Log("load scene.");
        AsyncOperation tempLoadResource = Application.LoadLevelAsync(sceneIsLoading);
        tempLoadResource.allowSceneActivation = false;
        while (!tempLoadResource.isDone && progress < 0.9f)
        {
            LoadSceneProgress(tempLoadResource.progress);
            yield return null;
        }
        LoadSceneProgressHandlerEvent = null;
        //HiLog.MyLog.Log("load scene done.");
        isLoadingScene = false;
        tempLoadResource.allowSceneActivation = true;//跳转场景
    }

    /// <summary>
    /// 加载场景进度值
    /// </summary>
    void LoadSceneProgress(float value)
    {
        progress = value;
        if (LoadSceneProgressHandlerEvent != null)
            LoadSceneProgressHandlerEvent(progress);
    }
}
