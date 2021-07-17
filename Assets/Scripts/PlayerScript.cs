using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*TODO
    - battery mechanics (dash, shield, zap)
    - battery collection
    - wall plug
    - overcharge benefits
    - death, reserve state
*/
public class PlayerScript : MonoBehaviour
{
    [SerializeField] float playerLerpSpeed, handLerpSpeed, camLerpSpeed, moveSpeed, AmmoCount, AmmoMax, recoilIntensity, recoilAdd, recoilIntensityMax, recoilCooldownSpeed;
    [SerializeField] Transform crosshair, crosshairlet, camBuddyPos, playerVisual, handVisual, handRotationGhost;
    [SerializeField] Camera myCam;
    Rigidbody rb;
    Animator anim;
    Animator crosshairAnim;
    AudioSource aud;
    shootingScript shoot;
    Vector3 aimPointNormal; //used to get normal of point aimed at, decides whether to aim at the crosshair or the crosshairlet

    [SerializeField, Header("User Interface")] Text ammoCounter;
    [SerializeField] GameObject silencedText, laserSightText, overchargeText;
    [SerializeField] Slider ammoSlider, overchargeSlider;

    [Header("Charge Handling Stuff"), SerializeField] float chargeDecay;
    [SerializeField] float overchargeDecay, leftChargeAmount, leftChargeMax, rightChargeAmount, rightChargeMax, overchargeAmount, overchargeMax;
    [SerializeField] ParticleSystem overchargeFX;
    [SerializeField] Slider leftChargeSlider, rightChargeSlider;

    [SerializeField, Header("Sound Stuff")] GameObject soundSphere;

