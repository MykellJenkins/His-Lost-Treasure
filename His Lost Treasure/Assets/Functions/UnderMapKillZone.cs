using UnityEngine;

public class UnderMapKillZone : MonoBehaviour
{
    public int damageAmount = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           Player HP = other.GetComponent<Player>();
            if (HP != null)
            {
                HP.TakeDamage(damageAmount);
            }

            RespawnManager.Instance.RespawnPlayer(other.gameObject);
        }
    }
}
