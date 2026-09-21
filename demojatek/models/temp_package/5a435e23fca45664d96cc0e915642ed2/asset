using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    public GameObject InfoScroll;
    public GameObject AreYouSure;
    public GameObject BoatNaming;
    public TMP_InputField boatNameInput;
    
    [Header("Új Steampunk Panelek")]
    public GameObject MainMenuPanel;    // Az új rézcsöves menü 
    public GameObject QuitWarningPanel; // A "Vesszenek a kártyák?" ablak 
    public GameObject LoginButtonOnQuitPanel; // A "Belépek" gomb a kilépési panelen
    public TextMeshProUGUI QuitWarningText;
    [Header("Login")]
    public GameObject loginWindow;      // A belépő ablak
    public TMP_InputField uname;
    public TMP_InputField passwd;
    public GameObject loginFailedWindow;
    public GameObject wellcome;
    public GameObject GameisOffline;
    public GameObject NoODBpanel;

    [Header("End of Game")]
    public GameObject EoGTab;
    public GameObject EoGWindow1;
    public GameObject EoGWindow2;
    public GameObject EoGWindow3;
    
    private int eogState = 0;
    private CameraMove cameraMove;
    private OnlineManager onlineManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        cameraMove = FindAnyObjectByType<CameraMove>();
        onlineManager = FindAnyObjectByType<OnlineManager>();

    }

    void Update()
    {
        if (loginWindow != null && loginWindow.activeInHierarchy)
        {
            if (cameraMove != null) cameraMove.lockState(true); // Javítva: true = UI mód (kamera fagyasztva)
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if (eogState > 0 && eogState != 4 && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            NextEoGWindow();
        }
    }

    public void ActivateEoG()
    {
        if (onlineManager != null && !onlineManager.IsLoggedIn)
        {
            if (loginWindow != null) loginWindow.SetActive(true);
        }

        if (EoGTab != null) EoGTab.SetActive(true);
        if (EoGWindow1 != null) EoGWindow1.SetActive(true);
        if (EoGWindow2 != null) EoGWindow2.SetActive(false);
        if (EoGWindow3 != null) EoGWindow3.SetActive(false);

        eogState = 1;
        Time.timeScale = 0f;
        if (cameraMove != null)
            cameraMove.lockState(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void NextEoGWindow()
    {
        if (eogState == 1)
        {
            if (EoGWindow1 != null) EoGWindow1.SetActive(false);
            if (EoGWindow2 != null) EoGWindow2.SetActive(true);
            eogState = 2;
        }
        else if (eogState == 2)
        {
            if (EoGWindow2 != null) EoGWindow2.SetActive(false);
            if (DBManager.Instance.isTutorialOK != "ok")
            {
                if (BoatNaming != null) BoatNaming.SetActive(true);
                eogState = 4;
            }
            else
            {
                if (EoGWindow3 != null) EoGWindow3.SetActive(true);
                eogState = 3;
            }
        }
        else if (eogState == 3)
        {
            if (EoGWindow3 != null) EoGWindow3.SetActive(false);
            if (EoGTab != null) EoGTab.SetActive(false);
            eogState = 0;
            Time.timeScale = 1f;
            if (cameraMove != null) cameraMove.lockState(false);
            SceneManager.Instance.SetupMainMenu();
        }
    }

    public void CancelInfo()
    {
        if (InfoScroll != null)
        {
            if (cameraMove != null) cameraMove.lockState(false);
            InfoScroll.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("InfoScroll closed.");
        }
    }
    public void CancelExit()
    {
        // Az 5S szellemében minden panelt alaphelyzetbe állítunk
        if (AreYouSure != null) AreYouSure.SetActive(false);
        if (QuitWarningPanel != null) QuitWarningPanel.SetActive(false);

        // Visszaadjuk az irányítást a játékosnak
        if (cameraMove != null) cameraMove.lockState(false);
        Time.timeScale = 1f;
        Debug.Log("Kilépés megszakítva, minden panel bezárva.");
    }
  
  public void YesExit()
    {
        // Egyszerűsített ellenőrzés: ha a DBManager példány létezik
        bool hasDB = DBManager.Instance != null;
        
        // Itt egyelőre feltételezzük a tutorialt, ha a DBManager szerint nincs kész
        bool isTutorial = hasDB && DBManager.Instance.isTutorialOK != "ok";

        // Panelek váltása
        if (AreYouSure != null) AreYouSure.SetActive(false);
        if (QuitWarningPanel != null) QuitWarningPanel.SetActive(true);

        // Szöveg beállítása
        if (QuitWarningText != null)
        {
            // Ha a loginWindow aktív, akkor biztosan nincs belépve
            if (loginWindow != null && !loginWindow.activeSelf)
            {
                if (isTutorial)
                {
                    QuitWarningText.text = "Ha megszakítod a tutorialt, az eddig megszerzett hártyáid el fognak veszni. Biztosan kilépsz?";
                }
                else
                {
                    QuitWarningText.text = "Ha kilépsz, a hajón levő hártyáid el fognak veszni. Biztosan kilépsz?";
                }
            }
            else
            {
                QuitWarningText.text = "Nem vagy belépve. Ha most kilépsz, az eddig megszerzett hártyáid el fognak veszni.";
            }
        }
        
        Debug.Log("YesExit: Ellenőrzés lefutott a loginWindow alapján.");
    }

    public void InfoPressed()
    {
        if (InfoScroll == null) return;
        if (cameraMove != null) cameraMove.lockState(true);
        InfoScroll.SetActive(true);
        Time.timeScale = 0f;
    }

        public void LoginOK()
    {
        if (onlineManager != null && uname != null && passwd != null)
        {
            string u = uname.text.Trim();
            string p = passwd.text.Trim();
            
            if (!string.IsNullOrEmpty(u) && !string.IsNullOrEmpty(p))
                onlineManager.LocalLogin(u, p);
        }
        else if (onlineManager == null)
        {
            onlineManager = FindAnyObjectByType<OnlineManager>(); // Ha null lenne, megpróbáljuk megkeresni
        }
        Debug.Log("LoginOK pressed.");
    }

    // Ezt a függvényt kell meghívni az OnlineManager-ből, ha a belépés sikeres volt!
    public void OnLoginSuccess()
    {
        Debug.Log("Login successful! Starting game...");
        
        // Close the login window immediately
        if (loginWindow != null) loginWindow.SetActive(false);
        
        // Ha a kilépési panelről léptünk be, azt most bezárjuk
        if (QuitWarningPanel != null) QuitWarningPanel.SetActive(false);

        if (eogState == 0)
        {
            MenuManager.Instance.CloseMenu();
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.StartLoggedInGame();
            }
        }
    }

    public void TogglePasswordVisibility(TMP_InputField inputField)
    {
        if (inputField == null) return;

        inputField.contentType = (inputField.contentType == TMP_InputField.ContentType.Password)
            ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;
        inputField.ForceLabelUpdate();
    }

    // Ezt kösd be a Login ablak "szem" ikonjára (SeePass)
    public void ToggleLoginPasswordVisibility()
    {
        TogglePasswordVisibility(passwd);
    }

    public void CancelLogin()
    {
        if (loginWindow != null) loginWindow.SetActive(false);
        // Ha a játék meg volt állítva (pl. kilépés panelről jöttünk), és mégsem lépünk be, indítsuk újra
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            if (cameraMove != null) cameraMove.lockState(false);
            // Visszaállítjuk a kurzort játék módba
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void CancelLoginFailed()
    {
        if (loginFailedWindow != null) loginFailedWindow.SetActive(false);
    }
    public void ShowWelcome(string nev)
    {
        if (wellcome != null)
        {
            wellcome.SetActive(true);
            TextMeshProUGUI[] texts = wellcome.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var txt in texts)
            {
                if (txt.text.Contains("<Névtelen>"))
                {
                    txt.text = txt.text.Replace("<Névtelen>", nev);
                }
            }
            
            // Megállítjuk a játékot és előhozzuk az egeret az üdvözlő ablakhoz
            if (cameraMove != null) cameraMove.lockState(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void CancelWelcome()
    {
        if (wellcome != null) wellcome.SetActive(false);
        
        // Kényszerített újraindítás: biztosítjuk, hogy a játék és a vezérlés elinduljon
        Time.timeScale = 1f;
        if (cameraMove != null) cameraMove.lockState(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (loginWindow != null) loginWindow.SetActive(false);
    }
    public void RegisterPressed()
    {
        Application.OpenURL("reg.html");
    }
    public void SendEndOfGameData()
    {
        onlineManager.EndTutorial();
    }
    public void BoatNameOK()
    {
        string name = boatNameInput != null ? boatNameInput.text : "";
        // Engedélyezzük a magyar ékezetes karaktereket is (Regex ellenőrzés)
        if (System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9 áéíóöőúüűÁÉÍÓÖŐÚÜŰ]{3,16}$"))
        {
            onlineManager.csonaknev = name;
            if (BoatNaming != null) BoatNaming.SetActive(false);
            DBManager.Instance.isTutorialOK = "ok";
            SendEndOfGameData();
            if (EoGWindow3 != null) EoGWindow3.SetActive(true);
            eogState = 3;
        }
        else
        {
            Debug.Log("Invalid boat name. Must be 3-16 characters, alphanumeric and spaces only.");
        }
    }
    public void CancelOfflinePanel()
    {
        if (GameisOffline != null) GameisOffline.SetActive(false);
    }
    public void CancelNoODBPanel()
    {
        if (NoODBpanel != null) NoODBpanel.SetActive(false);
    }
    public void OpenMainMenu()
{
    // Itt hívjuk meg újra a kameraforgatós részt:
    SceneManager.Instance.SetupMainMenu();
    
    // És itt jöhet a Login ablak is, ha még nincs belépve:
    if (onlineManager != null)
    {
        onlineManager.CheckConnectivity(() => {
             if (!onlineManager.IsLoggedIn && loginWindow != null) 
                 loginWindow.SetActive(true);
        });
    }
}

    // A Halálfej és a Főmenü Vissza gombja is ezt hívja
    public void ExitPressed()
    {
        // 1. Megállítjuk a játékot
        if (cameraMove != null) cameraMove.lockState(true);
        Time.timeScale = 0f;

        // 2. Megnézzük a helyzetet (Tutorial és Belépés ellenőrzés)
        // Olyan változókat használunk, amik biztosan nem dobnak hibát
        bool isLoggedIn = onlineManager != null && onlineManager.IsLoggedIn;
        bool isTutorial = DBManager.Instance != null && DBManager.Instance.isTutorialOK != "ok";

        // 3. A szöveg beállítása a QuitWarningPanelen a te kérésed szerint
        if (QuitWarningText != null)
        {
            if (!isLoggedIn)
            {
                QuitWarningText.text = "Nem vagy belépve. Ha most kilépsz, az eddig megszerzett hártyáid el fognak veszni.";
            }
            else if (isTutorial)
            {
                QuitWarningText.text = "Ha megszakítod a tutorialt, az eddig megszerzett hártyáid el fognak veszni. Biztosan kilépsz?";
            }
            else
            {
                QuitWarningText.text = "Ha kilépsz, a hajón levő hártyáid el fognak veszni. Biztosan kilépsz?";
            }
        }

        // 4. Azonnal a QuitWarningPanel-t nyitjuk meg (az AreYouSure már nem kell!)
        if (QuitWarningPanel != null) QuitWarningPanel.SetActive(true);

        // 5. A Belépés gomb megjelenítése, ha nincs belépve
        if (LoginButtonOnQuitPanel != null)
        {
            LoginButtonOnQuitPanel.SetActive(!isLoggedIn);
        }
        
        Debug.Log("ExitPressed: A helyes figyelmeztető ablak megnyitva.");
    }

    // Ezt a függvényt kösd rá az új "Belépek" gombra a Unity Editorban!
    public void LoginFromQuitPanel()
    {
        if (QuitWarningPanel != null) QuitWarningPanel.SetActive(false);
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.StartLogin();
        }
        else
        {
            Debug.LogError("LoginFromQuitPanel: MenuManager Instance is null!");
        }
    }

    public void ConfirmQuit()
    {
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ExitGame();
        }
        else
        {
            Debug.LogError("ConfirmQuit: MenuManager Instance is null!");
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
