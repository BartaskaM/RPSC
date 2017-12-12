using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FBLogIn : API
{
    public static FBLogIn Instance;
    private static AccessToken Token;
    private static Texture2D ProfilePic;
    public GameObject LogInSprites;
    public static bool mPicking = false;
    string GameId;
    AndroidJavaClass _class;
    public AndroidJavaObject instance { get { return _class.GetStatic<AndroidJavaObject>("instance"); } }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
#if UNITY_EDITOR
        LogInSprites.transform.GetChild(1).gameObject.SetActive(true);
        LogInSprites.transform.GetChild(2).gameObject.SetActive(true);
        LogInSprites.transform.GetChild(3).gameObject.SetActive(true);
#endif
        _class = new AndroidJavaClass("matbar.messenger.messenger");
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
            if (FB.IsLoggedIn)
            {
                _class.CallStatic("start", gameObject.name);
                instance.Call("GetFlow");
            }
            else
            {
                LogInSprites.transform.GetChild(1).gameObject.SetActive(true);
                LogInSprites.transform.GetChild(2).gameObject.SetActive(true);
                LogInSprites.transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }

    public AccessToken GetToken()
    {
        return Token;
    }
    public void UnityDebug(string message)
    {
        Debug.Log("Debug from android: "+message);
    }
    public void OnPluginInit(string state)
    {
        Debug.Log("Metadata: " + state);
        string[] data = state.Split('/');
        foreach (string dat in data)
        {
            Debug.Log("Split metadata " + dat);
        }
        if (data[0] == "true")
        {
            mPicking = true;
            GameId = data[1];
        }
        SelectScene();
        Debug.Log(mPicking);
    }
    public void OnMessageSent(string gameId)
    {
        Debug.Log(gameId);
    }
    private void InitCallback()
    {

        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            if (FB.IsLoggedIn)
            {

                _class.CallStatic("start", gameObject.name);
                instance.Call("GetFlow");
            }
            else
            {
                LogInSprites.transform.GetChild(1).gameObject.SetActive(true);
                LogInSprites.transform.GetChild(2).gameObject.SetActive(true);
                LogInSprites.transform.GetChild(3).gameObject.SetActive(true);
            }

        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    public void LogIn()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene("Game");
#endif
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "user_friends" }, AuthCallback);
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("profile_id", AccessToken.CurrentAccessToken.UserId);
            Debug.Log(result.ResultDictionary);
            //data.Add("user_name", result.ResultDictionary["first_name"].ToString());
            POST("/users", data, null);
            _class.CallStatic("start", gameObject.name);
            instance.Call("GetFlow");
            GetPlayerInfo();
        }
    }
    private void GetPicture(IGraphResult result)
    {
        if (result.Texture != null)
        {
            ProfilePic = result.Texture;
        }
    }
    private void GetPlayerInfo()
    {
        Token = AccessToken.CurrentAccessToken;
        FB.API("/me/picture", HttpMethod.GET, GetPicture);
    }

    private void SelectScene()
    {
        if (mPicking)
        {
            Instance.GetPlayerInfo();
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("profile_id", Token.UserId);
            Debug.Log("Game id: " + GameId);
            POST("/games/" + GameId + "/join", data, JoinCallback);
        }
        else
        {
            Instance.GetPlayerInfo();
            SceneManager.LoadScene("Start");
        }
    }
    private void JoinCallback()
    {
        PusherClient.Pusher.SetFirstDataPack(Results);
        SceneManager.LoadScene("Game");
    }
    public string GetGameId()
    {
        return GameId;
    }
    public void SetGameId(string id)
    {
        GameId = id;
    }
    public string GetId()
    {
        return Token.UserId;
    }
}
