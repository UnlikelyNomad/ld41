using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    static UIController _instance;
    public static UIController Instance {
        get { return _instance; }
    }

    public Text log;
    public Text input;

    public CanvasGroup[] groups;

    public bool alphaRamped = false;

    public GameObject logo;

    public GameObject icons;

	// Use this for initialization
	void Awake () {
        if (_instance == null) {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        } else {
            Destroy(gameObject);
        }
	}

    public void MenuMode(bool menu) {
        logo.SetActive(menu);
        icons.SetActive(!menu);
    }
}
