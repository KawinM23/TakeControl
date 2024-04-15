using UnityEngine.Audio;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance;
    public float music_multiplier;
    public float sfx_multiplier;

    // Sounds
    public Sound[] sounds;

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

            music_multiplier= PlayerPrefs.GetFloat("music_volume_multiplier");
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
    }

    private void Start()
    {
        // sleep for 1 second to allow the game to load
       
        PlayBGM();
    }

    public void PlayJump()
    {
        Debug.Log(music_multiplier);
        Debug.Log(sfx_multiplier);
        Play("Jump");
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

    public void PlayBGM()
    {
        Play("BGM Isolation");
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

    private void Play(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
        Debug.Log("Playing sound: " + name);
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
            Debug.Log("Playing sound: " + name);
        }

        // Random number between 0 to n-1
        int randomIndex = Random.Range(0, names.Length);
        sounds[randomIndex].source.Play();
        Debug.Log("Playing sound: " + names[randomIndex]);
    }


}