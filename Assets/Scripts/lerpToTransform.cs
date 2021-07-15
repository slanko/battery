using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lerpToTransform : MonoBehaviour
{
    [SerializeField] float lerpSpeed;
    [SerializeField] Transform target;
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, lerpSpeed * Time.deltaTime);
    }
}
