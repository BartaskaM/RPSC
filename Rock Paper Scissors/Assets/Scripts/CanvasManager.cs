using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {
    public static GameManager gameManager;
	// Use this for initialization
	void Awake () {
        Debug.Log("awake");
        if (gameManager != null)
            Destroy(gameObject);
        DontDestroyOnLoad(this);
	}
}
