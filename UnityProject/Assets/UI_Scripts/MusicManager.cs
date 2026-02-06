using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource clickSound;
    public bool soundEnabled = true;
    public AudioSource museGiggle;
    public float giggleInterval = 120f;
    public bool gigglesEnabled = true;
    private float giggleTimer;
    private static MusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (soundEnabled && clickSound != null && Input.GetMouseButtonDown(0))
        {
            clickSound.Play();
        }
        // Handle Muse giggle
        if (museGiggle != null && gigglesEnabled)
        {
            giggleTimer += Time.deltaTime;
            if (giggleTimer >= giggleInterval)
            {
                museGiggle.Play();
                giggleTimer = 0f;
            }
        }
    }

    public void ToggleMusic(bool isOn)
    {
        audioSource.mute = !isOn;
    }
    public void ToggleMusicButton()
    {
        audioSource.mute = !audioSource.mute;
    }
    public void PlayClick()
    {
        if (soundEnabled && clickSound != null)
            clickSound.Play();
    }

    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;
    }
    public void ToggleGiggles()
    {
        gigglesEnabled = !gigglesEnabled;
    }
}