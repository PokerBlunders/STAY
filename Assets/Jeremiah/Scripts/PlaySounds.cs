using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlaySounds : MonoBehaviour
{
    public string actionName;
    public float volume = 0.5f;


    private void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.PlaySFX(actionName);
        AudioManager.Instance.SFXVolume(volume);
    }

    public void PlaySFXByName(string sfxName)
    {
        AudioManager.Instance.SFXVolume(volume);
        AudioManager.Instance.PlaySFX(sfxName);
    }

}
