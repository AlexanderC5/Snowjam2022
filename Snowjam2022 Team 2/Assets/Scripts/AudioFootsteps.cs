using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFootsteps : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.manager;
    }

    public void PlayRandomFootsteps()
    {
        audioManager.PlayRandomSFX("Footsteps");
    }
}
