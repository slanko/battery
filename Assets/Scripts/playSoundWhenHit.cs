using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSoundWhenHit : MonoBehaviour
{
    AudioSource aud;
    [SerializeField] float collisionThreshold;
    float volMult;
    [SerializeField] AudioClip[] sfx;
    // Start is called before the first frame update
    void Start()
    {
        aud = GetComponent<AudioSource>();
        volMult = aud.volume;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > collisionThreshold)
        {
            aud.volume = collision.relativeVelocity.magnitude / 5 * volMult;
            aud.PlayOneShot(sfx[Random.Range(0, sfx.Length)]);
        }
    }
}