    [SerializeField, Header("Gubbins")] GameObject spentClip;
    [SerializeField] GameObject unspentClip, pingedShell, bulletTrail, impactFX, impactSparkFX, silencer, droppedGun;
    [SerializeField] Transform clipDropPoint, shellPingPoint, bulletFirePoint;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] AudioClip unsilencedShot, silencedShot;
    [SerializeField] Light muzzleFlashLight;
    [SerializeField] LineRenderer laserSight;
    [SerializeField] Transform laserSightStartLocation;
    [SerializeField] AudioClip gunClick;
    public GameObject myBlood;

    [SerializeField, Header("Emotional Gubbins :(")] float currentMood;
    [SerializeField, ColorUsage(true, true)] Color baseColour, angryColour;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        crosshairAnim = crosshair.gameObject.GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        ammoSlider.value = AmmoCount;
        shoot = GetComponent<shootingScript>();
    }

    // Update is called once per frame
    void Update()
    {
        crosshairUpdate();
        playerFacingUpdate();
        weaponHandlingUpdate();
        chargeHandlingUpdate();
        miscUpdate();
    }

    private void FixedUpdate()
    {
        movementUpdate();
    }

    void crosshairUpdate()
    {
        var ray = myCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            crosshair.transform.position = hit.point;
            aimPointNormal = hit.normal;
        }

        if (laserSight.enabled)
        {
            laserSight.SetPosition(0, laserSightStartLocation.position);
            RaycastHit laserPointHit;
            if (Physics.Raycast(laserSightStartLocation.position, laserSightStartLocation.forward, out laserPointHit, Mathf.Infinity)) laserSight.SetPosition(1, laserPointHit.point);
        }
        crosshairAnim.SetBool("Up", aimPointNormal == Vector3.up);
    }

    void playerFacingUpdate ()
    {
        if (aimPointNormal == Vector3.up) handRotationGhost.LookAt(crosshairlet.position);
        else handRotationGhost.LookAt(crosshair.position);
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

    public void takeDamage()
    {
        Debug.Log("ow!!");
    }
    void weaponHandlingUpdate()
    {
        anim.SetBool("Fire", Input.GetMouseButton(0));
        anim.SetBool("NoAmmo", AmmoCount <= 0);
        if (Input.GetKeyDown(KeyCode.R)) anim.SetTrigger("Reload");
        if (Input.GetKeyDown(KeyCode.T)) anim.SetTrigger("ToggleSilencer");
        if (Input.GetKeyDown(KeyCode.Y)) toggleLaserSight();
        ammoCounter.text = AmmoCount.ToString();
        if (overchargeAmount > 0)
        {
            float overchargeDiv = overchargeAmount / (overchargeAmount * 0.5f);
            anim.SetFloat("Firerate", 1 + overchargeDiv);
            anim.SetFloat("ReloadSpeed", 1 + overchargeDiv);
        }
        else
        {
            anim.SetFloat("Firerate", 1);
            anim.SetFloat("ReloadSpeed", 1);
        }
        if (recoilIntensity > 0) recoilIntensity -= recoilCooldownSpeed * Time.deltaTime;
        if (recoilIntensity > recoilIntensityMax) recoilIntensity = recoilIntensityMax;
        if (recoilIntensity < 0) recoilIntensity = 0;
    }

    void miscUpdate()
    {
        anim.SetFloat("Emotion", currentMood);
    }

    void chargeHandlingUpdate()
    {
        overchargeText.gameObject.SetActive(overchargeAmount > 0);
        anim.SetFloat("Overcharge", overchargeAmount);
        if (overchargeAmount > overchargeMax) overchargeAmount = overchargeMax;
        if (overchargeAmount > 0) overchargeAmount -= overchargeDecay * Time.deltaTime;
        overchargeSlider.value = Mathf.CeilToInt(overchargeAmount);
        if (Input.GetKeyDown(KeyCode.O)) getCharge(12);
        if(overchargeAmount <= 0)
        {
            if (leftChargeAmount > 0) leftChargeAmount -= chargeDecay * Time.deltaTime;
            if (rightChargeAmount > 0) rightChargeAmount -= chargeDecay * Time.deltaTime;
        }
        leftChargeSlider.value = leftChargeAmount;
        rightChargeSlider.value = rightChargeAmount;
    }

    public void fireGun()
    {
        AmmoCount--;
        ammoSlider.value = AmmoCount;
        Quaternion recoilVector = new Quaternion();
        recoilVector.eulerAngles = new Vector3(handVisual.rotation.eulerAngles.x + Random.Range(-recoilIntensity * 1.5f, 0), handVisual.rotation.eulerAngles.y + Random.Range(-recoilIntensity, recoilIntensity) * 0.75f, handVisual.rotation.eulerAngles.z); //fuck this
        handVisual.rotation = recoilVector;
        recoilIntensity += recoilAdd;
        if (!silencer.activeSelf)
        {
            muzzleFlash.Play();
            muzzleFlashLight.range = 10;
            shoot.fireABullet(bulletFirePoint.position, bulletFirePoint.forward, false);
            makeSoundSphere(100f, .1f, bulletFirePoint.position);
        }
        else
        {
            muzzleFlashLight.range = 0;
            shoot.fireABullet(bulletFirePoint.position, bulletFirePoint.forward, false);
            makeSoundSphere(10f, .1f, bulletFirePoint.position);
        }
    }
    public void refillAmmo()
    {
        if (AmmoCount >= 1) AmmoCount = AmmoMax + 1;
        else AmmoCount = AmmoMax;
        ammoSlider.value = AmmoCount;
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

    void makeSoundSphere(float size, float destroyTime, Vector3 pos)
    {
        soundManager sound = Instantiate(soundSphere, pos, Quaternion.identity).GetComponent<soundManager>();
        sound.setVariables(size, destroyTime);
    }

    public void playGunShotSpecifically()
    {
        if (silencer.activeSelf) aud.PlayOneShot(silencedShot);
        else aud.PlayOneShot(unsilencedShot);
    }

    public void toggleSilencer()
    {
        if (silencer.activeSelf)
        {
            silencer.SetActive(false);
            silencedText.SetActive(false);
        }
        else
        {
            silencer.gameObject.SetActive(true);
            silencedText.SetActive(true);
        }
    }

    public void toggleLaserSight()
    {
        if (laserSight.enabled)
        {
            laserSight.enabled = false;
            laserSightText.SetActive(false);
        }
        else
        {
            laserSight.enabled = true;
            laserSightText.SetActive(true);
        }
        aud.PlayOneShot(gunClick);
    }

    public void getCharge(float amount)
    {
        if (leftChargeAmount < 0) leftChargeAmount = 0;
        if (rightChargeAmount < 0) rightChargeAmount = 0;
        leftChargeAmount += amount / 2;
        rightChargeAmount += amount / 2;
        float addToOvercharge = (leftChargeAmount + rightChargeAmount) - (leftChargeMax + rightChargeMax);
        if(addToOvercharge > 0) overchargeAmount += addToOvercharge;
        if (leftChargeAmount > leftChargeMax) leftChargeAmount = leftChargeMax;
        if (rightChargeAmount > rightChargeMax) rightChargeAmount = rightChargeMax;
        if (overchargeAmount > 0) overchargeFX.Play();
    }
}