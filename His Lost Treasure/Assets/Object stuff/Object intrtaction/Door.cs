using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public int requiredKeyID;
    public float openHeight = 2f;
    public float openSpeed = 2f;

    private bool isLocked = true;
    private bool isOpen = false;
    private Vector3 closedPos;
    private Vector3 openPos;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isLocked && !isOpen)
        {
            StartCoroutine(OpenDoor());
        }
    }

    public void Unlock()
    {
        isLocked = false;
    }

    IEnumerator OpenDoor()
    {
        isOpen = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.position = Vector3.Lerp(closedPos, openPos, t);
            yield return null;
        }

        transform.position = openPos;
    }
}