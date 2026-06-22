using UnityEngine;

public class MapUI : MonoBehaviour
{
    public static MapUI Instance;

    [Header("UI")]
    public GameObject mapPanel;

    [Header("Input")]
    public KeyCode mapKey = KeyCode.M;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openMapSound;
    public AudioClip closeMapSound;

    [Range(0f, 1f)]
    public float soundVolume = 0.8f;

    private bool isOpen = false;

    private void Awake()
    {
        Instance = this;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }

        isOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(mapKey))
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        if (isOpen)
        {
            CloseMap();
        }
        else
        {
            OpenMap();
        }
    }

    public void OpenMap()
    {
        if (PlayerStats.Instance == null || !PlayerStats.Instance.hasMap)
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Карта не куплена");
            }

            return;
        }

        isOpen = true;

        if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }

        PlaySound(openMapSound);

        Time.timeScale = 0f;
    }

    public void CloseMap()
    {
        if (!isOpen) return;

        isOpen = false;

        if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }

        PlaySound(closeMapSound);

        Time.timeScale = 1f;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null) return;
        if (clip == null) return;

        audioSource.PlayOneShot(clip, soundVolume);
    }
}