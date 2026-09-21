using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

// --- Data Transfer Objects (DTOs) for requests ---

[Serializable]
public class VerifyPlayerRequest
{
    public string action = "verifyPlayerForBetting";
    public LoginData loginData;
}

[Serializable]
public class LoginData
{
    public string kaloznev;
    public string etel;
}

[Serializable]
public class SaveScoreRequest
{
    public string action = "saveHartyaScore";
    public string email;
    public int score;
    public string triggersString;
}

[Serializable]
public class TutorialEndRequest
{
    public string action = "tutorialEnded";
    public string email;
    public bool tutorialEnded;
    public string csonaknev;
    public int whiteHartya;
    public string triggersString;
}

// --- Data Transfer Objects (DTOs) for responses ---

[Serializable]
public class VerifyPlayerResponse
{
    public bool success;
    public string userEmail;
    public int credits;
    public string triggers;
    public string reason;
}

[Serializable]
public class SaveScoreResponse
{
    public bool success;
    public string reason;
}

[Serializable]
public class TutorialEndResponse
{
    public bool success;
    public string reason;
}

public class OnlineManager : MonoBehaviour
{
    public static OnlineManager Instance { get; private set; }

    // Variables
    public string URL;
    public string Kaloznev;
    public string FavFood;
    public string email;
    public string triggersString;
    public string csonaknev;
    public bool IsLoggedIn => _isLoggedIn;
    private bool _isLoggedIn;

