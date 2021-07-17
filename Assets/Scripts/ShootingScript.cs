using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingScript : MonoBehaviour
{
    [SerializeField] GameObject impactFX, impactSparkFX, bulletTrail;
    public void fireABullet(Vector3 pos, Vector3 dir, bool reflected)
    {
        RaycastHit bulletImpact;
        Quaternion quatFromDir = Quaternion.LookRotation(dir, Vector3.up);
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
                    fireABullet(bulletImpact.point, Vector3.Reflect(dir, bulletImpact.normal), true);
                }
                Quaternion impactSparksDir = new Quaternion();
                impactSparksDir.eulerAngles = Vector3.Reflect(dir, bulletImpact.normal);
                Instantiate(impactSparkFX, bulletImpact.point, impactSparksDir);
                break;
            case "Mook":
                foundType = true;
                EnemyScript enemy = bulletImpact.transform.gameObject.GetComponent<EnemyScript>();
                enemy.takeDamage(1);
                Instantiate(enemy.myBlood, bulletImpact.point, quatFromDir);
                break;
            case "Player":
                foundType = true;
                PlayerScript player = bulletImpact.transform.gameObject.GetComponent<PlayerScript>();
                player.takeDamage();
                Instantiate(player.myBlood, bulletImpact.point, quatFromDir);
                break;
        }
        if (foundType == false) Instantiate(impactFX, bulletImpact.point, quatFromDir);
    }

    void createNewBulletLine(Vector3 startPoint, Vector3 endPoint, bool hitSomething)
    {
        GameObject bulletLine = Instantiate(bulletTrail, Vector3.zero, new Quaternion(0, 0, 0, 0));
        BulletTrailScript trailScript = bulletLine.GetComponent<BulletTrailScript>();
        trailScript.point0Point = startPoint;
        if (hitSomething) trailScript.point1Point = endPoint;
        else trailScript.point1Point = new Vector3(startPoint.x, startPoint.y, startPoint.z * 1000f);
    }
}