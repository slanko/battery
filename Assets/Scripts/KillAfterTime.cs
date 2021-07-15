using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KillAfterTime : MonoBehaviour
{
    [SerializeField] float time;
    void Start()
    {
        Destroy(gameObject, time);
    }
}
