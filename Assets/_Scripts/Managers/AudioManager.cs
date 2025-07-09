using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip menuMusic;
    public AudioClip powerUpSound;
    public AudioClip enemyHitSound;
    public AudioClip playerAttackSound;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        musicSource.clip = menuMusic;
        musicSource.Play();
    }
}
