using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : API
{
    public bool Developement;
    public GameObject Canvas;
    public GameObject Reason;
    public static GameManager instance;
    GameInfo gameInfo;
    public GameObject PlayerPics;
    public int PlayerCount;
    public GameObject HostPopUp;
    public GameObject TimerCircle;
    Coroutine CurrentCoroutine;
    public GameObject Sound;
    public GameObject HelpPopUp;
    public GameObject[] Signs;
    public bool FirstDataPack = true;
    public bool FirstRoundPack;
    public GameObject ModeBar;
    public GameObject ExitPopUp;
    public GameObject Hands;
    public GameObject YouAreOut;
    public GameObject FinalBox;
    public GameObject[] Buttons;
    public Text Log;
    bool isPlaying;
    bool loadingPic = false;
    int RoundNr;
    float TimeRemaining;
    List<Participant> Participants = new List<Participant>();
    Status RoundStatus = Status.I;
    Sign SelectedSign;
    public GameObject test;
    enum Sign
    {
        Rock,
        Paper,
        Scissors,
        XLeg
    }
    enum Status
    {
        I,
        A,
        S,
        F
    }
    // Use this for initialization
    void Start()
    {
        GetHealth();
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                ChangeSoundPic(false);
            }
        }
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            instance.Reenable();
            Destroy(this);
        }
        FBLogIn.Instance.instance.Call("PreparationToShare");
        if (PusherClient.Pusher.IsHost)
        {
            HostPopUp.SetActive(true);
        }
        //StartCoroutine(Simulation());
        Debug.Log(FBLogIn.Instance.GetGameId());
        //!!!!!!!!!!!!!!
        //HelpPopUp.transform.GetChild(0).gameObject.GetComponent<Text>().text += FBLogIn.Instance.GetGameId() + Environment.NewLine + "FB ID " + FBLogIn.Instance.GetId();
        
        CanvasManager.gameManager = this;
        if (!PusherClient.Pusher.GetSubscribeStatus())
        {
            PusherClient.Pusher.Subscribe("GAME_" + FBLogIn.Instance.GetGameId());
        }
        PusherClient.Pusher.GetPusherData(PusherClient.Pusher.GetFirstDataPack());
    }
    void GetHealth()
    {
        GET("/health", null);
    }
    void Reenable()
    {
        Debug.Log("ReenableStart");
        Canvas.SetActive(true);
        Debug.Log("ReenableEnd");
    }
    IEnumerator Simulation()
    {
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(0).gameObject.GetComponent<Text>().text);
        yield return new WaitForSeconds(10f);
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(1).gameObject.GetComponent<Text>().text);
        yield return new WaitForSeconds(7f);
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(2).gameObject.GetComponent<Text>().text);
        StartTimer("1");
        yield return new WaitForSeconds(1.5f);
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(3).gameObject.GetComponent<Text>().text);
        yield return new WaitForSeconds(10f);
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(4).gameObject.GetComponent<Text>().text);
        yield return new WaitForSeconds(10f);
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(5).gameObject.GetComponent<Text>().text);
        yield return new WaitForSeconds(10f);
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(6).gameObject.GetComponent<Text>().text);
        yield return new WaitForSeconds(10f);
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(7).gameObject.GetComponent<Text>().text);
        yield return new WaitForSeconds(10f);
        PusherClient.Pusher.GetPusherData(test.transform.GetChild(8).gameObject.GetComponent<Text>().text);
    }

    public GameInfo GetGameInfo()
    {
        return gameInfo;
    }
    public void UpdateInfo()
    {
        StartTimer(gameInfo.next_event_in);
        switch (RoundStatus)
        {
            case Status.I:
                {
                    //AddPlayerPicture();
                    RefreshCircles();
                    break;
                }
            case Status.A:
                {
                    RefreshCircles();
                    break;
                }
            case Status.S:
                {
                    break;
                }
            case Status.F:
                {
                    break;
                }
        }
    }
    void RefreshCircles()
    {
        for (int i = 0; i < PlayerCount; i++)
        {
            if (Participants[i].current_selection != "N" && Participants[i].result != "F" && !Participants[i].ChangedCircle)
            {
                PlayerPics.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Frames/Green", typeof(Sprite));
                Participants[i].ChangedCircle = true;
            }
        }
    }
    public void AddPlayerPicture(int index)
    {
        Debug.Log("Inserting player " + index + " photo!");
        if (Participants.Count > PlayerCount)
        {
                if (!Participants[index].IsPhotoLoaded)
                {
                    PlayerPics.transform.GetChild(index).gameObject.GetComponent<PictureManager>().UploadPicture(Participants[index].profile_id);
                    Participants[index].IsPhotoLoaded = true;
                }
            
        }
    }
    private void GetPicture(IGraphResult result)
    {
        if (result.Texture != null)
        {
            PlayerPics.transform.GetChild(PlayerCount).gameObject.SetActive(true);
            PlayerPics.transform.GetChild(PlayerCount).GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f));
            PlayerCount++;
        }
    }
    public void SetGameInfo(GameInfo info)
    {
        gameInfo = info;
        SortParticipants(info);
        CheckRound();
    }
    public void ClosePopUp()
    {
        HostPopUp.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Game")
        {
            ShowExitPopUp();
        }
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
    IEnumerator Timer(float time)
    {
        if (time > TimeRemaining * 1.2 && time > 1)
        {
            TimerCircle.GetComponent<Image>().fillAmount = 1f;
        }
        TimeRemaining = time;
        if (time == 9999)
        {
            TimerCircle.GetComponent<Image>().fillAmount = 1f;

        }
        else
        {
            float partPerSecond = TimerCircle.GetComponent<Image>().fillAmount / time;
            while (TimeRemaining > 0)
            {
                float timeLeft = time;
                TimerCircle.GetComponent<Image>().fillAmount -= partPerSecond * Time.deltaTime;
                TimeRemaining -= Time.deltaTime;
                yield return null;
            }
            CurrentCoroutine = null;
            yield return null;
        }
    }
    public void StartTimer(string time)
    {
        if (CurrentCoroutine != null)
        {
            StopCoroutine(CurrentCoroutine);
        }
        CurrentCoroutine = StartCoroutine(Timer((float)Convert.ToDouble(time)));
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
    public void SelectSign(string sign)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("profile_id", FBLogIn.Instance.GetId());
        data.Add("round", gameInfo.play.round.number);
        data.Add("play", "1");
        data.Add("selection", sign.ToCharArray()[0].ToString());
        POST("/games/" + FBLogIn.Instance.GetGameId() + "/choose", data, null);
        foreach (GameObject obj in Signs)
        {
            if (sign.Equals(obj.name))
            {
                SelectedSign = (Sign)Enum.Parse(typeof(Sign), sign);
                obj.GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + obj.name + "Shine", typeof(Sprite));
            }
            else
            {
                obj.GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + obj.name + "Grey", typeof(Sprite));
            }
        }
    }
    public void CheckIfUserPlays()
    {
        for (int i = 0; i < gameInfo.play.round.participants.Count; i++)
        {
            if (gameInfo.play.round.participants[i].profile_id == FBLogIn.Instance.GetId())
            {
                Participants.Add(gameInfo.play.round.participants[i]);
                AddPlayerPicture(0);
                isPlaying = true;
                return;
            }
        }
        Debug.Log("User not in list. User Id: " + FBLogIn.Instance.GetId());
        DisableSigns();
    }
    void SortParticipants(GameInfo info)
    {
        if (FirstDataPack)
        {
            CheckIfUserPlays();
        }
        bool x;
        for (int i = 0; i < info.play.round.participants.Count; i++)
        {
            x = false;
            for (int j = 0; j < Participants.Count; j++)
            {
                if (Participants[j].profile_id == info.play.round.participants[i].profile_id)
                {
                    Participants[j] = info.play.round.participants[i];
                    x = true;
                    break;
                }
            }
            if (Participants.Count == 0)
            {
                Participants.Add(info.play.round.participants[i]);
                continue;
            }
            if (x)
            {
                continue;
            }
            Participants.Add(info.play.round.participants[i]);
            AddPlayerPicture(Participants.Count - 1);
        }
    }
    void SetGameMode()
    {
        if (gameInfo.type == "W")
        {
            ModeBar.GetComponent<Image>().sprite = (Sprite)Resources.Load("Buttons/Winner", typeof(Sprite));
        }
    }
    public void StartGame()
    {

        if (Participants.Count > 1)
        {


            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("profile_id", FBLogIn.Instance.GetId());
            data.Add("round", "1");
            data.Add("play", "1");
            POST("/games/" + FBLogIn.Instance.GetGameId() + "/start", data, TurnOnHands);
        }
    }
    void TurnOnHands()
    {
        Hands.SetActive(true);
    }
    void SetGameReason()
    {
        Reason.GetComponent<Text>().text = gameInfo.description;
    }
    void DisableSigns()
    {
        foreach (GameObject sign in Signs)
        {
            sign.GetComponent<Button>().interactable = false;
            sign.GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + sign.name + "Grey", typeof(Sprite));
        }
    }
    public void OnFirstDataPack()
    {
        SetGameMode();
        SetGameReason();
        FirstDataPack = false;
    }
    public void ShowExitPopUp()
    {
        if (ExitPopUp == null)
        {
            Debug.Log("ExitPopUp is null");
        }
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
    void RoundActiveStart()
    {
        RoundActiveSprites();

        if (RoundNr != 1)
        {
            ResetHands();

            ResetCircles();
        }
        else
        {
            TurnOnHands();
        }
        //StartTimer(gameInfo.next_event_in);
        FirstRoundPack = false;
    }
    void RoundShowDownStart()
    {
        StartCoroutine(ShowFullSigns());
        DisableSigns();
        ShowResult();
        PlayerDefeat();
        FirstRoundPack = false;
    }

    IEnumerator ShowFullSigns()
    {
        ShowSigns();
        yield return new WaitForSeconds(0.5f);
        ChangeHandColor();
        //yield return new WaitForSeconds(2.4f);
        //HideSigns();

    }
    void ShowResult()
    {
        if (Participants[0].current_selection != "N")
        {
            if (gameInfo.play.round.result == "D")
            {
                TimerCircle.transform.parent.GetChild(4).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Result/draw", typeof(Sprite));
            }
            else
            {
                if (gameInfo.type == "W")
                {
                    if (Participants[0].result == "C")
                    {
                        TimerCircle.transform.parent.GetChild(4).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Result/win", typeof(Sprite));
                    }
                    else
                    {
                        if (Participants[0].result == "F")
                        {
                            TimerCircle.transform.parent.GetChild(4).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Result/lose", typeof(Sprite));
                        }
                    }
                }
                else
                {
                    if (gameInfo.type == "L")
                    {
                        if (Participants[0].result == "F")
                        {
                            TimerCircle.transform.parent.GetChild(4).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Result/win", typeof(Sprite));
                        }
                        else
                        {
                            if (Participants[0].result == "C")
                            {
                                TimerCircle.transform.parent.GetChild(4).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Result/lose", typeof(Sprite));
                            }
                        }
                    }
                }
            }
            TimerCircle.transform.parent.GetChild(4).gameObject.SetActive(true);
        }
    }
    void ShowSigns()
    {
        for (int i = 0; i < Participants.Count; i++)
        {
            if (Participants[i].current_selection != "N")
            {
                Hands.transform.GetChild(i).gameObject.SetActive(true);
                Hands.transform.GetChild(i).gameObject.GetComponent<ShowHand>().ShowSign(Participants[i].current_selection);
            }
        }
    }
    void ChangeHandColor()
    {
        if (gameInfo.play.round.result == "D")
        {
            return;
        }
        if (gameInfo.type == "W")
        {
            for (int i = 0; i < Participants.Count; i++)
            {
                if (Participants[i].current_selection != "N")
                {
                    if (Participants[i].result == "F")
                    {
                        Hands.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + Hands.transform.GetChild(i).gameObject.GetComponent<Image>().sprite.name + "Grey", typeof(Sprite));
                    }
                    else
                    {
                        if (Participants[i].result == "C")
                        {
                            Hands.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + Hands.transform.GetChild(i).gameObject.GetComponent<Image>().sprite.name + "Shine", typeof(Sprite));
                        }
                    }
                }
            }
        }
        else
        {
            if (gameInfo.type == "L")
            {
                for (int i = 0; i < Participants.Count; i++)
                {
                    if (Participants[i].current_selection != "N")
                    {
                        if (Participants[i].result == "C")
                        {
                            Hands.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + Hands.transform.GetChild(i).gameObject.GetComponent<Image>().sprite.name + "Grey", typeof(Sprite));
                        }
                        else
                        {
                            if (Participants[i].result == "F")
                            {
                                Hands.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + Hands.transform.GetChild(i).gameObject.GetComponent<Image>().sprite.name + "Shine", typeof(Sprite));
                            }
                        }
                    }
                }
            }
        }
    }
    void HideSigns()
    {
        for (int i = 0; i < Participants.Count; i++)
        {
            if (Hands.transform.GetChild(i).gameObject.activeSelf)
            {
                if (Participants[i].result == "C")
                {
                    Hands.transform.GetChild(i).gameObject.GetComponent<ShowHand>().HideSign(true);
                }
                else
                {
                    Hands.transform.GetChild(i).gameObject.GetComponent<ShowHand>().HideSign(false);
                }
            }
        }
    }
    void RoundFinishStart()
    {
        FirstRoundPack = false;
        HideSigns();
    }
    public void OnRoundStart()
    {
        switch (RoundStatus)
        {
            case Status.I:
                {
                    break;
                }
            case Status.A:
                {
                    RoundActiveStart();
                    break;
                }
            case Status.S:
                {
                    RoundShowDownStart();
                    break;
                }
            case Status.F:
                {
                    RoundFinishStart();
                    break;
                }
        }
    }
    void CheckRound()
    {
        if (gameInfo.play.round.status != RoundStatus.ToString())
        {
            RoundStatus = (Status)Enum.Parse(typeof(Status), gameInfo.play.round.status);
            RoundNr = int.Parse(gameInfo.play.round.number);
            FirstRoundPack = true;
        }
    }
    void RoundActiveSprites()
    {
        if (RoundNr == 1)
        {
            TimerCircle.transform.parent.GetChild(2).gameObject.SetActive(false);
            TimerCircle.transform.parent.GetChild(3).gameObject.SetActive(true);
        }
        TimerCircle.transform.parent.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text = RoundNr.ToString();
        TimerCircle.transform.parent.GetChild(4).gameObject.SetActive(false);
    }
    void ResetHands()
    {
        if (isPlaying)
        {
            for (int i = 0; i < Signs.Length; i++)
            {
                Signs[i].GetComponent<Button>().interactable = true;
                Signs[i].GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + Signs[i].name, typeof(Sprite));
            }
        }
    }
    void ResetCircles()
    {
        for (int i = 0; i < Participants.Count; i++)
        {
            if (Participants[i].result != "F")
            {
                PlayerPics.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Frames/White", typeof(Sprite));
                Participants[i].ChangedCircle = false;
            }
        }
    }
    void PlayerDefeat()
    {
        for (int i = 0; i < Participants.Count; i++)
        {
            if (Participants[i].result == "F")
            {
                PlayerPics.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Frames/Red", typeof(Sprite));
                if (i == 0)
                {
                    if (gameInfo.type == "L")
                    {
                        YouAreOut.GetComponent<Image>().sprite = (Sprite)Resources.Load("Result/youAreSafe", typeof(Sprite));
                    }
                        YouAreOut.SetActive(true);
                    
                    foreach (GameObject sign in Signs)
                    {
                        sign.SetActive(false);
                    }
                    isPlaying = false;
                }
            }

        }
    }
    public void ShowFinalScreen()
    {
        FinalBox.SetActive(true);
        YouAreOut.SetActive(false);
        foreach (Participant participant in Participants)
        {
            if (participant.result == "C")
            {
                FB.API("/" + participant.profile_id, HttpMethod.GET, ChangeText);
                break;
            }
        }
        FinalBox.transform.GetChild(1).gameObject.GetComponent<Text>().text += gameInfo.description;
        foreach (GameObject button in Buttons)
        {
            button.SetActive(true);
        }
        foreach (GameObject sign in Signs)
        {
            sign.SetActive(false);
        }
    }
    void ChangeText(IGraphResult result)
    {
        IDictionary<string, object> dict = result.ResultDictionary;
        if (gameInfo.type == "W")
        {
            FinalBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = (string)dict["name"] + " is" + Environment.NewLine + "WINNER";
        }
        else
            if (gameInfo.type == "L")
        {
            FinalBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = (string)dict["name"] + " is" + Environment.NewLine + "LOSER";
        }
        else
        {
            Debug.Log("Invalid game type");
        }
        FBLogIn.Instance.instance.Call("drawMultilineTextToBitmap", FinalBox.transform.GetChild(0).gameObject.GetComponent<Text>().text + Environment.NewLine + FinalBox.transform.GetChild(1).gameObject.GetComponent<Text>().text);
    }
    public void ShareToMessenger()
    {
        FBLogIn.Instance.instance.Call("ShareInfo", gameInfo.id);
    }
    public void ChallengeOthers()
    {
        FBLogIn.Instance.instance.Call("ResetMpicking");
        Canvas.SetActive(false);
        SceneManager.LoadScene("Start");
    }
}
