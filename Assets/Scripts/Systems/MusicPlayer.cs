using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip track;
    private AudioSource source;
    public static MusicPlayer Instance { get; private set; }
    private PersistantPlayerStats playerStats;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerStats = PersistantPlayerStats.Instance;


        source = GetComponent<AudioSource>();
        source.clip = track;
        source.volume = playerStats.musicVolume;
        source.loop = true;
        source.Play();

        playerStats.OnMusicVolumeChanged += HandleVolumeChanged;
    }

    private void HandleVolumeChanged(float newVolume)
    {
        source.volume = newVolume;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (playerStats != null)
        {
            playerStats.OnMusicVolumeChanged -= HandleVolumeChanged;
        }
    }
}
