using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions : MonoBehaviour {

    static GameOptions _instance;
    public static GameOptions Instance {
        get { return _instance; }
    }

    public Color _mainColor;

    public bool typingSounds = true;

    public int musicVolume = 100;

    List<string> randomLines = new List<string>();

    [System.Serializable]
    public struct ColorOptions {
        public string name;
        public Color col;
    }

    public ColorOptions[] colorOptions;

    public Color MainColor {
        get {
            return _mainColor;
        }

        set {
            _mainColor = value;
        }
    }

	// Use this for initialization
	void Awake () {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
	}

    public void RandomLine() {
        if (randomLines.Count == 0) {
            randomLines.AddRange(Responses.random);
        }

        int i = Random.Range(0, randomLines.Count);
        CommandLog.Instance.AddLine("COMPUTER: " + randomLines[i]);
        randomLines.RemoveAt(i);
    }
}
