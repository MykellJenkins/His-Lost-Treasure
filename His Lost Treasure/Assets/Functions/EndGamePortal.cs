using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Portal triggered by: " + other.name);

        if (!other.CompareTag("Player")) return;

        Debug.Log("Calling TriggerWin()");
        GameManager.Instance.TriggerWin();

        GetComponent<Collider>().enabled = false;
    }
}
