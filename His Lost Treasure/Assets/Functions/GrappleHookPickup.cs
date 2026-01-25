using UnityEngine;

public class GrappleHookPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!GameManager.Instance.player.GetComponent<GrappleHook>().GetCanGrapple())
            {
                GameManager.Instance.player.GetComponent<GrappleHook>().SetCanGrapple(true);
            }

            Destroy(gameObject);
        }
    }
}
