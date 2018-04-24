using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandLog : MonoBehaviour {

    public List<string> commands = new List<string>();
    public List<string> logLines = new List<string>();

    public InputHandler handler;

    public int maxLogLines = 25;

    public Text inputLine;
    public Text log;

    public int lastCmdOffset = 0;

    public string prompt = ">";
    public string caret = "_";
    public float caretInterval = 0.5f;
    float caretPeriod = 0;

    public bool _inputEnabled = false;
    public bool InputEnabled {
        get {
            return _inputEnabled;
        }

        set {
            _inputEnabled = value;
            if (value) {
                Command = _command;
            } else {
                inputLine.text = "";
            }
        }
    }

    bool caretVis = false;

    public AudioClip typingSound;
    public AudioSource typingSource;

    public bool waitMore = false;

    static CommandLog _instance = null;
    public static CommandLog Instance {
        get {
            return _instance;
        }
    }

    string _command = "";
    public string Command {
        get { return _command; }
        set {
            _command = value;
            if (_inputEnabled) {
                inputLine.text = prompt + value + (caretVis ? caret : "");
            } else {
                inputLine.text = "";
            }
        }
    }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
        typingSource = GetComponent<AudioSource>();
        log = UIController.Instance.log;
        inputLine = UIController.Instance.input;
        Command = "";
	}

    public void More() {
        InputEnabled = false;
        AddLine("  [Press any key for more...]");
        waitMore = true;
    }
	
	// Update is called once per frame
	void Update () {
        InputUI();
	}

    void InputUI() {
        if (_inputEnabled) {
            caretPeriod -= Time.deltaTime;

            if (caretPeriod <= 0) {
                caretVis = !caretVis;
                Command = _command;
                caretPeriod += caretInterval;
            }
        }


        if (!waitMore) {
            foreach (char c in Input.inputString) {
                //Debug.Log((int)c);

                switch (c) {
                case '\b':
                    if (_command.Length > 0) {
                        Command = _command.Substring(0, _command.Length - 1);
                    }
                    break;

                case '\n':
                case '\r':
                //Debug.Log("enter");
                    HandleInput();
                    Command = "";
                    break;

                default:
                    Command = _command + c;
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                Command = PrevCommand();
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                Command = NextCommand();
            }
        }

        if (Input.anyKeyDown) {

            if (GameOptions.Instance.typingSounds) {
                float v = Random.Range(0.8f, 1.1f);
                float p = Random.Range(0.8f, 2f);

                typingSource.pitch = p;
                typingSource.PlayOneShot(typingSound, v);
            }

            if (waitMore) {
                waitMore = false;
            }
        }
    }

    void HandleInput() {
        commands.Add(_command);
        AddLine(prompt + _command);
        lastCmdOffset = 0;
        handler.HandleInput(_command);
    }

    public void AddLine(string line) {
        logLines.Add(line);
        while (logLines.Count > maxLogLines) {
            logLines.RemoveAt(0);
        }

        string t = "";
        for (int i = 0; i < logLines.Count; ++i) {
            t += "\r\n" + logLines[i]; 
        }

        log.text = t;
    }

    public string PrevCommand() {
        ++lastCmdOffset;
        if (lastCmdOffset > commands.Count) lastCmdOffset = commands.Count;

        string c = commands[commands.Count  - lastCmdOffset];
        return c;
    }

    public string NextCommand() {
        string c = "";
        --lastCmdOffset;
        if (lastCmdOffset < 0) lastCmdOffset = 0;

        try {
            c = commands[commands.Count - lastCmdOffset];
        } catch {
        }

        return c;
    }
}
