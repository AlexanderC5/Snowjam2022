using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// These variables persist for as long as the game is running, regardless of scene changing
/// </summary>
public class Settings : MonoBehaviour
{
    public static Settings Instance;

    public float masterVolume = 1f;
    public float sfxVolume = 1f;
    public float musVolume = 1f;
    public float animationSpeed = 1.5f;
    public int difficulty = 1;

    // public float[] zones = { 300f, 600f, 900f }; // Enemy spawning zones (DROPPED IDEA)
    public float enemySpawnRate = 1f;

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
    }

    public void SetAnimationSpeed(float spd) { animationSpeed = spd; }
    public void SetMasterVolume(float vol) { masterVolume = vol; }
    public void SetMusVolume(float vol) { musVolume = vol; }
    public void SetSfxVolume(float vol) { sfxVolume = vol; }
    public void SetEnemyDifficulty(float dif) { difficulty = (int) dif; }
}
