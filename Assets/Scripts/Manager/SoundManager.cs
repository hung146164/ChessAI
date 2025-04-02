using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private Slider backgroundSlider;
    [SerializeField] private Slider sfxSlider;


    private AudioSource backgroundSource;
    private AudioSource SFXSource;

    [Header("SFX music")]
    public AudioData[] sfxAudio;

    [Header("Background Music")]
    public AudioData[] backgroundAudio;

    private Queue<AudioClip> backgroundQueue;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupAudioSources();
            SetupSoundSlider();
            SetupBackgroundList();

            StartCoroutine(PlayBackgroundMusic());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void SetupSoundSlider()
    {
        backgroundSlider.onValueChanged.AddListener(SetupVolumeBackground);
        sfxSlider.onValueChanged.AddListener(SetupVolumeSFX);

    }
    private void SetupVolumeBackground(float x)
    {
        backgroundSource.volume = x;
        Debug.Log("Change");
    }
    private void SetupVolumeSFX(float x)
    {
        SFXSource.volume = x;
        Debug.Log("Change");
    }
    private void SetupAudioSources()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            SFXSource = gameObject.AddComponent<AudioSource>();
            backgroundSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            SFXSource = audioSources[0];
            backgroundSource = audioSources[1];
        }

        backgroundSource.loop = false;
    }

    private void SetupBackgroundList()
    {
        backgroundQueue = new Queue<AudioClip>(backgroundAudio.Select(a => a.audioClip));
        if (backgroundQueue.Count == 0)
        {
            Debug.LogWarning("no background music found");
        }
    }

    private IEnumerator PlayBackgroundMusic()
    {
        while (true)
        {
            if (!backgroundSource.isPlaying && backgroundQueue.Count > 0)
            {
                AudioClip nextClip = backgroundQueue.Dequeue(); 
                backgroundSource.clip = nextClip;
                backgroundSource.Play();

                backgroundQueue.Enqueue(nextClip); 
            }
            yield return null; 
        }
    }

    public void PlaySFX(AudioClip audioClip)
    {
        SFXSource.PlayOneShot(audioClip);
    }

    public void PlaySFX(string name)
    {
        AudioClip audioClip = sfxAudio.Where(c => c.name == name).Select(p => p.audioClip).FirstOrDefault();
        if (audioClip != null)
        {
            SFXSource.PlayOneShot(audioClip);
        }
        else
        {
            Debug.LogWarning($"SFX {name} not found!");
        }
    }

    public void PlayBackground(AudioClip audioClip)
    {
        backgroundSource.clip = audioClip;
        backgroundSource.Play();
    }
}