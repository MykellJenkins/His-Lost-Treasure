using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnManager.Instance.SetCheckPoint(transform.position);
        }
    }

    private void Update()
    {
        RotateItem();
    }

    void RotateItem()
    {
        Vector3 rotSpeed = new Vector3(0, 90, 0);
        transform.Rotate(rotSpeed * Time.deltaTime);
    }
}
