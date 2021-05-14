using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float playerLerpSpeed, handLerpSpeed, camLerpSpeed, moveSpeed, AmmoCount, AmmoMax;
    [SerializeField] Transform crosshair, camBuddyPos, playerVisual, handVisual, handRotationGhost;
    [SerializeField] Camera myCam;
    Rigidbody rb;
    Animator anim;
    AudioSource aud;

    [SerializeField, Header("User Interface")] Text ammoCounter;

    [SerializeField, Header("Gubbins")] GameObject spentClip;
    [SerializeField] GameObject unspentClip, pingedShell;
    [SerializeField] Transform clipDropPoint, shellPingPoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        crosshairUpdate();
        playerFacingUpdate();
        movementUpdate();
        weaponHandlingUpdate();
    }

    void crosshairUpdate()
    {
        var ray = myCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            crosshair.transform.position = hit.point;
        }
    }

    void playerFacingUpdate ()
    {
        handRotationGhost.LookAt(crosshair.position);
        handVisual.rotation = Quaternion.Slerp(handVisual.rotation, handRotationGhost.rotation, handLerpSpeed * Time.deltaTime);
        Vector3 handRotationYOnly = new Vector3(transform.rotation.eulerAngles.x, handVisual.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        playerVisual.rotation = Quaternion.Slerp(playerVisual.rotation, Quaternion.Euler(handRotationYOnly), playerLerpSpeed * Time.deltaTime);
        camBuddyPos.position = Vector3.Lerp(camBuddyPos.position, transform.position, camLerpSpeed * Time.deltaTime);
    }

    void movementUpdate()
    {
        float vert = Input.GetAxis("Vertical");
        float horiz = Input.GetAxis("Horizontal");

        var locVel = transform.InverseTransformDirection(rb.velocity);
        locVel.z = vert * moveSpeed;
        locVel.x = horiz * moveSpeed;
        rb.velocity = transform.TransformDirection(locVel);
        anim.SetFloat("Walkey", rb.velocity.magnitude);
    }

    void weaponHandlingUpdate()
    {
        anim.SetBool("Fire", Input.GetMouseButton(0));
        anim.SetBool("NoAmmo", AmmoCount <= 0);
        if (Input.GetKeyDown(KeyCode.R)) anim.SetTrigger("Reload");
        ammoCounter.text = AmmoCount.ToString();
    }

    public void DecreaseAmmoCount()
    {
        AmmoCount--;
    }

    public void refillAmmo()
    {
        AmmoCount = AmmoMax;
    }

    public void dropClip()
    {
        if (AmmoCount > 1) Instantiate(unspentClip, clipDropPoint.position, clipDropPoint.rotation);
        else Instantiate(spentClip, clipDropPoint.position, clipDropPoint.rotation);
    }

    public void pingShell()
    {
        Instantiate(pingedShell, shellPingPoint.position, shellPingPoint.rotation);
    }

    public void playSound(AudioClip audio)
    {
        aud.PlayOneShot(audio);
    }
}
