using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LogInFadeIn : MonoBehaviour
{

    // Use this for initialization
    public void Fade()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            StartCoroutine(FadeIn(transform.GetChild(i).gameObject.GetComponent<Image>()));
        }
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
}
