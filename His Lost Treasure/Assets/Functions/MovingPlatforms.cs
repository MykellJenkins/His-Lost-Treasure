using UnityEngine;
using System.Collections;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] float speed;
    [SerializeField] float resetTime;

    Vector3 target;

    bool stopped;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = end.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopped) return;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            StartCoroutine(SwapTarget());
        }
    }
    IEnumerator SwapTarget()
    {
        stopped = true;
        yield return new WaitForSeconds(resetTime);

        target = target == start.position ? end.position : start.position;
        stopped = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);

        }
    }
}
