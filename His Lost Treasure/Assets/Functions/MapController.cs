using UnityEngine;
using UnityEngine.SceneManagement;


public class MapController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Node currentNode;
    [SerializeField] int playerSpeed;
    string levelName;
    bool isMoving;
    bool movingForward;
    Vector3 target;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.transform.position = currentNode.transform.position;
        levelName = currentNode.GetNodeLevelName();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene(levelName);
        }
        if (currentNode.GetNextNode() != null && currentNode.GetNextNode().GetUnlocked())
        {
            
            if (Input.GetButtonDown("Right") && !isMoving)
            {

                isMoving = true;
                movingForward = true;
                target = currentNode.GetNextNodePos().position;



            }
        } 
            if (currentNode.GetPrevNode() != null && currentNode.GetPrevNode().GetUnlocked())
        {
            if (Input.GetButtonDown("Left") && !isMoving)
            {

                isMoving = true;
                movingForward = false;
                target = currentNode.GetPrevNodePos().position;



            }
        }
        if (isMoving)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, playerSpeed * Time.deltaTime);
            if (Vector3.Distance(player.transform.position, target) < 0.01f)
            {
                isMoving = false;
                if (movingForward)
                {
                    currentNode = currentNode.GetNextNode();
                }else if (!movingForward) 
                {
                    currentNode = currentNode.GetPrevNode();
                }
                    levelName = currentNode.GetNodeLevelName();
            }
        }
    }
    
}
