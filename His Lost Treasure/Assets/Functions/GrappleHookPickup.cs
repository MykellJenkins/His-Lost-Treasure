using UnityEngine;

public class GrappleHookPickup : MonoBehaviour
{
    bool havePickedup;
    private void Update()
    {
        havePickedup = GameManager.Instance.player.GetComponent<GrappleHook>().GetCanGrapple();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (havePickedup == false)
            {
                GameManager.Instance.player.GetComponent<GrappleHook>().SetCanGrapple(true);
            }

            Destroy(gameObject);
        }
    }
}
