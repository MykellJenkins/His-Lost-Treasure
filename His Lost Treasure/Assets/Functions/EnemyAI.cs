using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("---Components---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;

    [Header("---Stats---")]
    [SerializeField] int HP;
    [SerializeField] float attackRate;

    Color ColorOG;

    Vector3 playerDir;

    float attackTimer;
    float angleToPlayer;

    bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorOG = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        if (playerInRange)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (attackTimer >= attackRate)
                {
                    attack();
                }
            }
        }
    }

    void attack()
    {
        attackTimer = 0f;



        //Q: How do I get him to do the animation and damage the player?    ?
        //First: find the player                                            x
        //Second: get in range to the player                                x
        //Third: play animation toward the player                           ?
        //Fourth: damage the player hopefully if I set up the sword right   ?

    }

    // Won't work properly idk
    //
    //bool canSeePlayer()
    //{
    //    playerDir = GameManager.instance.player.transform.forward - transform.position;
    //    angleToPlayer = Vector3.Angle(playerDir, transform.forward);

    //    if (angleToPlayer <= FOV)
    //    {
    //        agent.SetDestination(GameManager.instance.player.transform.position);

    //        if (agent.remainingDistance <= agent.stoppingDistance)
    //        {
    //            faceTarget();
    //        }
    //        if (attackTimer >= attackRate)
    //        {
    //            attack();
    //        }
    //        return true;
    //    }
    //    return false;
    //}

    //void faceTarget() 
    //{
    //    Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
    //    transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(int damageAmount, Vector3 attackerPosition)
    {
        HP -= damageAmount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        StartCoroutine(flashRed());

        if(HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = ColorOG;
    }
}
