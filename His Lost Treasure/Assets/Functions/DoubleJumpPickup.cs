using UnityEngine;

public class DoubleJumpPickup : MonoBehaviour
{
    [SerializeField] int jumpIncrease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
            return;
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerScript.jumps += jumpIncrease;
            Destroy(gameObject);
        }
    }
}
