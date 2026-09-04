using UnityEngine;

public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioSource pressButtonSE;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void LoadButtonPressSE()
    {
        pressButtonSE.Play();
    }
}