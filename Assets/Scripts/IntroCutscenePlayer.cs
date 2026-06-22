using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroCutscenePlayer : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage screen;
    public VideoClip[] cutsceneClips;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Scene")]
    public string sceneToLoad = "Level_1";

    [Header("Skip")]
    public KeyCode skipKey = KeyCode.Space;
    public bool allowSkip = true;

    private bool skipRequested;

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (videoPlayer != null && audioSource != null)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }

    private void Start()
    {
        StartCoroutine(PlayCutscenes());
    }

    private void Update()
    {
        if (!allowSkip) return;

        if (Input.GetKeyDown(skipKey))
        {
            skipRequested = true;
        }
    }

    private IEnumerator PlayCutscenes()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer is not assigned");
            LoadNextScene();
            yield break;
        }

        if (cutsceneClips == null || cutsceneClips.Length == 0)
        {
            Debug.LogWarning("No cutscene clips assigned");
            LoadNextScene();
            yield break;
        }

        for (int i = 0; i < cutsceneClips.Length; i++)
        {
            VideoClip clip = cutsceneClips[i];

            if (clip == null) continue;

            skipRequested = false;

            videoPlayer.Stop();
            videoPlayer.clip = clip;
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            videoPlayer.Play();

            if (audioSource != null)
            {
                audioSource.Play();
            }

            while (videoPlayer.isPlaying)
            {
                if (skipRequested)
                {
                    videoPlayer.Stop();

                    if (audioSource != null)
                    {
                        audioSource.Stop();
                    }

                    break;
                }

                yield return null;
            }
        }

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}