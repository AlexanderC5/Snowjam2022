using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioTest : MonoBehaviour
{
    private AudioManager audioManager;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
       audioManager = AudioManager.manager; 
    }

    public void Click()
    {
        button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string text = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        if (text.Contains("PlayMusic"))
        {
            audioManager.PlayMusic(button.name);
        }
        else if (text.Contains("PlaySFX"))
        {
            audioManager.PlaySFX(button.name);
        }
        else if (text.Contains("PlayRandomSFX"))
        {
            audioManager.PlayRandomSFX(button.name);
        }
        else if (text.Contains("StopMusic"))
        {
            audioManager.StopMusic();
        }
        else if (text.Contains("StopSFX"))
        {
            audioManager.StopSFX();
        }
        else if (text.Contains("StopAllAudio"))
        {
            audioManager.StopAllAudio();
        }
    }
}
