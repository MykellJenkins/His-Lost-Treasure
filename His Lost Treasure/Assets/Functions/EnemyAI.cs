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
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [Header("---Stats---")]
    [SerializeField] int HP;
    [SerializeField] float attackRate;

    Color ColorOG;

    Vector3 playerDir;
    Vector3 startingPos;

    float attackTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOG;

    bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorOG = model.material.color;
        startingPos = transform.position;
        stoppingDistOG = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        if(agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if(playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }
    }

    void checkRoam()
    {
        if (agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
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

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }
                if (attackTimer >= attackRate)
                {
                    attack();
                }

                agent.stoppingDistance = stoppingDistOG;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

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
            agent.stoppingDistance = 0;
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
