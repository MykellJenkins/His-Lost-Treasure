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
    bool isGrappling; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        freeLookCam.Priority = 10;
        grappleCam.Priority = 0;
    }
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
            freeLookCam.Priority = 0;
            grappleCam.Priority = 20;
            freeLookCam.gameObject.SetActive(false);
            grappleCam.gameObject.SetActive(true);
            RaycastHit hit;
            if (Physics.Raycast(GameManager.Instance.playerScript.transform.position, Camera.main.transform.forward,
                out hit, maxGrappleDistance, ~ignoreLayer) && hit.collider.CompareTag("GrapplePoint"))
            {
                Debug.Log("Hit: " + hit.collider.name);
                grapplePos = hit.point;
                isGrappling = true;
            }
        } 

        if (Input.GetKeyUp(KeyCode.F))
        {
            freeLookCam.Priority = 20;
            grappleCam.Priority = 0;
            freeLookCam.gameObject.SetActive(true);
            grappleCam.gameObject.SetActive(false);
            
            isGrappling = false;
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
