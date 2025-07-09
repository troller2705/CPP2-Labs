using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    public ParticleSystem collectEffect;
    public ParticleSystem deathEffect;

    public void PlayCollectEffect()
    {
        Instantiate(collectEffect, transform.position, Quaternion.identity);
    }

    public void PlayDeathEffect()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
    }
}
