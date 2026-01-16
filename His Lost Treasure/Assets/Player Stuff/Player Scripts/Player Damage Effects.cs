using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDamageEffects : MonoBehaviour
{
    public float knockbackForce = 15f; // Adjusted for Impulse
    private float knockbackTimer = 2;
    private bool isKnockbackActive = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isKnockbackActive)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockbackActive = false;
                knockbackTimer = 2; // Reset timer
            }
        }
    }

    public void ApplyKnockback(Vector3 attackerPosition)
    {
        if (rb != null)
        {
            if (isKnockbackActive == true)
            {
                return; // Still in knockback cooldown
            }
            Vector3 knockbackDir = (transform.position - attackerPosition);
            knockbackDir.y = 0; // Keep it purely horizontal for now
            knockbackDir.Normalize();

            
            Vector3 finalDirection = knockbackDir + (Vector3.up * 0.2f);

            
            rb.linearVelocity = Vector3.zero;

            
            rb.AddForce(finalDirection.normalized * knockbackForce, ForceMode.VelocityChange);

            
            Debug.DrawRay(transform.position, finalDirection.normalized * 5f, Color.blue, 1f);

            isKnockbackActive = true;
        }
    }
}
