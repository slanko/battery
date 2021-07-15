using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float collisionThreshold;
    [SerializeField] Vector3 upForce, spinForce;
    [SerializeField] float randomMin, randomMax;
    [SerializeField] AudioClip[] shellSounds;
    float volMult;
    AudioSource aud;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
        volMult = aud.volume;
        rb.AddForce(upForce, ForceMode.Impulse);
        rb.AddTorque(spinForce * Random.Range(randomMin, randomMax), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > collisionThreshold)
        {
            aud.volume = collision.relativeVelocity.magnitude / 5 * volMult;
            aud.PlayOneShot(shellSounds[Random.Range(0, shellSounds.Length - 1)]);
        }
    }
}
