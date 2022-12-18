using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{    
    public static AudioManager manager {get; private set;}
    public List<Sounds> music;
    public List<Sounds> sfx;
    private float volumeMaster;
    private float volumeSFX;
    private float volumeMusic;

    void Awake()
    {
        // Singleton pattern
        if (manager != null && manager != this)
        {
            Destroy(manager);
        }
        else
        {
            manager = this;
            DontDestroyOnLoad(manager);
        }

        // Init AudioSources
        foreach (Sounds song in music)
        {
            song.source = gameObject.AddComponent<AudioSource>();
            song.source.clip = song.clip;
            // sound.source.volume = sound.volume / 100f;
            song.source.loop = song.loop;
        }

        foreach (Sounds sound in sfx)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            // sound.source.volume = sound.volume / 100f;
            sound.source.loop = sound.loop;
        }

        // Init volume values
        volumeMaster = 1f;
        volumeMusic = 1f;
        volumeSFX = 1f;
    }

    public void PlayMusic(string songName)
    {
        
        foreach (Sounds song in music)
        {
            if (song.clip.name == songName)
            {
                song.source.Play();
                break;
            }
            else
            {
                Debug.LogWarning("Song \"" + song.clip.name + "\" not found.");
            }
        }
    }

    public void PlaySFX(string sfxName)
    {
        foreach (Sounds sound in sfx)
        {
            if (sound.clip.name == sfxName)
            {
                sound.source.PlayOneShot(sound.clip);
                return;
            }
        }
        Debug.LogWarning("SFX \"" + sfxName + "\" not found.");
    }
    
    public void PlayRandomSFX(string sfxName)
    {
        List<Sounds> soundPool = new List<Sounds>();
        foreach (Sounds sound in sfx)
        {
            // if clip name matches front part
            if (sound.clip.name.Contains(sfxName))
            {
                soundPool.Add(sound);
            }
        }

        if (soundPool.Count > 0)
        {

            Sounds sound = soundPool[Random.Range(0, soundPool.Count)];
            sound.source.PlayOneShot(sound.clip);
        }
        else
        {
            Debug.LogWarning("SFX \"" + sfxName + "\" not found.");
        }
    }

    public void StopMusic()
    {
        foreach(Sounds song in music)
        {
            song.source.Stop();
        }
    }

    //added this
    public void StopSFXName(string sfxName)
    {
        foreach (Sounds sound in sfx)
        {
            if(sound.clip.name.Contains(sfxName))
                sound.source.Stop();
        }
    }

    public void StopSFX()
    {
        foreach(Sounds sound in sfx)
        {
            sound.source.Stop();
        }
    }

    public void StopAllAudio()
    {
        StopMusic();
        StopSFX();
    }

    void UpdateVolume()
    {
        float finalVolumeMusic = volumeMusic * volumeMaster;
        float finalVolumeSFX = volumeSFX * volumeMaster;
        foreach (Sounds sound in sfx)
        {
            sound.source.volume = finalVolumeSFX;
        }
        foreach (Sounds song in music)
        {
            song.source.volume = finalVolumeMusic;
        }
    }

    public void SetLevel(float sliderVolume)
    {
        Slider slider = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
        if (slider.name.Contains("Music"))
        {
            SetLevelHelper(sliderVolume, "Music");
        }
        else if (slider.name.Contains("SFX"))
        {
            SetLevelHelper(sliderVolume, "SFX");
        }
        else SetLevelHelper(sliderVolume, "Master");
    }

    public void SetLevelHelper(float volume, string Music_or_SFX_or_Master)
    {
        switch (Music_or_SFX_or_Master)
        {
            case "Music":
                volumeMusic = volume;
                break;
            case "SFX":
                volumeSFX = volume;
                break;
            case "Master":
                volumeMaster = volume;
                break;
        }
        UpdateVolume();
    }
}
