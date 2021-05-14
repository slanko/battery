using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float upForce, spinForce, collisionThreshold;
    [SerializeField] AudioClip[] shellSounds;
    AudioSource aud;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
        rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
        rb.AddTorque(new Vector3(spinForce, 0, 0), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if(collision.relativeVelocity.magnitude > collisionThreshold)
        {
            aud.volume = collision.relativeVelocity.magnitude / 5;
            aud.PlayOneShot(shellSounds[Random.Range(0, shellSounds.Length - 1)]);
        }
    }
}
