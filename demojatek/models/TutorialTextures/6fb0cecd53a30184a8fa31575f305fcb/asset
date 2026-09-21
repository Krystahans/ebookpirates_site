using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    public Button MusicON;
    public Button MusicOFF;
    public AudioSource MusicSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("MusicManager Start");
        if (MusicON != null)
        {
            MusicON.onClick.AddListener(MusicOnClick);
        }
        if (MusicOFF != null)
        {
            MusicOFF.onClick.AddListener(MusicOffClick);
        }
    }

    public void MusicOnClick()
    {
        if (MusicON != null)
        {
            Debug.Log("MusicOnClick");
            MusicON.gameObject.SetActive(false);
            MusicOFF.gameObject.SetActive(true);
            if (MusicSource != null) MusicSource.mute = true;
        }
    }
    public void MusicOffClick()
    {
        if (MusicOFF != null)
        {
            Debug.Log("MusicOffClick");
            MusicOFF.gameObject.SetActive(false);
            MusicON.gameObject.SetActive(true);
            if (MusicSource != null) MusicSource.mute = false;
        }
    }
}
