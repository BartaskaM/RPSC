using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PusherClient : MonoBehaviour
{
    public bool IsHost;
    AndroidJavaClass _class;
    public AndroidJavaObject instance { get { return _class.GetStatic<AndroidJavaObject>("instance"); } }
    public static PusherClient Pusher;
    private string FirstDataPack;
    private bool IsSubscribed=false;
    // Use this for initialization
    void Start()
    {
        if (Pusher == null)
        {
            Pusher = this;
        }
        else
            Destroy(this);
        DontDestroyOnLoad(this);
        _class = new AndroidJavaClass("com.matbar.pusher.PusherManager");
        javaStart();
    }
    //public void StartSimulation()
    //{
    //    StartCoroutine(Simulation());
    //}
    //IEnumerator Simulation()
    //{
    //    GetPusherData(GameManager.instance.test.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text);
    //    yield return new WaitForSeconds(5f);
    //    GetPusherData(GameManager.instance.test.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text);
    //    yield return new WaitForSeconds(5f);
    //    GetPusherData(GameManager.instance.test.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text);
    //    GameManager.instance.StartTimer("1");
    //    yield return new WaitForSeconds(1f);
    //    GetPusherData(GameManager.instance.test.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Text>().text);
    //    yield return new WaitForSeconds(5f);
    //    GetPusherData(GameManager.instance.test.transform.GetChild(4).gameObject.GetComponent<UnityEngine.UI.Text>().text);
    //    yield return new WaitForSeconds(5f);
    //    GetPusherData(GameManager.instance.test.transform.GetChild(5).gameObject.GetComponent<UnityEngine.UI.Text>().text);
    //    yield return new WaitForSeconds(5f);
    //    GetPusherData(GameManager.instance.test.transform.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Text>().text);
    //    yield return new WaitForSeconds(5f);
    //    GetPusherData(GameManager.instance.test.transform.GetChild(7).gameObject.GetComponent<UnityEngine.UI.Text>().text);
    //    yield return new WaitForSeconds(5f);
    //}
    private void OnDestroy()
    {
        instance.Call("Unsubscribe");
    }
    public bool GetSubscribeStatus()
    {
        return IsSubscribed;
    }
    public void javaStart()
    {
        _class.CallStatic("start", gameObject.name);
    }
    public void Subscribe(string channelName)
    {
        instance.Call("Subscribe", channelName);
        IsSubscribed = true;
    }
    public void GetPusherData(string data)
    {
        StartCoroutine(UpdateData(data));
    }
    IEnumerator UpdateData(string data)
    {
        while(SceneManager.GetActiveScene().name!="Game")
        {
            yield return null;
        }
        GameInfo info = JsonUtility.FromJson<GameInfo>(data);
        GameManager.instance.Log.text += "info " + info.play.round.status + " " + info.next_event_in + Environment.NewLine;
        GameManager.instance.SetGameInfo(info);
        Debug.Log(JsonUtility.ToJson(info));
        if (info.status == "F")
        {
            GameManager.instance.ShowFinalScreen();
        }

        if (GameManager.instance.FirstDataPack)
        {
            GameManager.instance.OnFirstDataPack();
        }
        if (GameManager.instance.FirstRoundPack)
        {
            GameManager.instance.OnRoundStart();
        }
        GameManager.instance.UpdateInfo();
    }
    public void StartTimer(string data)
    {

        GameStarting info = JsonUtility.FromJson<GameStarting>(data);
        GameManager.instance.Log.text += "game-starting " + info.starts_in + Environment.NewLine;
        GameManager.instance.StartTimer(info.starts_in);
    }
    public void SetFirstDataPack(string x)
    {
        FirstDataPack = x;
    }
    public string GetFirstDataPack()
    {
        return FirstDataPack;
    }
}
