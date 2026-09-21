using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    private SceneManager sceneManager;
    public GameObject MainMenu;

    void Awake()
    {
        // Singleton setup
        sceneManager = FindAnyObjectByType<SceneManager>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void NewGame()
    {
        if (sceneManager != null) sceneManager.NewGame();
    }
    public void LoadGame()
    {
        if (sceneManager != null) sceneManager.LoadGame();
    }
    public void StartLogin()
    {
        Debug.Log("StartLogin called."); // Ellenőrző log
        try
        {
            // Biztonságosabb ellenőrzés: ha az Instance null, megpróbáljuk megkeresni
            var om = OnlineManager.Instance ?? FindAnyObjectByType<OnlineManager>();

            if (om == null)
            {
                Debug.LogWarning("StartLogin: OnlineManager not found in scene. Cannot log in.");
                return;
            }

            if (om.IsLoggedIn)
            {
                Debug.Log("StartLogin: User already logged in.");
                return;
            }
            om.CheckConnectivity(() =>
                {
                    if (PanelManager.Instance != null && PanelManager.Instance.loginWindow != null)
                    {
                        PanelManager.Instance.loginWindow.SetActive(true);
                    }
                });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"StartLogin Error: {ex.Message}");
        }
    }

    public void SaveGame()
    {
        if (OnlineManager.Instance != null)
        {
            if (OnlineManager.Instance.IsLoggedIn)
            {
                OnlineManager.Instance.OnlineSave();
                Debug.Log("SaveGame: Mentés kezdeményezve.");
            }
            else
            {
                Debug.Log("SaveGame: A játékos nincs belépve, a mentés nem lehetséges.");
            }
        }
    }

    // Átnevezve ExitGame-re, hogy megszakítsuk a beragadt hivatkozásokat
    public void ExitGame()
    {
        Debug.Log("ExitGame function called.");
        
        bool fromWebsite = SceneManager.Instance != null && SceneManager.Instance.isFromWebsite;
        if (fromWebsite)
        {
            // 3. Honlapról jött -> Vissza oda, ahonnan indult
            if (DBManager.Instance != null && DBManager.Instance.isTutorialOK == "ok")
            {
                Application.OpenURL("kikoto.html"); // Kalózkikötő
            }
            else
            {
                Application.OpenURL("hebok.html"); // Tutorial nem OK -> Start oldal
            }
        }
        else
        {
            // 2. Külső linkről jött, és belépett -> Honlap
            if (OnlineManager.Instance != null && OnlineManager.Instance.IsLoggedIn)
            {
                Application.OpenURL("hebok.html");
            }
            // 1. Külső linkről jött, ÉS nem lépett be -> Bezárás
            else
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }
    }
    public void CloseMenu()
    {
        if (MainMenu != null) MainMenu.SetActive(false);
        else Debug.LogWarning("MenuManager: MainMenu reference is missing!");
    }

}
