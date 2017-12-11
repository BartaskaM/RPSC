using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowHand : MonoBehaviour
{
    float outX = -1f;
    float outY = -1f;
    float inX = -1f;
    float inY = -1f;
    // Use this for initialization
    void Start()
    {

    }
    public void ShowSign(string name)
    {
        switch (name)
        {
            case "R":
                {
                    name = "Rock";
                    break;
                }
            case "S":
                {
                    name = "Scissors";
                    break;
                }
            case "P":
                {
                    name = "Paper";
                    break;
                }
        }
        GetComponent<Image>().sprite = (Sprite)Resources.Load("Signs/" + name, typeof(Sprite));
        StartCoroutine(Slide(0.3f, true, false));
    }
    public void HideSign(bool delayed)
    {
        StartCoroutine(Slide(0.3f, false, delayed));
    }
    IEnumerator Slide(float time, bool slideIn, bool delayed)
    {

        if (inX == -1f)
        {
            inX = GetComponent<RectTransform>().anchorMax.x;
            inY = GetComponent<RectTransform>().anchorMax.y;
            if (GetComponent<RectTransform>().anchorMax.x > 0.5f)
            {
                outX = 1f;
            }
            else
            if (GetComponent<RectTransform>().anchorMax.x < 0.5f)
            {
                outX = 0f;
            }
            else
            {
                outX = 0.5f;
            }
            if (GetComponent<RectTransform>().anchorMax.y > 0.5f)
            {
                outY = 1f;
            }
            else
            if (GetComponent<RectTransform>().anchorMax.y < 0.5f)
            {
                outY = 0f;
            }
            else
            {
                outY = 0.5f;
            }

            GetComponent<RectTransform>().anchorMax = new Vector2(outX, outY);
            GetComponent<RectTransform>().anchorMin = new Vector2(outX, outY);
        }
        //yield return new WaitForSeconds(2f);
        float x;
        float y;
        float xDistance;
        float yDistance;
        if (slideIn)
        {
            xDistance = inX - outX;
            yDistance = inY - outY;
            x = outX;
            y = outY;
        }
        else
        {
            xDistance = outX - inX;
            yDistance = outY - inY;
            x = inX;
            y = inY;
            if (delayed)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
        float stepX = xDistance / time;
        float stepY = yDistance / time;
        float currX = 0f;
        float currY = 0f;
        while (System.Math.Abs(currX) < System.Math.Abs(xDistance) || System.Math.Abs(currY) < System.Math.Abs(yDistance))
        {
            x += stepX * Time.deltaTime;
            currX += stepX * Time.deltaTime;
            y += stepY * Time.deltaTime;
            currY += stepY * Time.deltaTime;
            GetComponent<RectTransform>().anchorMax = new Vector2(x, y);
            GetComponent<RectTransform>().anchorMin = new Vector2(x, y);
            yield return null;
        }
        if (!slideIn)
        {
            gameObject.SetActive(false);
        }
    }
}
