using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureManager : MonoBehaviour {

    public void UploadPicture(string profileId)
    {
        FB.API("/" + profileId + "/picture", HttpMethod.GET, GetPicture);
    }
    private void GetPicture(IGraphResult result)
    {
        if (result.Texture != null)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f));
            GameManager.instance.PlayerCount++;
        }
    }
}
