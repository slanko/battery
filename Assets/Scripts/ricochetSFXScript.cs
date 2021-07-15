using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ricochetSFXScript : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] ricochetSFX;
    // Start is called before the first frame update
    void Start()
    {
        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.volume = Random.Range(.4f, .55f);
        aud.PlayOneShot(ricochetSFX[Random.Range(0, ricochetSFX.Length)]);
    }
}
