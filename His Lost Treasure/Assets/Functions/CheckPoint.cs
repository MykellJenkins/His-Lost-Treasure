using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private bool saveOnTrigger = true;

    [Header("Audio")]
    [SerializeField] private AudioSource checkpointSound;

    private void Awake()
    {
        if (checkpointSound == null)
            checkpointSound = GetComponent<AudioSource>();
    }

    private bool isActivated = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            isActivated = true;
            if (other.CompareTag("Player"))
            {
                // Play checkpoint sound
                if (checkpointSound != null)
                {
                    checkpointSound.pitch = Random.Range(0.95f, 1.05f); // optional polish
                    checkpointSound.Play();
                }

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
