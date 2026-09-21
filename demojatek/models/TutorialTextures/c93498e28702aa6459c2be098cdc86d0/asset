using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    public bool isFromWebsite = false;
    public MenuCamera menuCam; 
    public CameraMove playerCam; 
    public GameObject openingCam;
    public GameObject ascensionCam;
    public GameObject theChest;
    public GameObject endTrigger;
    public GameObject chestTrigger;
    public int SpawnedRemaining;

    private DBManager dbManager;
    private Chase chase;
    private Spawner spawner;
    private WindManger windManger;
    private TopBarManager topBarManager;
    private MonologManager monologManager;

    void Start()
{
    // Alapvető manager keresések maradnak...
    if (Instance != null && Instance != this) { Destroy(gameObject); return; }
    Instance = this;
    
    dbManager = FindAnyObjectByType<DBManager>();
    chase = FindAnyObjectByType<Chase>();
    spawner = FindAnyObjectByType<Spawner>();
    windManger = FindAnyObjectByType<WindManger>();
    topBarManager = FindAnyObjectByType<TopBarManager>();
    monologManager = FindAnyObjectByType<MonologManager>();

    // 1. DÖNTÉSI LOGIKA: Honlapról vagy külső linkről jön-e?
    if (isFromWebsite) 
    {
        dbManager.ReadFromDBFile(); // Adatok betöltése az állapothoz

        // a. Nincs tutorial -> Móló + Zozó opening
        if (dbManager.isTutorialOK != "ok")
        {
            InitializeOpening(); 
        }
        // b. és c. Volt tutorial -> Móló, de nincs opening/monológ
        else
        {
            InitializePlayer(); 
        }
    }
    // 2. Alapindítás (nem honlapról) -> Itt jön be a kameraforgatós MainMenu
    else
    {
        SetupMainMenu(); 
    }
}

    void Update()
    {
        // 1. Az esc billentyű megnyomásával el kell indítani az Exit rutint.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PanelManager.Instance != null) PanelManager.Instance.ExitPressed();
        }

        // 2. Az egér jobb klikkjével, és a space-e pedig el kell tudni érni a TopBart.
        if (Input.GetMouseButtonDown(1) || (monologManager != null && !monologManager.IsMonologOn && Input.GetKeyDown(KeyCode.Space)))
        {
            ToggleCursor();
        }
    }

    public void ToggleCursor()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    /// <summary>
    /// </summary>
    public void InitializeScene()
    {
        if (dbManager.Stored == 0)
        {
            SetupLevelZero();
        }
        else
        {
            ResetBalloonPositions();
            ReconfigureAndRespawnSpawners();
            // 7. Javítás: Ha már nem a 0. szinten vagyunk, a szelet vissza kell kapcsolni!
            if (windManger != null)
            {
                windManger.enabled = true;
                windManger.SetRandomWind();
            }
        }
        if(dbManager.Stored >= 30 && dbManager.isTutorialOK != "ok")
        {      
            EndGame();
        }
    }

    /// <summary>
    /// Called when the player reaches Home. Advances the level and performs the reset/respawn tasks.
    /// </summary>
    public bool OnPlayerReachedHome()
    {
        if(dbManager.hasAscended)
            dbManager.firstChase = true;
        if(topBarManager.PickupCount > 6)
            dbManager.secoundChase = true;
        InitializeScene();
        return true;
    }

    private void EndGame()
    {
        Debug.Log("EndGame enabled");
        spawner.ClearSpawned();
        if (theChest != null) theChest.SetActive(true);
        if (endTrigger != null) endTrigger.SetActive(true);
        if (chestTrigger != null) chestTrigger.SetActive(true);
    }

    private void ResetBalloonPositions()
    {
        if (chase != null && topBarManager.Stored+topBarManager.Carried >= 4)
        {
            chase.ChaseOn = true;
        }
        chase.ResetToStart();
    }
    private void ResetPlayerPosition()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
        if (player != null)
        {
            player.ResetToStart();
        }
    }

    private void SetupLevelZero()
    {
        // No wind
        if (windManger != null)
        {
            windManger.enabled = false;
            Debug.Log("SceneManager: Stored 0 setup - wind disabled.");
        }
        if (chase != null)
        {
            chase.ChaseOn = false;
            Debug.Log("SceneManager: Stored 0 setup - chase disabled.");
        }

        // Only one item spawns at 22, -530
        if (spawner != null)
        {
            spawner.ClearSpawned();
            spawner.SpawnAt(new Vector3(95f, -2f, -575f));
            SpawnedRemaining = 1;
        }
        
        topBarManager.updateFromDBManager();
    }
    private void InitializeOpening()
    {
        OpeningManager openingManager = FindAnyObjectByType<OpeningManager>();
        if (openingManager != null)
        {
            openingManager.enabled = true;
        }
        
        if (menuCam != null) menuCam.gameObject.SetActive(false);
        if (playerCam != null) playerCam.gameObject.SetActive(false);
        if (openingCam != null) openingCam.SetActive(true);  
        openingManager.gameObject.SetActive(true);    
        SetPlayerControl(false);
    }
    public void InitializePlayer()
    {
        if (playerCam != null) playerCam.gameObject.SetActive(true);
        if (menuCam != null) menuCam.gameObject.SetActive(false);
        if (openingCam != null) openingCam.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetPlayerControl(true);
    }
    public void InitializeAscensionScene()
    {
        chase.ChaseOn = true;
        if (playerCam != null) playerCam.gameObject.SetActive(false);
        if (ascensionCam != null) ascensionCam.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        windManger.enabled = false;
        StartCoroutine(firstAscension());
    }

    private System.Collections.IEnumerator firstAscension()
    {
        yield return new WaitForSeconds(8f);
        ascensionCam.SetActive(false);
        playerCam.gameObject.SetActive(true);
        Debug.Log("SceneManager: Ascension scene ended - returning to player camera.");
        monologManager.BaloonUp();
        dbManager.hasAscended = true;
        windManger.enabled = true;
        windManger.SetRandomWind();
    }

    private void ReconfigureAndRespawnSpawners()
    {
        float earlyCentral = Mathf.Max(50f, dbManager.Stored * 10f);
        float newCentral = Mathf.Min(earlyCentral, 500f);
        float newStd = Mathf.Min(dbManager.Stored * 10f + 50f, 550f);
        int newCount = dbManager.Stored + 3;

        spawner.ClearSpawned();
        spawner.centralNoSpawnRadius = newCentral;
        spawner.gaussianStdX = newStd;
        spawner.gaussianStdZ = newStd;
        spawner.RespawnCount(newCount);
        SpawnedRemaining = newCount;
    }
    public void SetupMainMenu()
    {
        if (menuCam != null) menuCam.gameObject.SetActive(true);
        if (playerCam != null) playerCam.gameObject.SetActive(false);
        if (openingCam != null) openingCam.SetActive(false);

        if (MenuManager.Instance.MainMenu != null) MenuManager.Instance.MainMenu.SetActive(true);

        int realStored = dbManager.Stored;
        dbManager.Stored = 0;
        topBarManager.updateFromDBManager();
        InitializeScene();
        dbManager.Stored = realStored;
        // 6. Javítás: A TopBar-t is frissíteni kell a visszaállított értékkel, különben 0-nak hiszi a Stored-ot!
        topBarManager.updateFromDBManager();
        SetPlayerControl(false);
    }

    public void NewGame()
    {
        dbManager.PlayerName = "Anonimus";
        dbManager.Stored = 0;
        dbManager.isTutorialOK = "";
        dbManager.hasAscended = false;
        dbManager.hasMoved = false;
        dbManager.firstReset = false;
        dbManager.firstWind = false;
        dbManager.firstChase = false;
        dbManager.secoundChase = false;
        dbManager.firstCatch = false;
        dbManager.lastMonolog = 0;
        dbManager.SaveToDBFile();

        if (theChest != null) theChest.SetActive(false);
        if (endTrigger != null) endTrigger.SetActive(false);
        if (chestTrigger != null) chestTrigger.SetActive(false);

        topBarManager.updateFromDBManager();
        SetupLevelZero();
        ResetPlayer();
        if (isFromWebsite)
        {
            InitializeOpening();
        }
        else
        {
            InitializePlayer();
            
            // 1.b kiegészítés: Külső link esetén nincs monológ, de van szél, chase és marketing quiz
            dbManager.lastMonolog = int.MaxValue; // Monológok tiltása

            if (windManger != null)
            {
                windManger.enabled = true;
                windManger.SetRandomWind();
            }

            if (chase != null)
            {
                chase.ChaseOn = true;
            }
        }
    }
    public void LoadGame()
    {
        if (dbManager.ReadFromDBFile())
        {
            topBarManager.updateFromDBManager();
            ResetPlayerPosition();
            InitializeScene();
        }
        else
        {
            Debug.LogWarning("LoadGame failed: could not load from DB file.");
        }
        if (dbManager.Stored > 1)
            monologManager.PlayAllMonologs();
        if (menuCam != null) menuCam.gameObject.SetActive(false);
        if (playerCam != null) playerCam.gameObject.SetActive(true);
        if (openingCam != null) openingCam.SetActive(false);
        SetPlayerControl(true);
    }

    public void StartLoggedInGame()
    {
        // Belépés utáni indítás
        Time.timeScale = 1f; // Biztosítjuk, hogy a játékidő elinduljon (pl. ha a kilépési panelről jöttünk)
        // Frissítjük a UI-t a DBManager adataiból (amit a Login töltött be)
        topBarManager.updateFromDBManager();
        
        // Pálya és Játékos alaphelyzetbe állítása az adatok alapján
        ResetPlayer(); // Ez hívja a ResetPlayerPosition-t és az InitializeScene-t

        // Döntési logika: Tutorial vagy Játék
        if (dbManager.isTutorialOK != "ok")
        {
            InitializeOpening();
        }
        else
        {
            InitializePlayer();
        }
    }

    public void ResetPlayer()
    {
        ResetPlayerPosition();
        InitializeScene();
    }
    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void SetPlayerControl(bool state)
    {
        PlayerController player = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
        if (player != null)
        {
            player.canMove = state;
        }
    }
}
