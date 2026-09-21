using UnityEngine;
using TMPro;

public class TopBarManager : MonoBehaviour
{
    
    public static TopBarManager Instance { get; private set; }

    [Header("Player")]
    [Tooltip("Name used for save file (PlayerName.txt). Leave empty to use 'Player'.")]
    public string PlayerName = "FailedToLoad";

    // Number of carried items collected
    public int Carried { get; private set; } = 0;

    // Number of items permanently stored at Home
    public int Stored { get; private set; } = 0;

    // Number of pickup actions performed
    public int PickupCount { get; private set; } = 0;

    // Number of delivery actions performed
    public int DeliveryCount { get; private set; } = 0;

    [Header("UI")]
    [Tooltip("Optional TextMeshProUGUI that will display the current Carried value.")]
    public TextMeshProUGUI carriedText;

    [Tooltip("Optional TextMeshProUGUI that will display the current Stored value.")]
    public TextMeshProUGUI storedText;

    [Tooltip("Optional TextMeshProUGUI that will display the player's name.")]
    public TextMeshProUGUI playerNameText; // shows PlayerName in UI (optional) 
    public GameObject FSB;
    public GameObject SSB;
    private DBManager dbManager;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        
        dbManager = FindAnyObjectByType<DBManager>();

        Debug.Log("TopBarManager Awake: Loading data from DBManager.");
        // initialize UI (LoadFromDBTemp may have already updated some UI)
        updateFromDBManager();
    }
    public void updateFromDBManager()
    {
        if (dbManager != null)
        {
            PlayerName = dbManager.PlayerName;
            Stored = dbManager.Stored;
            if (PickupCount < Stored) PickupCount = Stored;
            if (DeliveryCount < Stored) DeliveryCount = Stored;
            UpdatePlayerNameUI();
            UpdateStoredUI();
        }
    }

    /// <summary>
    /// Increment the Carried score by the specified amount (default 1).
    /// </summary>
    public void AddCarried()
    {
        Carried ++;
        PickupCount++;
        Debug.Log($"Carried: {Carried}, PickupCount: {PickupCount}");
        UpdateCarriedUI();

        // Az első hártyafelvétel actionnál rögtön jön a Monologues 00_1.1
        if (PickupCount == 1 && MonologManager.Instance != null)
        {
            MonologManager.Instance.ShowFirstPickup();
        }
        // A következő (második) hártyafelvételi action esetén jön a 04_1.5 és 05_1.6
        else if (PickupCount == 2 && MonologManager.Instance != null)
        {
            MonologManager.Instance.ShowSecondPickup();
        }
        // Az 5. PickupCount után (azonnal) indul a ballon up (Zeppelin chasing + kamraváltás)
        else if (PickupCount == 5 && SceneManager.Instance != null)
        {
            if (SceneManager.Instance.isFromWebsite && dbManager != null && dbManager.isTutorialOK != "ok")
            {
                SceneManager.Instance.InitializeAscensionScene();
            }
        }
    }

    /// <summary>
    /// Move all carried items into stored and reset Carried to 0.
    /// </summary>
    public void StoreCarried()
    {
        if (Carried <= 0) return;
        
        bool isFirstDelivery = (Stored == 0);

        Stored += Carried;
        DeliveryCount++;
        Carried = 0;
        dbManager.Stored = Stored;
        Debug.Log($"Stored: {Stored}");
        UpdateCarriedUI();
        UpdateStoredUI();

        // Persist the updated Stored value to DBtemp.txt
        dbManager.SaveToDBFile();

        // Az első olyan eseménynél, amikor a játékos az első CarriedLoot-ot a Home-nál átváltja DeliveredLootra
        if (isFirstDelivery && MonologManager.Instance != null)
        {
            MonologManager.Instance.ShowFirstDelivery();
        }
        // A következő (második) home delivered után 06_1.7 és a 07_1.8
        else if (DeliveryCount == 2 && MonologManager.Instance != null)
        {
            MonologManager.Instance.ShowSecondDelivery();
        }
        // a 3. DeliveryCount után jön a 08, 09, 10, 11 és 12.
        else if (DeliveryCount == 3 && MonologManager.Instance != null)
        {
            MonologManager.Instance.ShowThirdDelivery();
        }
        // A ballon-up utáni első DeliveryCount után közvetlenül jön a monologue 15, 16, 17. 18 és 19.
        else if (dbManager.hasAscended && dbManager.lastMonolog == 14 && MonologManager.Instance != null)
        {
            MonologManager.Instance.ShowPostAscensionDelivery();
        }
        // A 19. monológ utáni következő deliverynél jön a 20-30.
        else if (dbManager.lastMonolog == 19 && MonologManager.Instance != null)
        {
            MonologManager.Instance.ShowFourthDelivery();
        }
    }

    /// <summary>
    /// Set the UI text if a TextMeshProUGUI has been assigned.
    /// </summary>
    private void UpdateCarriedUI()
    {
        if (carriedText != null)
        {
            carriedText.text = Carried.ToString();
        }
    }
    internal void ResetCarried()
    {
        Carried = 0;
        UpdateCarriedUI();
    }

    private void UpdateStoredUI()
    {
        if (storedText != null)
        {
            storedText.text = Stored.ToString();
        }
    }

    /// <summary>
    /// Update the player name UI element if assigned.
    /// </summary>
   private void UpdatePlayerNameUI()
    {
        if (playerNameText != null)
        {
            // Ha a név üres, vagy a "FailedToLoad" hibaüzenet maradt benne
            if (string.IsNullOrEmpty(PlayerName) || PlayerName == "FailedToLoad")
            {
                PlayerName = "Anonymus"; 
            }
            playerNameText.text = PlayerName;
        }
    }
        public void MenuPressed()
    {
        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.OpenMainMenu(); 
            Debug.Log("Steampunk Menü megnyitása kezdeményezve.");
        }
        else
        {
            Debug.LogWarning("MenuPressed: PanelManager Instance is null!");
        }
    }

        public void InfoPressed()
    {
        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.InfoPressed();
        }
        else
        {
            Debug.Log("InfoPressed, but PanelManager is null.");
        }
    }
    public void ExitPressed()
    {
        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.ExitPressed();
            Debug.Log("ExitPressed.");
        }
        else
        {
            Debug.Log("ExitPressed, but PanelManager is null.");
        }
    }
    public void FullScreenPressed()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Debug.Log("FullScreenPressed.");
        FSB.SetActive(false);
        SSB.SetActive(true);
    }
    public void SmallScreenPressed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Debug.Log("SmallScreenPressed.");
        SSB.SetActive(false);
        FSB.SetActive(true);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

}
