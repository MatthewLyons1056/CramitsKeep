using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : MonoBehaviour
{
    //reference obj
    public GameObject player;
    public GameObject bulletSpawn, enemyBullet;
    
    //reference scripts
    public EnemyAI enemAI;

    public float FireRate;
    public float castTime, lingerTime, timeBetweenAttacks;
    public float attackRange;
    public string attackAnimation; //assign name in the inspector to play animation
    public float accuracy;
    public float attackMovementSpeed;

    public bool willLookAtTarget = true;//used for snap aiming, used on wizards
    public bool trackPlayer;//used to cause enemies to rotate toward the player instead of snap aim, used for archers
    private Transform targetRotater;
    public float rotationSpeed;
    private float modifiedRotationSpeed;

    public bool canVolley;
    public float volleyAmount;
    public float shotTimeBetweenVolley;
    private bool canAttack = true;

    public bool useBulletTracker;//if enabled the enemy will move bullet spawn but not self, used on elementals
    
    
    public void Start()
    {
        //Reference EnemyAI Script
        enemAI = GetComponent<EnemyAI>();

        modifiedRotationSpeed = rotationSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
        targetRotater = player.transform;
    }
    public void Update()
    {
        AttackRangeCheck();
    }
    public void ShootAtPlayer()
    {
        if (enemAI.canMove)
        {
            
            //look at player
            if (willLookAtTarget == true)
            {
                transform.LookAt(player.transform.position);
            }
            StartCoroutine(WaitAndAttack(castTime, lingerTime, timeBetweenAttacks));
            

        }
    }
    public void Shoot()
    {
        //look at player
        if (willLookAtTarget == true)
        {
            transform.LookAt(player.transform.position);
        }
        if(useBulletTracker)
        {
            bulletSpawn.transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y + 2.1f, player.transform.position.z));
        }
        float accuracyRandom = Random.Range(-accuracy, accuracy);
        Vector3 accuracyAngle = new Vector3(0, accuracyRandom, 0);
      
        GameObject bullets = Instantiate(enemyBullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation * Quaternion.Euler(accuracyAngle), gameObject.transform);
        Destroy(bullets, 5f);
       
    }

    void AttackRangeCheck()
    {

        var distance = Vector3.Distance(transform.position, player.transform.position);
        //if the enemy 
        if (trackPlayer)
        {
            Vector3 targetDirection = targetRotater.position - transform.position;
            targetDirection = new Vector3(targetDirection.x, 0, targetDirection.z);
            float singleStep = modifiedRotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        if (distance <= attackRange && !enemAI.selfHealth.died && canAttack && enemAI.canAttack)
        {
            //play animation
            
            ShootAtPlayer();
            enemAI.InCombat();
        }
        if(enemAI.selfHealth.died)
        {
            StopAllCoroutines();
        }
    }

    public IEnumerator WaitAndAttack(float castTime, float lingerTime, float timeBetweenAttacks)
    {
        float originalSpeed = enemAI.aiPath.maxSpeed;
        canAttack = false;
        enemAI.aiPath.maxSpeed = attackMovementSpeed;
        if (enemAI.isAnimated)
        {
            enemAI.anim.SetTrigger("attack");
        }
        modifiedRotationSpeed = rotationSpeed / 5;
        
        yield return new WaitForSeconds(castTime / FireRate); //cast time
        if (canVolley)
        {
            float maxShots = volleyAmount;
            for (int i = 0; i < maxShots; i++) 
            {
                if (enemAI.canAttack)
                {
                    yield return new WaitForSeconds(shotTimeBetweenVolley);
                    Shoot();
                    if (enemAI.isAnimated)
                    {
                        enemAI.anim.SetTrigger("attack");
                    }

                }

            }
        }
        if (!canVolley)
        {
            Shoot();
        }
        modifiedRotationSpeed = rotationSpeed;

        //Strike
        yield return new WaitForSeconds(lingerTime / FireRate);//linger time
        enemAI.aiPath.maxSpeed = originalSpeed;
        canAttack = true;
        //pull back-return to idle/null

        //yield return new WaitForSeconds(timeBetweenAttacks / FireRate); //time between attacks

        //ready to attack again

        //turn on hitbox


    }
    public void StopRotation()
    {
        rotationSpeed = 0;
        modifiedRotationSpeed = 0;
    }
    

}
