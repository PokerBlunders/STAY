using System.Xml.Serialization;
using UnityEngine;

public class PlaySounds : MonoBehaviour
{
    public string actionName; // Type "Sit", "Down", "Left", or "Right"
    public float volume = 0.5f;
    
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.PlaySFX(actionName);
        AudioManager.Instance.SFXVolume(volume);
    }
}
