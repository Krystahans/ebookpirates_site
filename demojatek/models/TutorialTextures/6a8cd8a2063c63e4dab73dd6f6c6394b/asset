using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public Button SoundON;
    public Button SoundOFF;
    public AudioSource WaveSoundSource;
    public AudioSource HartyaSoundSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        
        Debug.Log("SoundManager Start");
        if (SoundON != null)
        {
            SoundON.onClick.AddListener(SoundOnClick);
        }
        if (SoundOFF != null)
        {
            SoundOFF.onClick.AddListener(SoundOffClick);
        }
    }

    public void SoundOnClick()
    {
        if (SoundON != null)
        {
            Debug.Log("SoundOnClick");
            SoundON.gameObject.SetActive(false);
            SoundOFF.gameObject.SetActive(true);
            WaveSoundSource.mute = true;
            HartyaSoundSource.mute = true;
        }
    }
    public void SoundOffClick()
    {
        if (SoundOFF != null)
        {
            Debug.Log("SoundOffClick");
            SoundOFF.gameObject.SetActive(false);
            SoundON.gameObject.SetActive(true);
            WaveSoundSource.mute = false;
            HartyaSoundSource.mute = false;
        }
    }
}
