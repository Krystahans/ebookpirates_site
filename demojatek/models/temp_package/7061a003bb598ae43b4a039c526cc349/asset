using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

public class MonologManager : MonoBehaviour
{
    public static MonologManager Instance { get; private set; }
    private GameObject ActiveWindow = null;
    public GameObject InGameMonolog;
    public bool IsMonologOn = false;
    private GameObject[] monologList;
    private GameObject[] PopUpList;
    private System.Action onCloseCallback;
    private GameObject interruptedMonolog;
    private System.Action interruptedCallback;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        fillMonologList();
        fillPopUpList();
        if (InGameMonolog != null) InGameMonolog.SetActive(false);
    }

    private void fillMonologList()
    {
        monologList = FindGameObjectsWithTagIncludingInactive("InGameMonolog")
            .OrderBy(go => GetNumericSortKey(go.name))
            .ToArray();
        Debug.Log("Monologs found: " + monologList.Length);
    }
    private void fillPopUpList()
    {
        PopUpList = FindGameObjectsWithTagIncludingInactive("PopUp")
            .OrderBy(go => GetNumericSortKey(go.name))
            .ToArray();
        Debug.Log("PopUps found: " + PopUpList.Length);
    }

    private int GetNumericSortKey(string name)
    {
        Match match = Regex.Match(name, @"\d+");
        if (match.Success && int.TryParse(match.Value, out int number))
            return number;
        return int.MaxValue; // Items without numbers will be sorted last
    }

    private GameObject[] FindGameObjectsWithTagIncludingInactive(string tag)
    {
        System.Collections.Generic.List<GameObject> result = new System.Collections.Generic.List<GameObject>();
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.hideFlags == HideFlags.None && go.scene.IsValid())
            {
                if (go.CompareTag(tag))
                {
                    result.Add(go);
                }
            }
        }
        return result.ToArray();
    }

    void Update()
    {
        if (PanelManager.Instance != null && PanelManager.Instance.loginWindow != null && PanelManager.Instance.loginWindow.activeInHierarchy) return;

        if (IsMonologOn)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Acknowledge();
            }
        }
    }

    private void Acknowledge()
    {
        Time.timeScale = 1f;
        ActiveWindow.SetActive(false);
        IsMonologOn = false;
        if (onCloseCallback != null)
        {
            System.Action callback = onCloseCallback;
            onCloseCallback = null;
            callback();
        }

        if (!IsMonologOn && interruptedMonolog != null)
        {
            ActiveWindow = interruptedMonolog;
            interruptedMonolog = null;
            PopUpOn();
            onCloseCallback = interruptedCallback;
            interruptedCallback = null;
        }
        else if (!IsMonologOn)
        {
            InGameMonolog.SetActive(false);
        }
    }

    public void AddOnCloseCallback(System.Action callback)
    {
        if (callback == null) return;
        if (onCloseCallback != null)
        {
            onCloseCallback += callback;
        }
        else
        {
            onCloseCallback = callback;
        }
    }

