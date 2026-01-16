using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;

    [SerializeField] int HP;

    Color ColorOG;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorOG = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        StartCoroutine(flashRed());

        if(HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = ColorOG;
    }
}
