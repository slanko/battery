using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*TODO
    - expand alerted behaviour
    - iconography (!, ?)
    - vocalizations - figure out cool sounds
    - HEARING!!!
    - expand vision cone while suspicious/alerted
*/

public class EnemyScript : MonoBehaviour
{
    NavMeshAgent nav;
    AudioSource aud;
    [SerializeField] int health;
    [SerializeField, Header("Sight Stuff")]
    float visionConeAngle;
    [SerializeField] float widenedVisionConeAngle, lookoutTicRate, visionDistance, alertDistance, handLerpSpeed, bodyLerpSpeed;
    [SerializeField] Transform playerChar, currentTarget, handVisual, handRotationGhost, enemyVisual, playerLocator, forwardLocator;
    [SerializeField] GameObject myBattery;
    Rigidbody rb;

    [SerializeField, Header("Shooting Stuff")] Transform bulletFirePoint;
    bool isShooting;
    [SerializeField] GameObject impactFX, impactSparkFX, bulletTrail, pingedShell, droppedGun;
    [SerializeField] Transform shellPingPoint, myGun;
    [SerializeField] float ammoCount, ammoCountMax, recoilIntensity, recoilAdd, recoilIntensityMax, recoilCooldownSpeed;
    [SerializeField] ParticleSystem muzzleFlash;

    [SerializeField, Header("Behavioural Stuff")] float suspicionTime;
    //suspiciouscheckiterations is how many times they get a new random point in a sphere to go to
    [SerializeField] float suspicousCheckIterations;
    Vector3 pointOfInterest;
    bool reachedPointOfInterest = false, begunExitFromSuspicious;
    [SerializeField] enemyState currentState;

    [SerializeField, Tooltip("When I am this far away from the next patrol point I will get a new point."), Header("Patrol Stuff")]
    float nextPointDist;
    [SerializeField] int currentPoint = 0;
    [SerializeField] Transform[] patrolPointList;

    [Header("FX Stuff")] public GameObject myBlood;



