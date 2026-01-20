using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private bool saveOnTrigger = true;

    private bool isActivated = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            isActivated = true;
            if (other.CompareTag("Player"))
            {
                // 1. Update the Respawn position in memory
                GameManager.Instance.rmInstance.SetCheckPoint(transform.position);

                // 2. Trigger a Persistent Save
                if (saveOnTrigger)
                {
                    SaveProgress();
                }

                // 3. Optional: Visual feedback (e.g., change color or play sound)
                // GetComponent<Renderer>().material.color = Color.green;
            }
        }
        
    }

    private void SaveProgress()
    {
        // Capture current player state and write to JSON
        PlayerSaveData data = GameManager.Instance.playerScript.GetSaveData();
        SavePlayerData.Instance.SavePlayer(data);

        Debug.Log("Checkpoint Saved to Disk!");
    }

    private void Update()
    {
        // Use Time.deltaTime for frame-rate independent rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
