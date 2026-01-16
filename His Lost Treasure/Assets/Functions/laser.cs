using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour
{
    [SerializeField] LineRenderer laserLine;

    [SerializeField] Transform model;
    [SerializeField] GameObject hitEffect;

    [SerializeField] int dist;
    [SerializeField] int damageamount;
    [SerializeField] float damageTimer;
    [SerializeField] float laserTimer;
    [SerializeField] int rotateSpeed;

    bool isDamaging;


    // Update is called once per frame
    void Update()
    {
        model.transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);


        createLaser();
    }

    void createLaser()
    {
        laserTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(model.position, model.transform.forward, out hit, dist))
        {
            Debug.DrawLine(model.position, hit.point);

            laserLine.SetPosition(0, model.position);
            laserLine.SetPosition(1, hit.point);
            hitEffect.SetActive(true);
            hitEffect.transform.position = hit.point;

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null && !isDamaging)
            {
                StartCoroutine(damageTime(dmg));
            }
        }
        else
        {
            laserLine.SetPosition(0, model.position);
            laserLine.SetPosition(1, model.position + model.forward * dist);
            hitEffect.SetActive(false);
        }
    }

    IEnumerator damageTime(IDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageamount, transform.position);
        yield return new WaitForSeconds(damageTimer);
        isDamaging = false;

    }

}
