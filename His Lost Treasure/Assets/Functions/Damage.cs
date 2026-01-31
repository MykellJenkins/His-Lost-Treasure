using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing, crushing}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] crushingscript crush;
    [SerializeField] Player player;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] float damageRate;
    [SerializeField] int destroyTime;

    bool isDamaging;
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);
            if (type == damageType.moving && rb != null)
            {
                // In Unity 6 (2026), use linearVelocity for Rigidbody movement
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }
    private void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Damage Trigger] Collided with: " + other.name);

        if (other.isTrigger) return;
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            Debug.Log("[Damage Trigger] Player hit! Taking damage");
            dmg.TakeDamage(damageAmount, transform.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
       
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        // PASS transform.position here as well
        d.TakeDamage(damageAmount, transform.position);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
