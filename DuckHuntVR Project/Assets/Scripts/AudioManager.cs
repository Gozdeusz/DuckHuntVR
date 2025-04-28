using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    //Pojedyncze dzwieki
    public AudioSource audioSource;

    //Lista dzwiekow
    public AudioClip[] audioClips;

    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();
    public float defaultCooldown = 0.5f;

    //Kontrola czy jest tylko jedna instacja menadzera
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
        audioSource = GetComponent<AudioSource>();
    }

    private AudioClip GetClipByName(string name)
    {
        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = System.Array.Find(audioClips, clip => clip.name == soundName);
        if (clip == null)
        {
            Debug.LogError($"Nie znaleziono dŸwiêku o nazwie: {soundName}");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void PlayRandomSound(string[] soundNames) {


        List<AudioClip> clipsToChooseFrom = new List<AudioClip>();
        foreach (string name in soundNames)
        {
            AudioClip clip = System.Array.Find(audioClips, clip => clip.name == name);
            if (clip != null)
            {
                clipsToChooseFrom.Add(clip);
            }
        }

        AudioClip randomClip = clipsToChooseFrom[Random.Range(0, clipsToChooseFrom.Count)];
        audioSource.PlayOneShot(randomClip); ;
    }

    public void PlaySoundWithCooldown(string soundName, float cooldown = -1)
    {
        if (cooldown < 0) cooldown = defaultCooldown; // U¿ycie domyœlnego cooldownu, jeœli nie podano

        if (soundCooldowns.ContainsKey(soundName))
        {
            if (Time.time < soundCooldowns[soundName])
            {
                // DŸwiêk jest jeszcze na cooldownie, wiêc go nie odtwarzamy
                return;
            }
        }

        // Odtwarzamy dŸwiêk i ustawiamy cooldown
        PlaySound(soundName);
    }
}
