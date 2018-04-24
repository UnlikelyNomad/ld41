using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : InputHandler {

    public string gameName = "Vault Defense Command";

    public List<string> menu = new List<string>();
    public List<string> help = new List<string>();

    public CommandLog log;

    public float lineDelay = 0.2f;

    public float dimSpeed = 0.1f;

    public enum MenuMode {
        MAIN,
        OPTIONS,
        COLOR,
        SOUND,
        MUSIC,
        HELP
    };

    public MenuMode menuMode = MenuMode.MAIN;

    public AudioClip printerClip;

	// Use this for initialization
	void Start () {
        if (!UIController.Instance.alphaRamped) {
            for (int i = 0; i < UIController.Instance.groups.Length; ++i) {
                UIController.Instance.groups[i].alpha = 0;
            }

            AudioSource src = GetComponent<AudioSource>();
            src.PlayOneShot(printerClip);
        }

        log = CommandLog.Instance;
        log.handler = this;

        menuMode = MenuMode.MAIN;

        UIController.Instance.MenuMode(true);
        if (!UIController.Instance.alphaRamped) {
            StartCoroutine(UpdateAlpha());
        } else {
            StartCoroutine(MainMenu());
        }
	}

    IEnumerator ShowHelp() {
        log.InputEnabled = false;

        int i = 0;
        while (i < help.Count) {

            if (help[i].CompareTo("[more]") == 0) {
                log.More();

                while (log.waitMore) {
                    yield return null;
                }
            } else {
                log.AddLine(help[i]);

                yield return new WaitForSeconds(lineDelay);
            }

            i++;
        }

        StartCoroutine(MainMenu());
    }

    IEnumerator MainMenu() {
        menuMode = MenuMode.MAIN;
        log.InputEnabled = false;

        int i = 0;

        while (i < menu.Count) {
            log.AddLine(menu[i]);
            i++;

            yield return new WaitForSeconds(lineDelay);
        }

        log.InputEnabled = true;
    }

    IEnumerator OptionsMenu() {
        log.InputEnabled = false;
        menuMode = MenuMode.OPTIONS;

        log.AddLine("Options Menu:");
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("1) Color");
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("2) Typing Sounds");
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("3) Music Volume");
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("");
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("Enter choice:");

        log.InputEnabled = true;
    }

    IEnumerator ColorMenu() {
        log.InputEnabled = false;
        menuMode = MenuMode.COLOR;

        int i = 0;
        while (i < GameOptions.Instance.colorOptions.Length) {
            

            string l = (i + 1) + ") " + GameOptions.Instance.colorOptions[i].name;
            log.AddLine(l);
            i++;
            yield return new WaitForSeconds(lineDelay);
        }

        log.AddLine("  Enter color number:");
        log.InputEnabled = true;
    }

    IEnumerator SoundMenu() {
        log.InputEnabled = false;
        menuMode = MenuMode.SOUND;

        log.AddLine("Typing sounds currently: " + (GameOptions.Instance.typingSounds ? "ENABLED" : "DISABLED"));
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("1) Enable");
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("2) Disable");
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("Enter choice:");

        log.InputEnabled = true;
    }

    IEnumerator MusicMenu() {
        log.InputEnabled = false;
        menuMode = MenuMode.MUSIC;

        log.AddLine("Current volume level: " + GameOptions.Instance.musicVolume);
        yield return new WaitForSeconds(lineDelay);
        log.AddLine("Enter new volume level (0-100):");

        log.InputEnabled = true;
    }

    public override void HandleInput(string command) {
        switch (menuMode) {
        case MenuMode.HELP:
            break;

        case MenuMode.MAIN:
            MainInput(command);
            break;

        case MenuMode.OPTIONS:
            OptionsInput(command);
            break;

        case MenuMode.COLOR:
            ColorInput(command);
            break;

        case MenuMode.SOUND:
            SoundInput(command);
            break;

        case MenuMode.MUSIC:
            MusicInput(command);
            break;
        }
    }

    void MainInput(string command) {
        switch (command) {
        case "1":
            UIController.Instance.MenuMode(false);
            SceneManager.LoadScene(1);
            break;

        case "2":
            StartCoroutine(OptionsMenu());
            break;

        case "3":
            StartCoroutine(ShowHelp());
            break;

        case "4":
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
            break;
        }
    }

    void OptionsInput(string command) {
        try {
            int n = int.Parse(command);
            switch (n) {
            case 1:
                StartCoroutine(ColorMenu());
                break;

            case 2:
                StartCoroutine(SoundMenu());
                break;

            case 3:
                StartCoroutine(MusicMenu());
                break;
            }
        } catch {
            log.AddLine("Invalid choice");
        }
    }

    void SoundInput(string command) {
        switch (command) {
        case "1":
            GameOptions.Instance.typingSounds = true;
            break;

        case "2":
            GameOptions.Instance.typingSounds = false;
            break;

        default:
            log.AddLine("Invalid choice");
            return;
        }

        StartCoroutine(MainMenu());
    }

    void MusicInput(string command) {
        try {
            int v = int.Parse(command);
            if (v >= 0 && v <= 100) {
                GameOptions.Instance.musicVolume = v;
                GameObject.FindGameObjectWithTag("MusicLoop").GetComponent<AudioSource>().volume = (v / 100f);
                StartCoroutine(MainMenu());
                return;
            }
        } catch {
        }

        log.AddLine("Invalid value (0-100).");
    }

    void ColorInput(string command) {
        //Debug.Log(command);

        char[] t = { ' ' };
        string[] tokens = command.Split(t);

        try {
            int i = int.Parse(tokens[0]) - 1;
            Color c = GameOptions.Instance.colorOptions[i].col;
            GameOptions.Instance.MainColor = c;

            ColorOnStart.UpdateAllColors();

            StartCoroutine(MainMenu());

        } catch {
            log.AddLine("  Invalid color selection.");
        }
    }

    IEnumerator UpdateAlpha() {
        log.InputEnabled = false;

        for (int i = 0; i < UIController.Instance.groups.Length; ++i) {
            while (UIController.Instance.groups[i].alpha < 1f) {
                UIController.Instance.groups[i].alpha += dimSpeed * Time.deltaTime;

                yield return null;
            }

            UIController.Instance.groups[i].alpha = 1f;
        }
        
        log.AddLine("Welcome to " + gameName);
        UIController.Instance.alphaRamped = true;

        StartCoroutine(MainMenu());
    }
}
