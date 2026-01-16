using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDamageEffects : MonoBehaviour
{
    public float knockbackForce = 15f; // Adjusted for Impulse
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyKnockback(Vector3 attackerPosition)
    {
        if (rb != null)
        {
            // 1. Get horizontal direction away from the attacker
            Vector3 knockbackDir = (transform.position - attackerPosition);
            knockbackDir.y = 0; // Keep it purely horizontal for now
            knockbackDir.Normalize();

            // 2. Add just a tiny amount of Y to keep them from sticking to the floor
            // 0.2f is enough; 0.5f+ might make them feel like they are just jumping up.
            Vector3 finalDirection = knockbackDir + (Vector3.up * 0.2f);

            // 3. Clear velocity so we don't fight existing movement
            rb.linearVelocity = Vector3.zero;

            // 4. Apply a direct velocity change (Try knockbackForce = 15 to 25)
            rb.AddForce(finalDirection.normalized * knockbackForce, ForceMode.VelocityChange);

            // DEBUG: Draws a blue line in the Scene view showing the push direction
            Debug.DrawRay(transform.position, finalDirection.normalized * 5f, Color.blue, 1f);
        }
    }
}