    //animator stuff
    Animator anim;
    public enum patrolType
    {
        PATROL,
        IDLE,
        RANDOMMOVE
    }
    public enum enemyState
    {
        IDLE,
        SUSPICIOUS,
        ALERTED,
        DEAD
    }
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        if (patrolPointList.Length > 0) nav.SetDestination(patrolPointList[currentPoint].position);
        anim = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        playerChar = GameObject.Find("Player").GetComponent<Transform>();
        playerLocator = GameObject.Find("PlayerLocationWithLerpSphere").GetComponent<Transform>();
        StartCoroutine("keepLookout");
        //CHANGE LATER MAYBE?? ONCE I FIGURE OUT WATH TO HE'LL I AM DOING
        currentTarget = playerChar;
    }
    void Update()
    {
        if (currentState != enemyState.DEAD)
        {
            if (currentState == enemyState.IDLE) idleUpdate();
            if (currentState == enemyState.SUSPICIOUS) suspiciousUpdate();
            if (currentState == enemyState.ALERTED) alertedUpdate();
        }
        animatorUpdate();
        weaponHandlingUpdate();
    }

    void weaponHandlingUpdate()
    {
        if (recoilIntensity > 0) recoilIntensity -= recoilCooldownSpeed * Time.deltaTime;
        if (recoilIntensity > recoilIntensityMax) recoilIntensity = recoilIntensityMax;
        if (recoilIntensity < 0) recoilIntensity = 0;
    }

    void animatorUpdate()
    {
        anim.SetFloat("Walkey", rb.velocity.magnitude);
        anim.SetBool("IDLE", currentState == enemyState.IDLE);
        anim.SetBool("SUSPICIOUS", currentState == enemyState.SUSPICIOUS);
        anim.SetBool("ALERTED", currentState == enemyState.ALERTED);
    }

    void idleUpdate()
    {
        if (nav.remainingDistance < nextPointDist && patrolPointList.Length > 0) arrivedAtNextPoint();
        looksyRoundyStuff(forwardLocator.position);
    }

    void suspiciousUpdate()
    {
        if(nav.remainingDistance > .5f && reachedPointOfInterest == false)
        {
            nav.destination = pointOfInterest;
            looksyRoundyStuff(pointOfInterest);
        }
        else
        {
            reachedPointOfInterest = true;
            Debug.Log(gameObject.name + " has reached point of interest!!");
            if (!begunExitFromSuspicious)
            {
                begunExitFromSuspicious = true;
                StartCoroutine(suspiciousLookAround());
            }
            looksyRoundyStuff(forwardLocator.position);
        }
    }

    IEnumerator suspiciousLookAround()
    {
        Vector3 whereIWas = transform.position;
        for(int i = 0; i < suspicousCheckIterations; i++)
        {
            nav.destination = transform.position + Random.insideUnitSphere * 5;
            yield return new WaitForSeconds(suspicionTime);
        }
        if (currentState == enemyState.SUSPICIOUS) transitionState(enemyState.IDLE);
        nav.destination = whereIWas;
    }

    void alertedUpdate()
    {
        if (currentTarget != null)
        {
            looksyRoundyStuff(currentTarget.position);
            float moveDist = Vector3.Distance(transform.position, playerChar.position);
            if (moveDist > 5f) nav.destination = playerChar.position;
            else nav.destination = transform.position;
            if (!isShooting) StartCoroutine("shootGunCoroutine");
        }
    }

    void looksyRoundyStuff(Vector3 target)
    {
        //what does this do?

        //set hand rotation ghost to look at current target. Nice!
        handRotationGhost.LookAt(target);
        //slerps hand visual rotation to that. Easy!
        handVisual.rotation = Quaternion.Slerp(handVisual.rotation, handRotationGhost.rotation, handLerpSpeed * Time.deltaTime);
        //gets hand rotation but only the Y value, stores it as a local variable. Okay!
        Vector3 handRotationYOnly = new Vector3(transform.rotation.eulerAngles.x, handVisual.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        //slerps the visual rotation to the Y rotation only variable we stored earlier. Sweet!
        enemyVisual.rotation = Quaternion.Slerp(enemyVisual.rotation, Quaternion.Euler(handRotationYOnly), bodyLerpSpeed * Time.deltaTime);
    }

    void arrivedAtNextPoint()
    {
        currentPoint++;
        if (currentPoint > patrolPointList.Length) currentPoint = 0;
        nav.SetDestination(patrolPointList[currentPoint].position);
    }

    IEnumerator keepLookout()
    {
        float coneAngle;
        if (currentState == enemyState.SUSPICIOUS || currentState == enemyState.ALERTED) coneAngle = widenedVisionConeAngle;
        else coneAngle = visionConeAngle;
        //run stuff if it needs to but only if it needs to
        if (currentState == enemyState.IDLE || currentState == enemyState.SUSPICIOUS || currentState == enemyState.ALERTED)
        {
            //make variables (RELEGATE THIS TO A COROUTINE THAT RUNS LIKE EVERY SECOND. IT'S BETTER THIS WAY. OR SOMETHING)
            Vector3 playerDir = playerChar.position - transform.position;
            float visAngle = Vector3.Angle(playerDir, enemyVisual.forward);
            float visDist = Vector3.Distance(transform.position, playerChar.position);
            //use variables :)
            switch (currentState)
            {
                case enemyState.IDLE:
                    if (visAngle < coneAngle && visDist < visionDistance && checkLineOfSight())
                    {
                        transitionState(enemyState.SUSPICIOUS);
                        Debug.Log(gameObject.name + " is now " + currentState);
                    }
                    break;
                case enemyState.SUSPICIOUS:
                    if (visAngle < coneAngle && visDist < alertDistance && checkLineOfSight())
                    {
                        transitionState(enemyState.ALERTED);
                        Debug.Log(gameObject.name + " is now " + currentState);
                    }
                    break;
                case enemyState.ALERTED:
                    if (visAngle < coneAngle && visDist > visionDistance && checkLineOfSight() ||
                        visAngle > coneAngle && visDist < visionDistance && checkLineOfSight() ||
                        visAngle < coneAngle && visDist < visionDistance && !checkLineOfSight())
                    {
                        transitionState(enemyState.SUSPICIOUS);
                        Debug.Log(gameObject.name + " is now " + currentState);
                    }
                    break;
            }
            debugVisionLines(coneAngle);
        }
        yield return new WaitForSeconds(lookoutTicRate);
        StartCoroutine(keepLookout());
    }

    bool checkLineOfSight()
    {
        bool canSeePlayer = false;
        RaycastHit hit;
        if (Physics.Linecast(transform.position, playerChar.position, out hit))
        {
            if (hit.transform.gameObject.tag == "Player") canSeePlayer = true;
        }
        return canSeePlayer;
    }

    IEnumerator shootGunCoroutine()
    {
        isShooting = true;
        yield return new WaitForSeconds(Random.Range(.1f, .5f));
        if (currentState == enemyState.ALERTED && ammoCount > 0) anim.SetTrigger("Fire");
        isShooting = false;
    }

    public void fireGun() //shamelessly lifted from playerscript hee hee
    {
        ammoCount--;
        Quaternion recoilVector = new Quaternion();
        recoilVector.eulerAngles = new Vector3(handVisual.rotation.eulerAngles.x + Random.Range(-recoilIntensity * 1.5f, 0), handVisual.rotation.eulerAngles.y + Random.Range(-recoilIntensity, recoilIntensity) * 0.75f, handVisual.rotation.eulerAngles.z); //fuck this
        handVisual.rotation = recoilVector;
        recoilIntensity += recoilAdd;
        muzzleFlash.Play();
        fireABullet(bulletFirePoint.position, bulletFirePoint.forward, false);

    }
    void fireABullet(Vector3 pos, Vector3 dir, bool reflected)
    {
        RaycastHit bulletImpact;
        if (Physics.Raycast(pos, dir, out bulletImpact, Mathf.Infinity))
            createNewBulletLine(pos, bulletImpact.point, true);
        else
            createNewBulletLine(pos, Vector3.zero, false);
        bool foundType = false;
        switch (bulletImpact.transform.gameObject.tag)
        {
            case "Reflective":
                foundType = true;
                if (reflected == false)
                {
                    fireABullet(bulletImpact.point, Vector3.Reflect(bulletFirePoint.forward, bulletImpact.normal), true);
                }
                Quaternion impactSparksDir = new Quaternion();
                impactSparksDir.eulerAngles = Vector3.Reflect(bulletFirePoint.forward, bulletImpact.normal);
                Instantiate(impactSparkFX, bulletImpact.point, impactSparksDir);
                break;
            case "Mook":
                foundType = true;
                EnemyScript enemy = bulletImpact.transform.gameObject.GetComponent<EnemyScript>();
                enemy.takeDamage(1);
                Instantiate(enemy.myBlood, bulletImpact.point, bulletFirePoint.rotation);
                break;
            case "Player":
                foundType = true;
                PlayerScript player = bulletImpact.transform.gameObject.GetComponent<PlayerScript>();
                player.takeDamage();
                Instantiate(player.myBlood, bulletImpact.point, bulletFirePoint.rotation);
                break;
        }
        if (foundType == false) Instantiate(impactFX, bulletImpact.point, bulletFirePoint.rotation);
    }

    void createNewBulletLine(Vector3 startPoint, Vector3 endPoint, bool hitSomething)
    {
        GameObject bulletLine = Instantiate(bulletTrail, Vector3.zero, new Quaternion(0, 0, 0, 0));
        BulletTrailScript trailScript = bulletLine.GetComponent<BulletTrailScript>();
        trailScript.point0Point = startPoint;
        if (hitSomething) trailScript.point1Point = endPoint;
        else trailScript.point1Point = new Vector3(startPoint.x, startPoint.y, startPoint.z * 1000f);
    }

    public void pingShell()
    {
        Instantiate(pingedShell, shellPingPoint.position, shellPingPoint.rotation);
    }

    void dropGun()
    {
        Instantiate(droppedGun, myGun.position, myGun.rotation);
    }

    public void playSound(AudioClip audio)
    {
        aud.PlayOneShot(audio);
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        switch (currentState)
        {
            case enemyState.IDLE:
                transitionState(enemyState.SUSPICIOUS);
                break;
            case enemyState.SUSPICIOUS:
                transitionState(enemyState.ALERTED);
                break;
        }
        if (health == 0)
        {
            transitionState(enemyState.DEAD);
        }
        else anim.SetTrigger("Hurt");
    }

    void debugVisionLines(float angle) // lol angle gets passed on twice. cringe behaviour
    {
        switch (currentState)
        {
            case enemyState.IDLE:
                displayViewCone(angle, Color.green, .1f);
                break;
            case enemyState.SUSPICIOUS:
                displayViewCone(angle, Color.yellow, .1f);
                break;
            case enemyState.ALERTED:
                displayViewCone(angle, Color.red, .1f);
                break;
        }

    }

    void displayViewCone(float angle, Color colour, float duration)
    {
        Debug.DrawRay(transform.position, enemyVisual.forward * visionDistance, colour, duration);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(angle / 2, Vector3.up) * enemyVisual.forward * visionDistance, colour, duration);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-angle / 2, Vector3.up) * enemyVisual.forward * visionDistance, colour, duration);
    }

    //USE THIS INSTEAD OF HARD TRANSITIONING INTO STATES. THIS LETS YOU CONSISTENTLY DO COOL SHIT WHEN YOU TRANSITION
    //if i want to elaborate on this further i could have a second switch statement within each case with a stateFrom, so i can do like, a whew sound when going from alerted to idle but a ...huh sound from suspicious to idle
    //lots of work tho
    void transitionState(enemyState stateTo)
    {
        currentState = stateTo;
        switch (stateTo)
        {
            case enemyState.IDLE:


                break;
            case enemyState.SUSPICIOUS:
                pointOfInterest = playerLocator.position;
                nav.destination = pointOfInterest;
                reachedPointOfInterest = false;
                break;
            case enemyState.ALERTED:


                break;
            case enemyState.DEAD:
                nav.isStopped = true;
                anim.SetBool("Alive", false);
                enemyVisual.LookAt(playerChar);
                dropGun();
                break;
        }
    }
}
