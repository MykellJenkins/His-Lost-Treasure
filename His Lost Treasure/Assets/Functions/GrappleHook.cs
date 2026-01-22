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
        Debug.DrawRay(GameManager.Instance.playerScript.transform.position, Camera.main.transform.forward * maxGrappleDistance, Color.red);
        HandleGrapple();
    }
    void FixedUpdate()
    {
        HandelGrappling();
    } 
    void HandleGrapple() 
    { 
        if (Input.GetKeyDown(KeyCode.F)) 
        { 
            RaycastHit hit;
            if (Physics.Raycast(GameManager.Instance.playerScript.transform.position, Camera.main.transform.forward,
                out hit, maxGrappleDistance, ~ignoreLayer) && hit.collider.CompareTag("GrapplePoint")) 
            {
                Debug.Log("Hit: " + hit.collider.name);
                grapplePos = hit.point;
                isGrappling = true;
            }
        } 
    }
    void HandelGrappling()
    {
        if (isGrappling)
        {
            if (Vector3.Distance(rb.transform.position, grapplePos) > 0.1f)
            {
                rb.MovePosition(Vector3.MoveTowards(rb.transform.position, grapplePos, Time.fixedDeltaTime * grappleSpeed));
            }
            else
            {
                isGrappling = false;
            }
        }
      
    }

}