    [Tooltip("The deployment URL of your Google Apps Script Web App.")]
    [SerializeField] private string webAppUrl = "https://script.google.com/macros/s/AKfycbx-Z8VuckLYOKgyxyJZAkfVcv6YL9Aed9hlNZ_NQsPjEz2SVGfk_VpDOP50iKE8CmsQ/exec";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        if (string.IsNullOrEmpty(webAppUrl))
        {
            webAppUrl = "https://script.google.com/macros/s/AKfycbx-Z8VuckLYOKgyxyJZAkfVcv6YL9Aed9hlNZ_NQsPjEz2SVGfk_VpDOP50iKE8CmsQ/exec";
        }
        URL = webAppUrl.Trim();
    }

    public void CheckConnectivity(Action onConnectionSuccess = null)
    {
        StartCoroutine(CheckConnectivityRoutine(onConnectionSuccess));
    }

    private IEnumerator CheckConnectivityRoutine(Action onConnectionSuccess)
    {
        if (string.IsNullOrEmpty(URL))
        {
            Debug.LogError("[OnlineManager] URL is empty or malformed.");
            if (PanelManager.Instance != null && PanelManager.Instance.GameisOffline != null)
            {
                PanelManager.Instance.GameisOffline.SetActive(true);
            }
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(URL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[OnlineManager] Connectivity check failed: {www.error}");
                if (PanelManager.Instance != null)
                {
                    if (www.result == UnityWebRequest.Result.ProtocolError && PanelManager.Instance.NoODBpanel != null)
                    {
                        PanelManager.Instance.NoODBpanel.SetActive(true);
                    }
                    else if (PanelManager.Instance.GameisOffline != null)
                    {
                        PanelManager.Instance.GameisOffline.SetActive(true);
                    }
                }
            }
            else
            {
                if (onConnectionSuccess != null)
                {
                    onConnectionSuccess.Invoke();
                }
                else
                {
                    AutoLogin();
                }
            }
        }
    }

    // Functions

    public void AutoLogin()
    {
        // Attempt to login if we have credentials stored in memory
        if (!string.IsNullOrEmpty(Kaloznev) && !string.IsNullOrEmpty(FavFood))
        {
            Debug.Log($"[OnlineManager] AutoLogin attempting for '{Kaloznev}'...");
            VerifyPlayer(Kaloznev, FavFood);
        }
    }

    public void LocalLogin(string uname, string passwd)
    {
        VerifyPlayer(uname, passwd);
    }

    public void VerifyPlayer(string u, string p)
    {
        Kaloznev = u;
        FavFood = p;
        StartCoroutine(VerifyPlayerRoutine(new LoginData { kaloznev = u, etel = p }));
    }

    public void TriggersfromString(string triggersStr)
    {
        if (string.IsNullOrEmpty(triggersStr) || DBManager.Instance == null) return;
        // Assuming CSV format: hasAscended,hasMoved,firstReset,firstWind,firstChase,secoundChase,firstCatch,lastMonolog,isTutorialOK
        string[] parts = triggersStr.Split(',');
        if (parts.Length >= 8)
        {
            bool.TryParse(parts[0], out DBManager.Instance.hasAscended);
            bool.TryParse(parts[1], out DBManager.Instance.hasMoved);
            bool.TryParse(parts[2], out DBManager.Instance.firstReset);
            bool.TryParse(parts[3], out DBManager.Instance.firstWind);
            bool.TryParse(parts[4], out DBManager.Instance.firstChase);
            bool.TryParse(parts[5], out DBManager.Instance.secoundChase);
            bool.TryParse(parts[6], out DBManager.Instance.firstCatch);
            int.TryParse(parts[7], out DBManager.Instance.lastMonolog);
            if (parts.Length > 8)
            {
                DBManager.Instance.isTutorialOK = parts[8];
            }
        }
    }

    public string TriggersToString()
    {
        if (DBManager.Instance == null) return "";
        return $"{DBManager.Instance.hasAscended},{DBManager.Instance.hasMoved},{DBManager.Instance.firstReset},{DBManager.Instance.firstWind},{DBManager.Instance.firstChase},{DBManager.Instance.secoundChase},{DBManager.Instance.firstCatch},{DBManager.Instance.lastMonolog},{DBManager.Instance.isTutorialOK}";
    }

    public void OnlineSave()
    {
        if (_isLoggedIn && DBManager.Instance != null)
        {
            triggersString = TriggersToString();
            StartCoroutine(SaveScoreRoutine(DBManager.Instance.Stored));
        }
    }

    public void EndTutorial()
    {
        if (_isLoggedIn && DBManager.Instance != null)
        {
            StartCoroutine(EndTutorialRoutine(DBManager.Instance.Stored));
        }
    }

    public void Logout()
    {
        if (_isLoggedIn)
        {
            Debug.Log($"[OnlineManager] Player '{email}' logged out.");
        }
        _isLoggedIn = false;
        Kaloznev = null;
        FavFood = null;
        email = null;
        PanelManager.Instance.loginFailedWindow.SetActive(true);
    }

    // --- Coroutines for Requests ---

    private IEnumerator VerifyPlayerRoutine(LoginData loginData)
    {
        var request = new VerifyPlayerRequest { loginData = loginData };
        string json = JsonUtility.ToJson(request);

        yield return PostRequest(json, (VerifyPlayerResponse response) =>
        {
            if (response.success)
            {
                email = response.userEmail;
                _isLoggedIn = true;
                triggersString = response.triggers;
                
                // Update DBManager
                if (DBManager.Instance != null)
                {
                    DBManager.Instance.Stored = response.credits;
                    Debug.Log($"[OnlineManager] Server sent credits: {response.credits}"); // Ellenőrző log
                    DBManager.Instance.PlayerName = Kaloznev;
                    TriggersfromString(triggersString);
                    DBManager.Instance.SaveToDBFile();
                }
                
                // UI frissítése és játék indítása
                if (PanelManager.Instance != null)
                {
                    PanelManager.Instance.OnLoginSuccess();
                    PanelManager.Instance.ShowWelcome(Kaloznev);
                }
                Debug.Log($"[OnlineManager] Login successful for {email}");
                Debug.Log($"[OnlineManager] Credits: {DBManager.Instance.Stored}");
                Debug.Log($"[OnlineManager] Triggers: {triggersString}");
            }
            else
            {
                Debug.LogWarning($"[OnlineManager] Login failed: {response.reason}");
                Logout();
            }
        });
    }

    private IEnumerator SaveScoreRoutine(int score)
    {
        var request = new SaveScoreRequest { email = email, score = score, triggersString = TriggersToString() };
        string json = JsonUtility.ToJson(request);
        yield return PostRequest(json, (SaveScoreResponse response) =>
        {
            if (response.success) Debug.Log("[OnlineManager] Score saved.");
            else Debug.LogError($"[OnlineManager] Save score failed: {response.reason}");
        });
    }

    private IEnumerator EndTutorialRoutine(int whiteHartya)
    {
        var request = new TutorialEndRequest
        {
            email = email,
            tutorialEnded = true,
            csonaknev = csonaknev,
            whiteHartya = whiteHartya,
            triggersString = TriggersToString()
        };
        string json = JsonUtility.ToJson(request);
        yield return PostRequest(json, (TutorialEndResponse response) =>
        {
            if (response.success) Debug.Log("[OnlineManager] Tutorial end sent.");
            else Debug.LogError($"[OnlineManager] Tutorial end failed: {response.reason}");
        });
    }

    // --- Generic POST Request Helper ---

    private IEnumerator PostRequest<T>(string jsonPayload, Action<T> callback)
    {
        // Debug.Log($"[OnlineManager] POST: {jsonPayload}");
        using (UnityWebRequest www = new UnityWebRequest(URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[OnlineManager] POST Error: {www.error}");
            }
            else
            {
                Debug.Log($"[OnlineManager] Response: {www.downloadHandler.text}");
                try
                {
                    T response = JsonUtility.FromJson<T>(www.downloadHandler.text);
                    callback?.Invoke(response);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[OnlineManager] Parse Error: {e.Message}");
                }
            }
        }
    }
}