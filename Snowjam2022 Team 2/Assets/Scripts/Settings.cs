using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// These variables persist for as long as the game is running, regardless of scene changing
/// </summary>
public class Settings : MonoBehaviour
{
    public static Settings Instance;

    public float volumeMaster;
    public float volumeSFX;
    public float volumeMusic;
    public float animationSpeed;
    public int difficulty = 1;

    // public float[] zones = { 300f, 600f, 900f }; // Enemy spawning zones (DROPPED IDEA)
    public float enemySpawnRate = 1f;

    AudioManager audioManager;

    // Start is called before the first frame update
    void Awake()
    {
        audioManager = AudioManager.manager; 

        if (Instance != null) // Remove extra instances
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    private void Start()
    {
        
    }

    public void SetAnimationSpeed(float spd) { animationSpeed = spd; }
    public void SetVolumeMaster(float vol) { volumeMaster = vol; AudioManager.manager.UpdateVolume(); }
    public void SetVolumeMusic(float vol) { volumeMusic = vol; AudioManager.manager.UpdateVolume(); }
    public void SetVolumeSFX(float vol) { volumeSFX = vol; AudioManager.manager.UpdateVolume(); }
    public void SetEnemyDifficulty(float dif) { difficulty = (int) dif; }
}
