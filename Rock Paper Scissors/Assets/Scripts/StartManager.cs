using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : API
{
    public GameObject WinnerButton;
    public GameObject LoserButton;
    public GameObject TextBox;
    public GameObject SendButton;
    public GameObject Sound;
    public GameObject HelpPopUp;
    public GameObject ExitPopUp;
    //private string GameType;
    Sprite TempSprite;
    enum GameType
    {
        Blank,
        Winner,
        Loser
    }
    GameType gameType = GameType.Blank;

    void Start()
    {
        if(PlayerPrefs.GetInt("Sound")==0&&PlayerPrefs.HasKey("Sound"))
        {
            Sound.GetComponent<Image>().sprite = (Sprite)Resources.Load("Buttons/NoSound", typeof(Sprite));
        }
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (FBLogIn.Instance.GetGameId() == ""|| FBLogIn.Instance.GetGameId() == null)
            {
                ShowExitPopUp();
            }
            else
            {
                Debug.Log(FBLogIn.Instance.GetGameId());
                SceneManager.LoadScene("Game");
            }
        }
    }
    string ToChar(GameType x)
    {
        return x.ToString().ToCharArray()[0].ToString();
    }
    public void testmeta()
    {
        FBLogIn.Instance.instance.Call("MetaTest");
    }
    public void SendToMessenger()
    {

        Dictionary<string, string> info = new Dictionary<string, string>();
        info.Add("profile_id", FBLogIn.Instance.GetId());
        info.Add("description", TextBox.transform.GetChild(1).gameObject.GetComponent<Text>().text);
        info.Add("type", ToChar(gameType));
        PusherClient.Pusher.IsHost = true;
        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.Canvas);
            Destroy(GameManager.instance);
            FBLogIn.Instance.instance.Call("resetShareImage");
        }
        POST("/games", info, JoinGame);
        
    }
    private void JoinGame()
    {
        string id = JsonUtility.FromJson<GameCreateResult>(Results).id;
        FBLogIn.Instance.SetGameId(id);
        PusherClient.Pusher.Subscribe("GAME_" + id);
        Debug.Log("Id before " + id);
        FBLogIn.Instance.instance.Call("SendMessage", id);
        Debug.Log("Id after " + id);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("profile_id", FBLogIn.Instance.GetId());
        POST("/games/"+id+ "/join",data,LoadGame);
    }
    void LoadGame()
    {
        Debug.Log("Join callback " + Results);
        PusherClient.Pusher.SetFirstDataPack(Results);
        SceneManager.LoadScene("Game");
    }
    public void SelectWinner()
    {
        //change of sprite
        if (gameType != GameType.Winner)
        {
            if (TempSprite != null)
                LoserButton.GetComponent<Image>().sprite = TempSprite;
            TempSprite = WinnerButton.GetComponent<Image>().sprite;
        }

        WinnerButton.GetComponent<Image>().sprite = (Sprite)Resources.Load("Buttons/Winner", typeof(Sprite));
        if (gameType == GameType.Blank)
        {
            TextBox.SetActive(true);
            SendButton.SetActive(true);
        }
        gameType = GameType.Winner;
    }
    public void SelectLoser()
    {


        //change of sprite
        if (gameType != GameType.Loser)
        {
            if (TempSprite != null)
                WinnerButton.GetComponent<Image>().sprite = TempSprite;
            TempSprite = LoserButton.GetComponent<Image>().sprite;
        }

        LoserButton.GetComponent<Image>().sprite = (Sprite)Resources.Load("Buttons/Loser", typeof(Sprite));
        if (gameType == GameType.Blank)
        {
            TextBox.SetActive(true);
            SendButton.SetActive(true);
        }
        gameType = GameType.Loser;
    }
    public void SwitchSound()
    {
        if (PlayerPrefs.GetInt("Sound") == 0 && PlayerPrefs.HasKey("Sound"))
        {
            SoundManager.instance.TurnOnSound();
            ChangeSoundPic(true);
        }
        else
        {
            SoundManager.instance.TurnOffSound();
            ChangeSoundPic(false);
        }
    }
    void ChangeSoundPic(bool soundStatus)
    {
        if (soundStatus)
        {
            Sound.GetComponent<Image>().sprite = (Sprite)Resources.Load("Buttons/Sound", typeof(Sprite));
        }
        else
        {
            Sound.GetComponent<Image>().sprite = (Sprite)Resources.Load("Buttons/NoSound", typeof(Sprite));
        }
    }
    public void ShowHelp()
    {
        HelpPopUp.SetActive(true);
    }
    public void CloseHelp()
    {
        HelpPopUp.SetActive(false);
    }
    public void ShowExitPopUp()
    {
        ExitPopUp.SetActive(true);
    }
    public void CloseExitPopUp()
    {
        ExitPopUp.SetActive(false);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
