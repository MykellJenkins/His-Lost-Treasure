using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{
    [SerializeField] bool unlocked;
    [SerializeField] Node nextNode;
    [SerializeField] Transform nodePos;
    [SerializeField] string levelName;

    bool levelCompleted;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    void CompleteLevel()
    {
        levelCompleted = true;
        if (nextNode != null)
        {
            nextNode.Unlock();
        }
    }
    void Unlock()
    {
        unlocked=true;
    }
    public Transform GetNextNodePos()
    {
        return nextNode.transform;
    }
    public Node GetNextNode()
    {
        return nextNode;
    }
    public bool GetUnlocked()
    {
        return unlocked;
    }
    public string GetNodeLevelName()
    {
        return levelName;
    }
}

