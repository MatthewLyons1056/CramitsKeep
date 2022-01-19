using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//



public class EnemyAI : DebuffManager
{
    //Reference scripts
    [HideInInspector]
    public Pathfinding.AIDestinationSetter aiDestination;
    [HideInInspector]
    public Pathfinding.AIPath aiPath;


    //reference animation condition
    public bool isAnimated = true; //set this value to false for enemies with no animation
    public Collider selfCollider;
    public Rigidbody selfRB;
    public GameObject spawnFX;
    public GameObject[] pCubeCollection;//pCube is the gameobject that holds the enemies body visual and weapon visual
    //data for movement
    public float enemySpeed;
    public bool canMove = true;

    //data for projectile targeting
    public GameObject player;

    //animator
    public Animator anim;

    //combat data

    public float DMGVal;
    public bool inCombat;
    public bool canAttack;
    //spawn cost is the primary way enemies are created, spawn cost is calculated by the health and DPS of the enemy excluding multi projectile attacks
    public int spawnCost;
    public float scalingFactor = 1;

    //enemy script data
    public bool isBoss;
    public EnemyHealth selfHealth;
    public GameStateController gameStateScript;
    // Start is called before the first frame update
    void Start()
    {
        //reference enemy types
        if (!isBoss)
        {
            GameObject spawnOBJ = Instantiate(spawnFX, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(spawnOBJ, 1.25f);
            canAttack = false;
            anim = GetComponentInChildren<Animator>();
            selfCollider = GetComponent<Collider>();
            selfRB = GetComponent<Rigidbody>();
            //reference ai pathfinding
            aiPath = GetComponent<Pathfinding.AIPath>();
            aiPath.maxSpeed = enemySpeed;
            aiDestination = GetComponent<Pathfinding.AIDestinationSetter>();
            GameObject mainSceneManager = GameObject.FindGameObjectWithTag("MainSceneController");
            gameStateScript = mainSceneManager.GetComponentInChildren<GameStateController>();
            DMGVal *= gameStateScript.enemyDamageBonus * scalingFactor;
            StartCoroutine(EnableAttacking());
        }
        //reference player
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if(canAttack)
        {
            AgroRangeCheck();
        }    
    }
    private IEnumerator EnableAttacking()
    {
        selfHealth.HealthBar.gameObject.SetActive(false);
        foreach(GameObject OBJ in pCubeCollection)
        {
            OBJ.SetActive(false);
        }
        yield return new WaitForSeconds(1.25f);
        foreach (GameObject OBJ in pCubeCollection)
        {
            OBJ.SetActive(true);
        }
        selfHealth.HealthBar.gameObject.SetActive(true);
        canAttack = true;
    }

    public void AgroRangeCheck()
    {
        if(!isBoss)
        {
            if (aiPath.destinationReached == true && isAnimated)
            {
                anim.SetBool("destinationReached", true);
            }
            else if (isAnimated)
            {
                anim.SetBool("destinationReached", false);

            }

            //calculate difference between positions of two transforms
            //If target is within (FlexTransform) distance of playerTarget, then swap target to player target

            var distance = Vector3.Distance(aiDestination.self.transform.position, aiDestination.playerTarget.transform.position);
            //Debug.Log(distance);

            if (distance < aiDestination.agroRange)
            {
                //moves player towards enemy
                aiDestination.flexTransform = aiDestination.playerTarget;
                //set animation for chasing player

                if (isAnimated == true)
                {
                    anim.SetBool("inRange", true);

                    anim.SetBool("inCombat", true);
                }

            }
            if (distance > aiDestination.agroRange && !inCombat)
            {
                aiDestination.flexTransform = aiDestination.self;
                if (isAnimated)
                {

                    anim.SetBool("inRange", false);
                   
                }

            }
        }
    }

    public void InCombat()
    {
        aiDestination.agroRange = 100;
        inCombat = true;
    }
}
