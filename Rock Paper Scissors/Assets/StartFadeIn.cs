using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartFadeIn : MonoBehaviour
{
    public static StartFadeIn Instance;
    void OnEnable()
    {
        for (int i = 1; i < 5; i++)
        {
            StartCoroutine(FadeIn(transform.GetChild(i).gameObject.GetComponent<Image>()));
        }
        Instance = this;
    }
    IEnumerator FadeIn(Image spriteRend)
    {
        Color tempClr = spriteRend.color;
        tempClr.a = 0f;
        while (tempClr.a <= 1f)
        {
            tempClr.a += 0.01f;
            spriteRend.color = tempClr;
            yield return null;
        }
    }
    public void SpawnSendButtons()
    {
        for (int i = 5; i < 7; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            StartCoroutine(FadeIn(transform.GetChild(i).gameObject.GetComponent<Image>()));
        }
    }
}
