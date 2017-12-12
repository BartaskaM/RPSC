using UnityEngine;

public class Scaler : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Rescale();

    }
    void Rescale()
    {
        float width = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect.width;
        float height = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect.height;
        float diffWidth = width / gameObject.GetComponent<RectTransform>().rect.width;
        float diffHeight = height / gameObject.GetComponent<RectTransform>().rect.height;
        float multp = diffHeight;
        if (diffWidth < diffHeight)
        {
            multp = diffWidth;
        }
        gameObject.transform.localScale *= multp;
        transform.parent.GetChild(transform.GetSiblingIndex() + 1).localScale *= multp;
    }
}
