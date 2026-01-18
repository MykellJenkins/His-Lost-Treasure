using System.Collections;
using UnityEngine;

public class crushingscript : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] float speed;
    [SerializeField] float resetTime;
    

    Vector3 prevpos;
    Vector3 target;
    bool isMovingDown;
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

        isMovingDown = transform.position.y < prevpos.y;
        prevpos = transform.position;

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            StartCoroutine(SwapTarget());
        }
    }
    public bool GetIsMovingDown()
    {
        return isMovingDown; 
    }
   
    IEnumerator SwapTarget()
    {
        stopped = true;
        yield return new WaitForSeconds(resetTime);

        target = target == start.position ? end.position : start.position;
        stopped = false;
    }
}
