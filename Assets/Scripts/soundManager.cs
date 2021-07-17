using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    public float sphereSize, destroyTime;
    // Start is called before the first frame update
    public void setVariables(float size, float destroyTime)
    {
        transform.localScale = new Vector3(size, size, size);
        if (destroyTime != 0) StartCoroutine(killMeAfterTime());
    }

    IEnumerator killMeAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
