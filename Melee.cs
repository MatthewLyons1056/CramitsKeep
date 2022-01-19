using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    //reference obj
    public GameObject player;

    //reference scripts
    public EnemyAI enemAI;

    public GameObject hitBox;
    public GameObject tRender;
    public float tRenderStartTime;

    //reference moveTypes
    public bool canWindUp = false;

    //reference animator
    

    //enemy melee logic
    public bool canAttack = true;

    //enemy melee stats
    public float castTime;
    public float lingerTime;
    public float timeBetweenAttacks;
    public float attackRange;
    public float attackSpeed;
    public float attackMovementSpeed;
    public bool willLookAtTarget = true;

    //audio locations

    public AudioSource attackSound;
    public AudioSource windUpSound;
    private void Start()
    {
        enemAI = GetComponent<EnemyAI>();

        
        player = GameObject.FindGameObjectWithTag("Player");

        
    }
    private void Update()
    {
        AttackRangeCheck();

    }
    void AttackRangeCheck()
    {
        var distance = Vector3.Distance(this.transform.position, player.transform.position);
        //if the enemy 
        if (distance <= attackRange && canAttack == true && !enemAI.selfHealth.died && enemAI.canAttack) //attack range
        {

            StartCoroutine(WaitAndAttack(castTime, lingerTime, timeBetweenAttacks));//attack speed, cast time
            enemAI.InCombat();
        }
        if (enemAI.selfHealth.died)//if dead then stop any active attack coroutines
        {
            StopAllCoroutines();
            
        }
    }
    public IEnumerator WaitAndAttack(float castTime, float lingerTime, float timeBetweenAttacks)
    {
        canAttack = false;

        //add windup time 
        if (canWindUp)
        {
            float windUpTime = .6f;
            //play windUp animaiton
            enemAI.anim.SetBool("WindUp", true);
            //play windup sound
            windUpSound.Play(0);

            yield return new WaitForSeconds(windUpTime);
        }
        //play attack sound
        

        float originalSpeed = enemAI.aiPath.maxSpeed;
        //new yield to add proper cast time
        enemAI.aiPath.maxSpeed = attackMovementSpeed;
        enemAI.anim.SetTrigger("attack");

        
        
        if (tRender != null)
        {
            //start trailRenderCorotuine
            StartCoroutine(TrailRenderer());
            
        }
        if (canWindUp)
        {
            enemAI.anim.SetBool("WindUp", false);
        }
        //Debug.Log("Is attacking");
        enemAI.anim.SetFloat("animStabSpeed", attackSpeed); //attempt to change attackspeed     
        if (willLookAtTarget == true)
        {
            transform.LookAt(player.transform.position);
        }
        yield return new WaitForSeconds(castTime / attackSpeed); //cast time
        hitBox.SetActive(true);
        attackSound.Play(0);
        //Debug.Log("Turning on hitbox");

        //Strike
        yield return new WaitForSeconds(lingerTime / attackSpeed);//linger time
        hitBox.SetActive(false);
        if (tRender != null)
        {
            tRender.SetActive(false);
        }
        enemAI.aiPath.maxSpeed = originalSpeed;
       //pull back-return to idle/null

       yield return new WaitForSeconds(timeBetweenAttacks / attackSpeed);//linger time

        //ready to attack again
        canAttack = true;
        //turn on hitbox
        

    }

    public IEnumerator TrailRenderer()
    {

        yield return new WaitForSeconds(tRenderStartTime);
        //turn off renderer
        tRender.SetActive(true);
    }

    

}
