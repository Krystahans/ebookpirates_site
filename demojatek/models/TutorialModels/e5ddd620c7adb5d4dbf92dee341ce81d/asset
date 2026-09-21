using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance { get; private set; }
    public GameObject videoScreen; // Assign this in the Inspector to the panel that contains the VideoPlayer
    [SerializeField] string EoGvideoName;
    [SerializeField] string CatchvideoName;
    private bool musicWasMuted;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void PlayEoGVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        // Csak a képernyőt kapcsoljuk be, a managert nem kell
        videoScreen.SetActive(true);
        if(videoPlayer != null)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, EoGvideoName);
            Debug.Log("Playing EoG video from path: " + videoPath);
            videoPlayer.url = videoPath;
            HandleAudioStart(videoPlayer);
            videoPlayer.loopPointReached += OnEoGVideoEnd;
            videoPlayer.Play();
            Time.timeScale = 0f; // Pause the game while the video is playing
        }
        else
        {
            Debug.LogError("VideoPlayer component not found on " + gameObject.name);
        }
    }
    public void PlayCatchVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        videoScreen.SetActive(true);
        if(videoPlayer != null)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, CatchvideoName);
            Debug.Log("Playing Catch video from path: " + videoPath);
            videoPlayer.url = videoPath;
            HandleAudioStart(videoPlayer);
            videoPlayer.loopPointReached += OnCatchVideoEnd;
            videoPlayer.Play();
            Time.timeScale = 0f; // Pause the game while the video is playing
        }
        else
        {
            Debug.LogError("VideoPlayer component not found on " + gameObject.name);
        }
    }
    private void HandleAudioStart(VideoPlayer vp)
    {
        if (MusicManager.Instance != null)
        {
            musicWasMuted = MusicManager.Instance.MusicSource.mute;
            if (musicWasMuted)
            {
                vp.SetDirectAudioMute(0, true);
            }
            else
            {
                vp.SetDirectAudioMute(0, false);
                MusicManager.Instance.MusicSource.mute = true;
            }
        }

        if (SoundManager.Instance != null)
        {
            if (SoundManager.Instance.WaveSoundSource != null) SoundManager.Instance.WaveSoundSource.mute = true;
            if (SoundManager.Instance.HartyaSoundSource != null) SoundManager.Instance.HartyaSoundSource.mute = true;
        }
    }

    private void OnCatchVideoEnd(VideoPlayer vp)
    {
        vp.loopPointReached -= OnCatchVideoEnd;
        RestoreAudio();
        videoScreen.SetActive(false);
        // A Catch videó alatt/után valószínűleg egy PopUp is aktív (pl. EnemyCrash), 
        // így a Time.timeScale marad 0, amíg a játékos azt le nem okézza.
    }

    private void OnEoGVideoEnd(VideoPlayer vp)
    {
        vp.loopPointReached -= OnEoGVideoEnd;
        RestoreAudio();
        videoScreen.SetActive(false);
        
        // Itt indítjuk el a végső paneleket (EoGTab, EoGWindow1, stb.)
        if (PanelManager.Instance != null)
            PanelManager.Instance.ActivateEoG();
    }

    private void RestoreAudio()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.MusicSource.mute = musicWasMuted;
        }

        if (SoundManager.Instance != null && SoundManager.Instance.SoundON != null)
        {
            bool soundOn = SoundManager.Instance.SoundON.gameObject.activeSelf;
            if (SoundManager.Instance.WaveSoundSource != null) SoundManager.Instance.WaveSoundSource.mute = !soundOn;
            if (SoundManager.Instance.HartyaSoundSource != null) SoundManager.Instance.HartyaSoundSource.mute = !soundOn;
        }
    }
}