using UnityEngine;

public class AddPoints : MonoBehaviour
{
    public int pointsToAdd;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HighScoreManager.Instance.AddPoints(pointsToAdd);
            Destroy(gameObject);
        }
    }
}