//PopUPs
    public void ShowPopUp(int index)
    {
        if (QuizManager.Instance != null && QuizManager.Instance.IsQuizActive)
        {
            QuizManager.Instance.AddOnCloseCallback(() => ShowPopUp(index));
            return;
        }
        if (IsMonologOn && ActiveWindow != null)
        {
            if (ActiveWindow.CompareTag("InGameMonolog"))
            {
                interruptedMonolog = ActiveWindow;
                interruptedMonolog.SetActive(false);
                interruptedCallback = onCloseCallback;
                onCloseCallback = null;
            }
            else ActiveWindow.SetActive(false);
        }
        ActiveWindow = PopUpList[index];
        PopUpOn();
    }
    private void PopUpOn()
    {
        IsMonologOn = true;
        InGameMonolog.SetActive(true);
        Time.timeScale = 0f;
        ActiveWindow.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void HowToMove()
    {
        ShowPopUp(0);
        DBManager.Instance.hasMoved = true;
        onCloseCallback = lvl0Quest;
        // Ja a pop-up 11.-et tedd be a howtomove után
        onCloseCallback = WindUp;
    }
    public void WrongAnswer()
    {
        ShowPopUp(1);
    }
    public void PickUp()
    {
        ShowPopUp(2);
        onCloseCallback = PlayAllMonologs;
    }
    public void Crashed(System.Action callback = null)
    {
        ShowPopUp(3);
        onCloseCallback = callback;
    }
    public void OffShore()
    {
        ShowPopUp(4);
    }
    public void EnemyCrash(System.Action callback = null)
    {
        if(!DBManager.Instance.firstCatch)
        {
            VideoManager.Instance.PlayCatchVideo();
            DBManager.Instance.firstCatch = true;
        }
        ShowPopUp(5);
        onCloseCallback = callback;
    }
    public void FirstCollect()
    {
        ShowPopUp(6);
    }
    public void EndOfMap()
    {
        ShowPopUp(8);
    }
    public void EndGame()
    {
        ShowPopUp(9);
    }

    public void NearChest()
    {
        ShowPopUp(10);
    }

    public void WindUp()
    {
        ShowPopUp(11);
        DBManager.Instance.firstWind = true;
        onCloseCallback = lvl0Quest;
    }
    public void BaloonUp()
    {
        ShowPopUp(12);
        // A 14_2.4 közvetlenül a 12. pop-up után
        if (IsTutorialActive())
        {
            onCloseCallback = () => ShowMonolog(14);
        }
    }
    public void EmptyCargo()
    {
        ShowPopUp(13);
    }
    public void lvl0Quest()
    {
        ShowPopUp(14);
    }
    public void Homecoming()
    {
        ShowPopUp(15);
    }
    public void RightAnswer()
    {
        ShowPopUp(16);
    }
    public void ChestCollected(System.Action callback = null)
    {
        ShowPopUp(17);
        // Videó indítása a popup bezárása után
        onCloseCallback = () => {
            if (VideoManager.Instance != null)
                VideoManager.Instance.PlayEoGVideo();
            if (callback != null) callback();
        };
    }

    // Segédfüggvény a tutorial státusz ellenőrzésére
    private bool IsTutorialActive()
    {
        if (SceneManager.Instance == null || DBManager.Instance == null) return false;
        // Csak akkor aktív, ha honlapról jött ÉS a tutorial még nem OK
        return SceneManager.Instance.isFromWebsite && DBManager.Instance.isTutorialOK != "ok";
    }

    // Új metódusok az új logikához
    public void ShowStartMonolog()
    {
        if (!IsTutorialActive()) return;
        // 01_1.2 -> Index 1 a listában
        ShowMonolog(1);
        // Amikor a monológ bezárul, jöjjön a HowToMove
        onCloseCallback = () => HowToMove();
    }

    public void ShowFirstPickup()
    {
        if (!IsTutorialActive()) return;
        // 00_1.1 -> Index 0
        // Utána Pop-up 02_Pick_up -> Index 2 a PopUp listában
        ShowMonolog(0);
        onCloseCallback = () => ShowPopUp(2);
    }

    public void ShowFirstDelivery()
    {
        if (!IsTutorialActive()) return;
        // 02_1.3 -> Index 2
        // Utána (kattintásra) 03_1.4 -> Index 3
        ShowMonolog(2);
        onCloseCallback = () => ShowMonolog(3);
    }

    public void ShowSecondPickup()
    {
        if (!IsTutorialActive()) return;
        // 04_1.5 -> Index 4
        // Utána 05_1.6 -> Index 5
        ShowMonolog(4);
        onCloseCallback = () => ShowMonolog(5);
    }

    public void ShowSecondDelivery()
    {
        if (!IsTutorialActive()) return;
        // 06_1.7 -> Index 6
        // Utána 07_1.8 -> Index 7
        ShowMonolog(6);
        onCloseCallback = () => ShowMonolog(7);
    }

    public void ShowThirdDelivery()
    {
        if (!IsTutorialActive()) return;
        // a 3. DeliveryCount után jön a 08, 09, 10, 11 és 12.
        ShowMonolog(8);
        onCloseCallback = () => {
            ShowMonolog(9);
            onCloseCallback = () => {
                ShowMonolog(10);
                onCloseCallback = () => {
                    ShowMonolog(11);
                    onCloseCallback = () => ShowMonolog(12);
                };
            };
        };
    }

    public void ShowPostAscensionDelivery()
    {
        if (!IsTutorialActive()) return;
        // A ballon-up utáni első DeliveryCount után közvetlenül jön a monologue 15, 16, 17. 18 és 19.
        ShowMonolog(15);
        onCloseCallback = () => {
            ShowMonolog(16);
            onCloseCallback = () => {
                ShowMonolog(17);
                onCloseCallback = () => {
                    ShowMonolog(18);
                    onCloseCallback = () => ShowMonolog(19);
                };
            };
        };
    }

    public void ShowFourthDelivery()
    {
        if (!IsTutorialActive()) return;
        // A 19. monológ utáni következő deliverynél jön a 20-30.
        ShowMonolog(20);
        onCloseCallback = () => {
            ShowMonolog(21);
            onCloseCallback = () => {
                ShowMonolog(22);
                onCloseCallback = () => {
                    ShowMonolog(23);
                    onCloseCallback = () => {
                        ShowMonolog(24);
                        onCloseCallback = () => {
                            ShowMonolog(25);
                            onCloseCallback = () => {
                                ShowMonolog(26);
                                onCloseCallback = () => {
                                    ShowMonolog(27);
                                    onCloseCallback = () => {
                                        ShowMonolog(28);
                                        onCloseCallback = () => {
                                            ShowMonolog(29);
                                            onCloseCallback = () => ShowMonolog(30);
                                        };
                                    };
                                };
                            };
                        };
                    };
                };
            };
        };
    }

//Monologes
    public void PlayAllMonologs()
    {
        if (monologList != null && monologList.Length > 0)
        {
            ShowMonolog(DBManager.Instance.lastMonolog);
        }
    }
    public void ShowMonolog(int index)
    {
        if (QuizManager.Instance != null && QuizManager.Instance.IsQuizActive)
        {
            QuizManager.Instance.AddOnCloseCallback(() => ShowMonolog(index));
            return;
        }

        DBManager.Instance.lastMonolog = index;
        if (index >= monologList.Length) return;
        ActiveWindow = monologList[index];
        PopUpOn();
        onCloseCallback = () => StartCoroutine(WaitAndShowNext(index));
    }

    private System.Collections.IEnumerator WaitAndShowNext(int index)
    {
        switch (index)
        {
            // A case 1-et kivettük, mert most már a TopBarManager vezérli a leadást
            case 13:
                yield return new WaitUntil(() => DBManager.Instance.hasAscended);
                break;
            case 14:
                yield return new WaitUntil(() => DBManager.Instance.firstChase);
                break;
            case 19:
                yield return new WaitUntil(() => DBManager.Instance.secoundChase);
                break;
            default:
                break;
        }
        yield return new WaitUntil(() => !IsBusy());
        ShowMonolog(index + 1);
    }

    private bool IsBusy()
    {
        if (ActiveWindow != null && ActiveWindow.activeInHierarchy && ActiveWindow.CompareTag("PopUp")) return true;
        if (SceneManager.Instance != null && SceneManager.Instance.ascensionCam != null && SceneManager.Instance.ascensionCam.activeInHierarchy) return true;
        if (QuizManager.Instance != null && QuizManager.Instance.quizPanel != null && QuizManager.Instance.quizPanel.activeInHierarchy) return true;
        return false;
    }    
}
