using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{    
    public static AudioManager manager {get; private set;}
    public List<Sounds> music;
    public List<Sounds> sfx;

    void Awake()
    {
        // Singleton pattern
        if (manager != null && manager != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            manager = this;
            DontDestroyOnLoad(this);
        }

        // Init AudioSources
        foreach (Sounds song in music)
        {
            song.source = gameObject.AddComponent<AudioSource>();
            song.source.clip = song.clip;
            song.source.volume = Settings.Instance.volumeMusic * Settings.Instance.volumeMaster;
            song.source.loop = song.loop;
        }

        foreach (Sounds sound in sfx)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = Settings.Instance.volumeSFX * Settings.Instance.volumeMaster;
            sound.source.loop = sound.loop;
        }

        SceneManager.activeSceneChanged += HandleOST;
    }

    public void PlayMusic(string songName)
    {
        foreach (Sounds song in music)
        {
            if (song.clip.name == songName)
            {
                song.source.Play();
                return;
            }
        }
        Debug.LogWarning("Song \"" + songName + "\" not found.");
    }

    public void FadeMusic(string songName, float duration, bool isFadingIn)
    {
        foreach (Sounds song in music)
        {
            if (song.clip.name == songName)
            {
                StartCoroutine(FadeHelper(song, duration, isFadingIn));
                return;
            }
        }
        Debug.LogWarning("Song \"" + songName + "\" not found.");
    }

    IEnumerator FadeHelper(Sounds song, float duration, bool isFadingIn)
    {
        float fullVolume = Settings.Instance.volumeMusic * Settings.Instance.volumeMaster;
        float tempTimer = 0;
        if (isFadingIn)
        {
            song.source.volume = 0f;
            song.source.Play();
            while (tempTimer < duration)
            {
                tempTimer += Time.deltaTime;
                song.source.volume = Mathf.Lerp(0f, fullVolume, tempTimer / duration);
                yield return null;
            }
            yield break;
        }
        else //fading out
        {
            while (tempTimer < duration)
            {
                tempTimer += Time.deltaTime;
                song.source.volume = Mathf.Lerp(fullVolume, 0f, tempTimer / duration);
                yield return null;
            }
            song.source.Stop();
            song.source.volume = fullVolume;
            yield break;
        }
    }

    // public void Crossfade(string newSongName)
    // {
    //     foreach (Sounds song in music)
    //     {
    //         if (song.source.isPlaying)
    //         {
    //             FadeMusic(song.clip.name, 3f, false);
    //         }
    //     }
    //     FadeMusic(newSongName, 3f, true);
    // }

    public void CrossfadeSeq(string newSongName, float duration)
    {
        StartCoroutine(CrossfadeSeqHelper(newSongName, duration));
    }

    private IEnumerator CrossfadeSeqHelper(string newSongName, float duration)
    {
        bool wasPlaying = false;
        foreach (Sounds song in music)
        {
            if (song.source.isPlaying)
            {
                wasPlaying = true;
                FadeMusic(song.clip.name, duration, false);
            }
        }
        if (wasPlaying) yield return new WaitForSeconds(duration);
        FadeMusic(newSongName, duration, true);
        yield break;
    }

    public void HandleOST(Scene sceneOld, Scene sceneNew)
    {
        switch (sceneNew.name)
        {
            case "Title":
                CrossfadeSeq("OST_Title", 2f);
                break;
            case "Main":
                CrossfadeSeq("OST_Day1", 2f);
                break;
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

            Sounds sound = soundPool[UnityEngine.Random.Range(0, soundPool.Count)];
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

    public void UpdateVolume()
    {
        float finalVolumeMusic = Settings.Instance.volumeMusic * Settings.Instance.volumeMaster;
        float finalVolumeSFX = Settings.Instance.volumeSFX * Settings.Instance.volumeMaster;
        foreach (Sounds sound in sfx)
        {
            sound.source.volume = finalVolumeSFX;
        }
        foreach (Sounds song in music)
        {
            song.source.volume = finalVolumeMusic;
        }
    }

    // public void SetLevel(float sliderVolume)
    // {
    //     Slider slider = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
    //     if (slider.name.Contains("Music"))
    //     {
    //         SetLevelHelper(sliderVolume, "Music");
    //     }
    //     else if (slider.name.Contains("SFX"))
    //     {
    //         SetLevelHelper(sliderVolume, "SFX");
    //     }
    //     else SetLevelHelper(sliderVolume, "Master");
    // }

    // public void SetLevel(float volume, float audioManagerVolume)
    // {
    //     if (audioManagerVolume == volumeMusic) volumeMusic = volume;
    //     else if (audioManagerVolume == volumeSFX) volumeSFX = volume;
    //     else if (audioManagerVolume == volumeMaster) volumeMaster = volume;
    //     UpdateVolume();
    // }
}
