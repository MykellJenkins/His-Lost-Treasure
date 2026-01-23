using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Rigidbody rb;

    [SerializeField] int maxGrappleDistance; 
    [SerializeField] int grappleSpeed;
    
   
    Vector3 grapplePos; 

    bool isGrappling; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }
    // Update is called once per frame
    private void Update()
    {
        
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * maxGrappleDistance, Color.red);
        HandleGrapple();
    }
    void FixedUpdate()
    {
        HandleGrappling();
    } 
    void HandleGrapple() 
    { 
        if (Input.GetKeyDown(KeyCode.F) && !isGrappling) 
        {
            RaycastHit hit;
            if (Physics.Raycast(GameManager.Instance.playerScript.transform.position, Camera.main.transform.forward,
                out hit, maxGrappleDistance, ~ignoreLayer) && hit.collider.CompareTag("GrapplePoint")) 
            {
                Debug.Log("Hit: " + hit.collider.name);
                grapplePos = hit.point;
                isGrappling = true;
                rb.useGravity = false;
            }
        } 
    }
    void HandleGrappling()
    {
        if (isGrappling)
        {
            if (Vector3.Distance(rb.position, grapplePos) > 0.9f)
            {
                rb.MovePosition(Vector3.MoveTowards(rb.position, grapplePos, Time.fixedDeltaTime * grappleSpeed));
            }
            else
            {
                isGrappling = false;
                grapplePos = Vector3.zero;
                rb.useGravity = true;
            }
        }
      
    }

}
