using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class OpeningManager : MonoBehaviour
{
    public static OpeningManager Instance { get; private set; }
    public GameObject OpeningScene;

    [Tooltip("The sequence of UI panels to show during the opening sequence.")]
    public GameObject[] openingPanels;

    private int currentPanelIndex = -1;

    // This method is called when the object becomes enabled and active.
    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Start the sequence from the beginning
        InitializeOpeningSequence();
    }

    // Initializes and starts the opening sequence.
    private void InitializeOpeningSequence()
    {
        GameObject monologBox = GameObject.Find("MonologBox");
        if (monologBox == null)
        {
            MonologManager mm = MonologManager.Instance ?? FindAnyObjectByType<MonologManager>();
            if (mm != null && mm.InGameMonolog != null && mm.InGameMonolog.name == "MonologBox")
                monologBox = mm.InGameMonolog;
        }

        if (monologBox != null)
        {
            List<GameObject> welcomes = new List<GameObject>();
            for (int i = 1; i <= 5; i++)
            {
                Transform t = monologBox.transform.Find("Welcome" + i);
                if (t != null) welcomes.Add(t.gameObject);
            }
            if (welcomes.Count > 0) openingPanels = welcomes.ToArray();
        }

        if(OpeningScene != null)
            OpeningScene.SetActive(true);

        // Ensure all panels are initially hidden
        foreach (var panel in openingPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        currentPanelIndex = -1;
        ShowNextPanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (PanelManager.Instance != null && PanelManager.Instance.loginWindow != null && PanelManager.Instance.loginWindow.activeInHierarchy) return;

        // Advance to the next panel when the space key is pressed
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            ShowNextPanel();
        }
    }

    /// <summary>
    /// Deactivates the current panel and activates the next one in the sequence.
    /// If the sequence is over, it transitions to the player state.
    /// </summary>
    private void ShowNextPanel()
    {
        // Deactivate the current panel if it's valid
        if (currentPanelIndex >= 0 && currentPanelIndex < openingPanels.Length)
        {
            if (openingPanels[currentPanelIndex] != null)
            {
                openingPanels[currentPanelIndex].SetActive(false);
            }
        }

        currentPanelIndex++;

        // If there's a next panel, show it
        if (currentPanelIndex < openingPanels.Length)
        {
            if (openingPanels[currentPanelIndex] != null)
            {
                openingPanels[currentPanelIndex].SetActive(true);
            }
        }
        else
        {
            // End of sequence: initialize the player and let SceneManager deactivate this scene
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.InitializePlayer();
                OpeningScene.SetActive(false);
                
                // Amikor elindítja a hajót, felugrik a Monologues 01_1.2, utána a HowToMove
                if (MonologManager.Instance != null) MonologManager.Instance.ShowStartMonolog();
                this.enabled = false; // Disable this manager
            }
        }
    }
}
