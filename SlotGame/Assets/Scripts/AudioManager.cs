using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _spinSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    

    public IEnumerator PlaySpinSound()
    {
        _audioSource.PlayOneShot(_spinSound);
        yield return new WaitForSeconds(1.8f);
        _audioSource.Stop();
    }
}
