using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// These variables persist for as long as the game is running, regardless of scene changing
/// </summary>
public class Settings : MonoBehaviour
{
    public static Settings Instance;

    public float masterVolume = 0.5f;
    public float sfxVolume = 0.5f;
    public float musVolume = 0.5f;
    public float animationSpeed = 1.5f;
    public int difficulty = 1;

    // public float[] zones = { 300f, 600f, 900f }; // Enemy spawning zones (DROPPED IDEA)
    public float enemySpawnRate = 1f;

    AudioManager audioManager;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null) // Remove extra instances
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioManager = GetComponent<AudioManager>();
        
    }

    private void Start()
    {
        SetMasterVolume(masterVolume);
        SetMusVolume(musVolume);
        SetSfxVolume(sfxVolume);

        audioManager.PlayMusic("OST_Title");
    }

    public void SetAnimationSpeed(float spd) { animationSpeed = spd; }
    public void SetMasterVolume(float vol) { masterVolume = vol;
        audioManager.SetLevelHelper(vol, "Master");
        Debug.Log("Master vol changed");
    }
    public void SetMusVolume(float vol) { musVolume = vol;
        audioManager.SetLevelHelper(vol, "Music");
    }
    public void SetSfxVolume(float vol) { sfxVolume = vol;
        audioManager.SetLevelHelper(vol, "SFX");
    }
    public void SetEnemyDifficulty(float dif) { difficulty = (int) dif; }
}
