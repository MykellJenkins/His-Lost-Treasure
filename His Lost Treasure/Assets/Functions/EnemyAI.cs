using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("---Components---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("---Stats---")]
    [SerializeField] int HP;
    [SerializeField] float attackRate;

    Color ColorOG;

    float attackTimer;

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

            if(attackTimer >= attackRate)
            {
                attack();
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
