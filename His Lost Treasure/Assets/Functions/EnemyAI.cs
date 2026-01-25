using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("---Components---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTranSpeed;
    [SerializeField] Animator anim; 

    [Header("---Stats---")]
    [SerializeField] int HP;
    [SerializeField] float attackRate;

    [Header("---Audio---")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audAttack;
    [SerializeField] float audAttackVol;
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] float audStepsVol;
    [SerializeField] float stepRate;


    Color ColorOG;

    Vector3 playerDir;
    Vector3 startingPos;

    float attackTimer;
    float roamTimer;
    float stepsTimer;

    float angleToPlayer;
    float stoppingDistOG;

    bool playerInRange;
    bool isPlayingSteps;

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
        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;
        locomotionAnim();
        CheckSteps();
        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        if (playerInRange && canSeePlayer() && agent.remainingDistance <= agent.stoppingDistance && attackTimer >= attackRate)
        {
            attack();
        }
        if (!playerInRange)
        {
            checkRoam();
        }
    }

    void locomotionAnim()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float agentSpeedAnim = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.MoveTowards(agentSpeedAnim, agentSpeedCur, Time.deltaTime * animTranSpeed));
    }

    void CheckSteps()
    {
        if(agent.hasPath && agent.velocity.sqrMagnitude > 0.1f)
        {
            if (!isPlayingSteps)
            {
                StartCoroutine(playSteps());
            }
        }
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        yield return new WaitForSeconds(stepRate);
        isPlayingSteps = false;
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
        anim.SetTrigger("Attack");
        aud.PlayOneShot(audAttack[Random.Range(0, audAttack.Length)], audAttackVol);
        anim.SetTrigger("Exit");
        attackTimer = 0f;
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.Instance.playerScript.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV)
            {
                
                // Problem could be here for new player char locating 

                agent.SetDestination(GameManager.Instance.playerScript.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
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
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        // Problem could be here for new player char locating 
        agent.SetDestination(GameManager.Instance.playerScript.transform.position);
        
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
