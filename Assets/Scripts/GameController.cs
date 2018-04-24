using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : InputHandler {

    private static GameController _instance = null;
    public static GameController Instance {
        get {
            return _instance;
        }
    }

    public CommandLog log;

    public List<string> droneNames = new List<string>();

    List<string> names = new List<string>();

    public List<Camera> cameras = new List<Camera>();

    Drone activeDrone = null;
    public List<Drone> drones = new List<Drone>();

    public List<GameObject> mobPrefabs = new List<GameObject>();

    public float difficulty = 0f;
    public float difficultyRate = 0.001f;

    public float healthMultiplier = 1f;
    public float healthRate = 0.02f;

    public List<Transform> spawns;
    public float spawnPeriod = 4f;
    public float spawnChange;
    public float lastSpawn = 0;
    public Transform mobParent;

    public float logDelay = 0.2f;
    public float curDelay = 0f;
    public int droneIndex = 0;

    public Transform droneParent;
    public GameObject dronePrefab;

    public int curDir = 0;

    public GameObject turretPrefab;

    public Text droneCount;
    public Text crapCount;

    public int crap = 20;

    public int camIndex = 0;

    public Sprite[] batteryIcons;

    float maxCore;
    public float curCore = 100f;

    public int turretCost = 4;
    public float upgradeMultiplier = 1f;

    public int droneCost = 10;
    public int numDrones = 1;

    public Image batteryImage;

    public AudioSource src;
    public AudioClip coreDamageSound;

    public int totalCrap = 0;

    public int turretSpend = 0;
    public int upgradeSpend = 0;
    public int repairSpend = 0;

    int secretCount = 0;
    int totalSecrets = 10;

    [System.Serializable]
    public struct CommandInfo {
        public string name;
        public string info;
    }

    public List<CommandInfo> commands = new List<CommandInfo>();

    //array index is hallway, key is station
    Dictionary<int, Turret>[] turrets = new Dictionary<int, Turret>[4];

    // Use this for initialization
    void Awake () {

        //setup singleton access
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
        }

        Color c = GameOptions.Instance.MainColor;
        c.r *= 0.1f;
        c.g *= 0.1f;
        c.b *= 0.1f;
        RenderSettings.ambientLight = c;

        for (int i = 0; i < 4; ++i) {
            turrets[i] = new Dictionary<int, Turret>();
        }
    }

    void Start() {
        UIController.Instance.MenuMode(false);

        log = CommandLog.Instance;
        log.handler = this;

        droneCount = GameObject.FindGameObjectWithTag("DroneCount").GetComponent<Text>();
        crapCount = GameObject.FindGameObjectWithTag("CrapCount").GetComponent<Text>();
        batteryImage = GameObject.FindGameObjectWithTag("BatteryIcon").GetComponent<Image>();

        names.Clear();
        for (int i = 0; i < droneNames.Count; ++i) {
            names.Add(droneNames[i]);
        }

        for (int i = 0; i < 4; ++i) {
            SpawnTurret(i, 1);
        }

        CreateDrone();

        maxCore = curCore;
        batteryImage.sprite = batteryIcons[batteryIcons.Length - 1];

        StartCoroutine(Chatter());
        log.InputEnabled = true;
    }

    IEnumerator Chatter() {
        while (true) {
            float time = Random.Range(30, 60);
            yield return new WaitForSeconds(time);

            GameOptions.Instance.RandomLine();
        }
    }

    public void TakeDamage(float amount) {
        curCore -= amount;

        if (curCore <= 0) {
            curCore = 0;
            Time.timeScale = 0;

            int secretPct = Mathf.RoundToInt(((float)secretCount / (float)totalSecrets) * 100f);
            log.AddLine("THE CORE HAS BEEN DESTROYED =(");
            log.AddLine("");
            log.AddLine("Total Scrap: " + totalCrap);
            log.AddLine("Spent on turrets: " + turretSpend);
            log.AddLine("Spent on upgrades: " + upgradeSpend);
            log.AddLine("Secret: " + secretPct + "%");
            log.AddLine("");
            log.AddLine("Type 'quit' to return to menu to exit or restart.");
        }

        src.PlayOneShot(coreDamageSound);

        float h = (curCore / maxCore) * 4f;
        int i = Mathf.RoundToInt(h);

        batteryImage.sprite = batteryIcons[i];
    }

    void OnTriggerEnter(Collider other) {
        Mob m = other.GetComponentInParent<Mob>();
        Destroy(m.gameObject);

        TakeDamage(m.damage);
    }

    void Spawn() {
        lastSpawn -= Time.deltaTime;
        if (lastSpawn <= 0) {
            lastSpawn += spawnPeriod;
            int s = Random.Range(0, 4);
            Transform spawn = spawns[s];

            int max = Mathf.RoundToInt(difficulty * mobPrefabs.Count);
            int i = Random.Range(0, max);

            GameObject newMob = (GameObject)Instantiate(mobPrefabs[i], spawn.position, spawn.rotation, mobParent);
            Mob m = newMob.GetComponent<Mob>();
            m.health *= healthMultiplier;
            m.velocity = spawn.position.normalized * -2f;
        }
    }

    // Update is called once per frame
    void Update () {

        Spawn();

        droneCount.text = numDrones.ToString();
        crapCount.text = crap.ToString();

        int i = Mathf.RoundToInt(batteryIcons.Length * curCore / maxCore);
        if (i < 0) i = 0;
        else if (i > batteryIcons.Length - 1) i = batteryIcons.Length - 1;

        batteryImage.sprite = batteryIcons[i];
    }

    public void CollectCrap(int amount) {
        crap += amount;

        if (amount > 0) {
            totalCrap += amount;
        }

        if (difficulty < 1f) {
            difficulty += difficultyRate;

            if (difficulty > 1f) {
                difficulty = 1f;
            }
        } else {
            healthMultiplier *= healthRate;
        }

        if (spawnPeriod > 1f) {
            spawnPeriod -= spawnChange;

            if (spawnPeriod < 1f) {
                spawnPeriod = 1f;
            }
        }
    }

    public void DestroyDrone(Drone d) {
        drones.Remove(d);
        log.AddLine("  " + d.name + " was destroyed!");
        numDrones--;

        if (activeDrone == d) {
            activeDrone = null;
        }
    }

    public override void HandleInput(string command) {
        char[] s = { ' ' };
        string[] tokens = command.Split(s);
        string cmd = tokens[0].ToLower();

        List<string> tokenList = new List<string>();
        for (int i = 0; i < tokens.Length; ++i) {
            tokenList.Add(tokens[i].ToLower());
        }

        switch (cmd) {

        case "open":
            DoOpen(tokenList);
            break;

        case "g":
        case "go":
            DoGo(tokenList);
            break;

        case "d":
        case "drone":
            SwitchDrone(tokenList);
            break;

        case "drones":
            StartCoroutine(ListDrones());
            break;

        case "u":
        case "upgrade":
            DoUpgrade(tokenList);
            break;

        case "b":
        case "build":
            DoBuild(tokenList);
            break;

        case "drink":
            DoDrink(tokenList);
            break;

        case "c":
        case "cam":
        case "camera":
            SwitchCamera(tokenList);
            break;

        case "reset":
            Reset();
            break;

        case "quit":
            Time.timeScale = 1;
            Responses.Quit();
            SceneManager.LoadScene(0);
            break;

        case "h":
        case "home":
            DroneHome();
            break;

        case "n":
        case "next":
            NextCamera();
            break;

        case "help":
            Help(tokenList);
            break;

        default:
            Responses.Unknown();
            break;
        }
    }

    void Help(List<string> tokens) {
        if (tokens.Count == 1) {
            //show commands
            StartCoroutine(ShowCommands());
        } else {
            string cmd = tokens[1];
            for (int i = 0; i < commands.Count; ++i) {
                if (cmd.CompareTo(commands[i].name) == 0) {
                    log.AddLine(commands[i].info);
                    return;
                }
            }

            Responses.Unknown();
        }
    }

    IEnumerator ShowCommands() {
        log.InputEnabled = false;

        log.AddLine("Type help <command> for more info.");
        yield return new WaitForSeconds(logDelay);
        log.AddLine("Most commands can be shortened to 1 letter.");
        yield return new WaitForSeconds(logDelay);
        log.AddLine("");
        yield return new WaitForSeconds(logDelay);

        int i = 0;
        while (i < commands.Count) {
            log.AddLine(commands[i].name);
            ++i;

            yield return new WaitForSeconds(logDelay);
        }

        log.InputEnabled = true;
    }

    void NextCamera() {
        curDir++;

        if (curDir == cameras.Count) {
            curDir = 0;
        }

        Camera.main.enabled = false;
        cameras[curDir].enabled = true;
    }

    void Reset() {
        log.AddLine("  Nuh uh uh! You didn't say the magic word!");
    }

    void DroneHome() {
        if (activeDrone == null) {
            log.AddLine("  No drone selected.");
            return;
        }

        activeDrone.Home();
    }

    void SwitchCamera(List<string> tokens) {
        int d = -1;
        Camera old = Camera.main;

        try {
            d = int.Parse(tokens[1]);

            Camera.main.enabled = false;
            cameras[d].enabled = true;
            curDir = d;
            Responses.CamPos(d);
        } catch {
            Responses.CamNeg();
            old.enabled = true;
            return;
        } 
    }

    void DoUpgrade(List<string> tokens) {
        if (activeDrone.moving) {
            log.AddLine("  The drone hasn't arrived yet.");
            return;
        }

        int station = StationNum(activeDrone.targetStation);

        if (!turrets[activeDrone.currentDir].ContainsKey(station)) {
            Responses.UpgradeNeg(activeDrone.name);
            return;
        }
            
        Turret t = turrets[activeDrone.currentDir][station];

        int cost = (int)t.damage * 2;

        if (crap < cost) {
            Responses.MoreScrap();
            log.AddLine("  You need " + cost + " scrap to upgrade this turret.");
            return;
        }

        CollectCrap(-cost);
        upgradeSpend += cost;

        t.damage++;

        Responses.UpgradePos(activeDrone.name);
        log.AddLine("  Turret upgraded to " + t.damage + " damage.");
    }

    void DoBuild(List<string> tokens) {
        switch (tokens[1]) {
        case "turret":
            BuildTurret();
            break;

        case "drone":
            BuildDrone();
            break;
        }
    }

    void DoDrink(List<string> tokens) {
        if (tokens.Contains("whiskey")) {
            log.AddLine("  What is \"whiskEy\"?");
        } else if (tokens.Contains("whisky")) {
            log.AddLine("  Your nerves are settled some.");
        } else {
            Responses.Drink();
        }
    }

    void DoOpen(List<string> tokens) {
        if (tokens.Contains("pod") && tokens.Contains("bay") && (tokens.Contains("door") || tokens.Contains("doors"))) {
            log.AddLine("COMPUTER: I'm sorry, I can't let you do that, Dave.");
        }
    }

    void DoGo(List<string> tokens) {
        if (activeDrone == null) {
            log.AddLine("COMPUTER: You have no drone active.");
            return;
        }

        try {
            int s = int.Parse(tokens[1]);
            if (s < 1 || s > 10) {
                Responses.GoNeg(activeDrone.name);
                return;
            }

            Responses.GoPos(activeDrone.name);
            activeDrone.Go(curDir, s);
        } catch {
            Responses.GoNeg(activeDrone.name);
        }
    }

    IEnumerator ListDrones() {
        log.InputEnabled = false;
        int i = 0;

        while (i < droneParent.childCount) {
            log.AddLine(i + ": " + droneParent.GetChild(i).name);
            i++;
            yield return new WaitForSeconds(logDelay);
        }
        log.InputEnabled = true;
    }

    void SwitchDrone(List<string> tokens) {
        int d = -1;
        if (tokens.Count < 2) {
            log.AddLine("  Listing drones.");
            StartCoroutine(ListDrones());
            return;
        }

        try {
            d = int.Parse(tokens[1]);

            if (d == 404) {
                log.AddLine("  Error: Drone not found.");
                return;
            }

            activeDrone = droneParent.GetChild(d).GetComponent<Drone>();
        } catch {
            Responses.DroneNeg();
            return;
        }

        Responses.DronePos(activeDrone.name);
    }

    public static float StationDist(int num) {
        return num * 2 + 9;
    }

    public static int StationNum(float dist) {
        return (int)((dist - 9) / 2);
    }

    void BuildDrone() {
        if (crap < droneCost) {
            log.AddLine("  Not enough scrap to build a drone!");
            return;
        }

        CollectCrap(-droneCost);

        CreateDrone();
    }

    void CreateDrone() {
        GameObject go = (GameObject)Instantiate(dronePrefab, Vector3.zero, Quaternion.identity, droneParent);
        Drone d = go.GetComponent<Drone>();

        int i = Random.Range(0, names.Count);
        d.name = names[i];
        names.RemoveAt(i);
        drones.Add(d);

        log.AddLine("COMPUTER: Drone " + d.name + " built.");
        numDrones++;
    }

    void BuildTurret() {
        if (activeDrone == null) {
            log.AddLine("COMPUTER: No active drone!");
            return;
        }

        if (activeDrone.moving) {
            log.AddLine(activeDrone.name + ": I'm not there yet!");
            return;
        }

        if (crap < turretCost) {
            Responses.MoreScrap();
            return;
        }

        int station = StationNum(activeDrone.targetStation);

        if (turrets[curDir].ContainsKey((int)station)) {
            Responses.ExistingTurret(activeDrone.name);
            return;
        }

        CollectCrap(-turretCost);
        turretSpend += turretCost;

        SpawnTurret(activeDrone.currentDir, station);
        Responses.Build(activeDrone.name);
    }

    void SpawnTurret(int dir, int station) {
        float offset = 0.6f;
        if (station % 2 == 0) {
            offset *= -1;
        }

        float dist = StationDist(station);

        float x = 0, z = 0, a = 0;
        switch (dir) {
        case 0: //north
            x = offset;
            z = dist;
            a = 0;
            break;

        case 1: //east
            z = offset;
            x = dist;
            a = 90;
            break;

        case 2: //south
            x = offset;
            z = -dist;
            a = 180;
            break;

        case 3: //west
            z = offset;
            x = -dist;
            a = -90;
            break;
        }

        Vector3 spawnPos = new Vector3(x, 0, z);
        Quaternion spawnRot = Quaternion.Euler(0, a, 0);

        GameObject go = (GameObject)Instantiate(turretPrefab, spawnPos, spawnRot);
        Turret t = go.GetComponent<Turret>();

        turrets[dir].Add(station, t);
    }
}
