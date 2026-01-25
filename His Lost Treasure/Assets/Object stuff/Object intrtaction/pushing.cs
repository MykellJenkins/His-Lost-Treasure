using UnityEngine;

public class pushing : MonoBehaviour
{
    public string playerTag = "Player";
    public string pushAble = "pushable";
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            rb.isKinematic = false; // Allow player to push
        }
        else if (collision.collider.CompareTag(pushAble))
        {
            rb.isKinematic = true; // Prevent pushing other objects
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            rb.isKinematic = true;
        }
        else {
            rb.isKinematic = false;
        }

        
    }
}
