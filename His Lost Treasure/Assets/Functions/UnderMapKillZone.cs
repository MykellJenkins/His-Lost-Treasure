using UnityEngine;

public class UnderMapKillZone : MonoBehaviour
{
    public int damageAmount = 100;

    private void OnTriggerEnter(Collider playerScript)
    {
        if (playerScript.CompareTag("Player"))
        {
           Player HP = playerScript.GetComponent<Player>();
            if (HP != null)
            {
                HP.TakeDamage(damageAmount, transform.position);
            }

            RespawnManager.Instance.RespawnPlayer(playerScript.gameObject.GetComponent<Player>());
        }
    }
}
