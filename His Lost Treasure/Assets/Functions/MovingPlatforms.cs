using UnityEngine;
using System.Collections;


public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] float speed;
    [SerializeField] float resetTime;

    Vector3 target;

    bool stopped;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = end.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stopped) return;

        rb.MovePosition(Vector3.MoveTowards(rb.position, target, Time.fixedDeltaTime * speed));

        if (Vector3.Distance(rb.position, target) < 0.01f)
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
  
}
