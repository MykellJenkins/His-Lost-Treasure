using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePortal : MonoBehaviour
{

    public string nextSceneName = "Level 2"; //Name of the scene to load replace the ? with the actual scene name

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has reached the end game portal.");
            GameManager.instance.endOfLevel = true;
        }

    }
}
