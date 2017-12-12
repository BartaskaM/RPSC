using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlideIn : MonoBehaviour
{

    public bool FromTop = false;
    public float Delay;
    Vector3 StartPoint;
    public float Speed;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Slide(Delay));
    }
    IEnumerator Slide(float delay)
    {
        StartPoint = gameObject.GetComponent<RectTransform>().position;
        if (!FromTop)
        {
            gameObject.GetComponent<RectTransform>().position -= new Vector3(0f, Screen.height / 2 + gameObject.GetComponent<Image>().sprite.bounds.size.y / 2, 0f);
        }
        else
        {
            gameObject.GetComponent<RectTransform>().position += new Vector3(0f, Screen.height / 2 + gameObject.GetComponent<Image>().sprite.bounds.size.y / 2, 0f);
        }
        yield return new WaitForSeconds(delay);
        Vector3 step = (StartPoint - gameObject.GetComponent<RectTransform>().position) * Speed;

        if (!FromTop)
        {
            while (gameObject.GetComponent<RectTransform>().position.y < StartPoint.y)
            {
                gameObject.GetComponent<RectTransform>().position += step;
                yield return null;
            }
        }
        else
        {
            while (gameObject.GetComponent<RectTransform>().position.y > StartPoint.y)
            {
                gameObject.GetComponent<RectTransform>().position += step;
                yield return null;
            }
        }
    }

}
