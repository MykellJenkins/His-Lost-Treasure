using UnityEngine;


public class MapController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Node currentNode;
    [SerializeField] int playerSpeed;
    bool isMoving;
    Vector3 target;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.transform.position = currentNode.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentNode.GetNextNode() != null && currentNode.GetNextNode().GetUnlocked())
        {
            
            if (Input.GetButtonDown("right") && !isMoving)
            {

                isMoving = true;
                target = currentNode.GetNextNodePos().position;



            }
        }
        if (isMoving)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, playerSpeed * Time.deltaTime);
            if (Vector3.Distance(player.transform.position, target) < 0.01f)
            {
                isMoving = false;
                currentNode = currentNode.GetNextNode();
            }
        }
    }
    
}
