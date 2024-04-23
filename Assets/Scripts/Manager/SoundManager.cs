using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance;
    public float music_multiplier;
    public float sfx_multiplier;
    private float currentMusicMultiplier;
    private float currentSFXMultiplier;

    // Sounds
    public Sound[] sounds;

    // BGMs
    public Sound[] bgms;
    private float _defaultBGMVolume = 0.5f;
    [SerializeField] public string playingBGM;
    [SerializeField] private string playingBGMVolume; // only for debugging

    [SerializeField] public string nextBGM;

    // BGM Names
    public string BGMMenu = "MEN NeoCityDive";
    public string BGMExploration = "EXP BlueTwilight";
    public List<string> BGMCombat = new List<string> { "CMB VirtualStorm", "CMB Crossfire", "CMB TechNTech" };

    // Scene Names
    public string _prev_scene;
    public string _current_scene;

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
        Debug.Log("bgm length" + bgms.Length);
        foreach (Sound s in bgms)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    private void Start() {
        // Play BGM based on scene name
        _current_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        PlayBGMBySceneName(_current_scene);
        _prev_scene = _current_scene;
    }

    private void Update()
    {
        // If other BGM is queued, play it
        if (nextBGM != null && nextBGM != "" && nextBGM != playingBGM)
        {
            Sound s = System.Array.Find(bgms, bgm => bgm.name == nextBGM);
            if (s != null)
            {
                PlayBGM(nextBGM);
                nextBGM = "";
            }
        }

        // Update Scene Name
        _current_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (_current_scene != _prev_scene)
        {
            // Play BGM based on scene name
            PlayBGMBySceneName(_current_scene);
            _prev_scene = _current_scene;
        }

        // Update volume
        if (currentMusicMultiplier != music_multiplier)
        {
            foreach (Sound s in bgms)
            {
                s.source.volume = music_multiplier;
            }
            currentMusicMultiplier = music_multiplier;
        }
        if (currentSFXMultiplier != sfx_multiplier)
        {
            foreach (Sound s in sounds)
            {
                s.source.volume = sfx_multiplier;
            }
            currentSFXMultiplier = sfx_multiplier;
        }
    }

    public void PlayBGMBySceneName(string sceneName) {
        // Hardcode here to override
        if (sceneName=="Map2-12") {
            PlayBGMCombatRandom();
            return; // dont forget to return
        }

        // Substring check here
        if (sceneName.Contains("Menu") || sceneName.Contains("Option"))
        {
            nextBGM=BGMMenu;
        }
        else if (sceneName.Contains("Map"))
        {
            nextBGM=BGMExploration;
        }
        else if (sceneName.Contains("Boss") || sceneName.Contains("Combat"))
        {
            PlayBGMCombatRandom();
        }
        else
        {
            nextBGM=BGMExploration; // default
        }
    }

    public string GetRandomCombatBGM() {
        return BGMCombat[Random.Range(0, BGMCombat.Count)];
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

    public void PlayJumpPad() {
        Play("JumpPad");
    }

    public void PlayLaser(){
        Play("Laser");
    }

    public void PlayHover() {
        Play("Hover");
    }

    public void PlayConfirm() {
        Play("Confirm");
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
            yield return new WaitForEndOfFrame();
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
            yield return new WaitForEndOfFrame();
        }

        s.source.volume = s.volume; // Ensure final volume is set
        s.source.loop = true;
        Debug.Log("Playing " + name);
        yield return null;
    }

    public void PlayBGMCombatRandom()
    {
        List<Sound> combatBGMs = new List<Sound>();
        foreach (Sound s in bgms)
        {
            // if name has "CMB" as substring
            if (s.name.Contains("CMB"))
            {
                combatBGMs.Add(s);
            }
        }
        if (combatBGMs.Count == 0)
        {
            Debug.LogWarning("No combat BGMs found!");
            return;
        }
        nextBGM = combatBGMs[Random.Range(0, combatBGMs.Count)].name;
    }

    public void PlayBGM(string name)
    {
        StopBGM(); // Stop current BGM
        Sound s = System.Array.Find(bgms, bgm => bgm.name == name);
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