using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance;
    public float music_multiplier;
    public float sfx_multiplier;

    // Sounds
    public Sound[] sounds;

    // BGMs
    public Sound[] bgms;
    private float _defaultBGMVolume = 0.5f;
    [SerializeField] public string playingBGM;
    [SerializeField] private string playingBGMVolume; // only for debugging

    [SerializeField] private string nextBGM; // only for debugging

    // Ding SFX Pitch
    public float DingPitchMin = 0.5f;
    public float DingPitchMax = 3.0f;

    // Initialize the singleton instance
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey("music_volume_multiplier"))
        {

            music_multiplier = PlayerPrefs.GetFloat("music_volume_multiplier");
        }
        if (PlayerPrefs.HasKey("sfx_volume_multiplier"))
        {
            sfx_multiplier = PlayerPrefs.GetFloat("sfx_volume_multiplier");
        }
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        foreach (Sound s in bgms)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    private void Start()
    {
        // sleep for 1 second to allow the game to load

        if (playingBGM != null && playingBGM != "")
        {
            PlayBGM(playingBGM);
        }
        else
        {
            PlayBGM("BGM Isolation"); // default
        }
    }

    public void Update()
    {
        // Update volume
        Sound playingSound = System.Array.Find(bgms, bgm => bgm.name == playingBGM);
        if (playingSound != null)
        {
            playingBGMVolume = playingSound.source.volume.ToString();
        }


        // If nextBGM not found, do nothing
        Sound nextSound = System.Array.Find(bgms, bgm => bgm.name == nextBGM);
        if (nextSound != null)
        {
            if (nextBGM != playingBGM)
            {
                PlayBGM(nextBGM);
                nextBGM = null;
            }
        }

    }

    public void PlayJump()
    {
        Play("Jump");
    }

    // Calculate pitch based on current count and total count
    // Designed such that pitch increases as the current count approaches the total count
    public void PlayDing(int currentCount, int totalCount)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == "Ding");
        if (s == null)
        {
            Debug.LogWarning("Sound: Ding not found!");
            return;
        }
        s.source.pitch = Mathf.Lerp(DingPitchMin, DingPitchMax, (float)(totalCount - currentCount) / totalCount);
        /*Debug.Log("Ding pitch: " + s.source.pitch + " (currentCount: " + currentCount + ", totalCount: " + totalCount + ")");*/
        Play("Ding");
    }

    public void PlayMagicCoin()
    {
        Play("MagicCoin");
    }

    public void PlaySlash()
    {
        Play("Slash");
    }

    public void PlayHack()
    {
        Play("Hack");
    }

    public void PlayDash()
    {
        Play("Dash");
    }

    public void PlayShoot()
    {
        Play("Shoot Single");
    }

    public void PlayBulletImpact()
    {
        PlayOneOf(new string[] { "Bullet Impact 1", "Bullet Impact 2" });
    }

    public void PlaySwordImpact()
    {
        PlayOneOf(new string[] { "Sword Impact 1" });
    }

    public void PlayPressurePlateUp()
    {
        Play("Pressure Plate Up");
    }

    public void PlayExplosion()
    {
        Play("Explosion");
    }

    public void StopBGM()
    {
        foreach (Sound s in bgms)
        {
            if (s.source.isPlaying)
            {
                // Fade out BGM
                StartCoroutine(FadeOut(playingBGM, 2.0f));
            }
        }
    }

    IEnumerator FadeOut(string name, float fadeTime)
    {
        Debug.Log("Fading out " + name);
        Sound s = System.Array.Find(bgms, bgm => bgm.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            yield break;
        }
        float startVolume = s.source.volume;

        while (s.source.volume > 0f)
        {
            s.source.volume -= startVolume * Time.deltaTime / fadeTime;
        }

        s.source.Stop(); // Stop the audio after fade out
        s.source.loop = false;
        Debug.Log("Stopped " + name);
        yield return null;
    }
    IEnumerator FadeIn(string name, float fadeTime)
    {
        Debug.Log("Fading in " + name);
        Sound s = System.Array.Find(bgms, bgm => bgm.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            yield break;
        }
        float startVolume = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            s.source.volume = Mathf.Lerp(startVolume, s.volume, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
        }

        s.source.volume = s.volume; // Ensure final volume is set
        s.source.loop = true;
        Debug.Log("Playing " + name);
        yield return null;
    }

    public void PlayBGM(string name)
    {
        Sound s = System.Array.Find(bgms, bgm => bgm.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
        playingBGM = name;
        Debug.Log("Playing BGM: " + name);
        return;
        StopBGM(); // Stop current BGM
        s = System.Array.Find(bgms, bgm => bgm.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = music_multiplier;
        playingBGM = name;
        s.source.Play();
        StartCoroutine(FadeIn(name, 2.0f));
    }

    private void Play(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = sfx_multiplier;
        s.source.Play();
    }

    private void PlayOneOf(string[] names)
    {
        foreach (string name in names)
        {
            Sound s = System.Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }
            s.source.Play();
            /*Debug.Log("Playing sound: " + name);*/
        }

        // Random number between 0 to n-1
        int randomIndex = Random.Range(0, names.Length);
        sounds[randomIndex].source.Play();
        /*Debug.Log("Playing sound: " + names[randomIndex]);*/
    }


}