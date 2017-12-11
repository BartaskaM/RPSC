using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this);
        if (PlayerPrefs.GetInt("Sound") == 1 || !PlayerPrefs.HasKey("Sound"))
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
    public void TurnOffSound()
    {
        gameObject.GetComponent<AudioSource>().Stop();
        PlayerPrefs.SetInt("Sound", 0);
    }
    public void TurnOnSound()
    {
        gameObject.GetComponent<AudioSource>().Play();
        PlayerPrefs.SetInt("Sound", 1);
    }
}
