using UnityEngine;
using Unity.Cinemachine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] Rigidbody rb;

    [SerializeField] int maxGrappleDistance;
    [SerializeField] int grappleSpeed;
    [SerializeField] CinemachineCamera freeLookCam;
    [SerializeField] CinemachineCamera grappleCam;

    Vector3 grapplePos;
    Vector3 startingGrapple;
    bool isGrappling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        freeLookCam.Priority = 20;
        grappleCam.Priority = 0;
    }
    // Update is called once per frame
    private void Update()
    { 
        Debug.DrawRay(GameManager.Instance.playerPrefab.GetComponent<CinemachineCamera>().transform.position, 
            GameManager.Instance.playerPrefab.GetComponent<CinemachineCamera>().transform.forward *
            maxGrappleDistance, Color.red);
        HandleGrapple();
    }
    void FixedUpdate()
    {
        HandleGrappling();
    } 
    void HandleGrapple() 
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            freeLookCam.Priority = 0; 
            grappleCam.Priority = 20;
            
        }
    
        if (Input.GetKeyUp(KeyCode.F) && !isGrappling) 
        {
            RaycastHit hit;
            if (Physics.Raycast(GameManager.Instance.playerPrefab.GetComponent<CinemachineCamera>().transform.position, 
            GameManager.Instance.playerPrefab.GetComponent<CinemachineCamera>().transform.forward, out hit, 
            maxGrappleDistance, ~ignoreLayer) && hit.collider.CompareTag("GrapplePoint"))
            {
                Debug.Log("Hit: " + hit.collider.name);
                grapplePos = hit.point;
                isGrappling = true;
                rb.useGravity = false;
                startingGrapple = rb.position;
            }
            freeLookCam.Priority = 20; 
            grappleCam.Priority = 0;
            
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
    private void OnCollisionEnter(Collision collision)
    {
        if(Vector3.Distance(startingGrapple, rb.position) > 1)
        {
            if (isGrappling) 
            {
                isGrappling = false;
                grapplePos = Vector3.zero;
                rb.useGravity = true;
            }
        }
        
    }

}
