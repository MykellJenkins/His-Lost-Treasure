using UnityEngine;

public class PlayerHP : MonoBehaviour, IDamage
{
    [Header("------ Stats ------")]
    [Range(1, 3)][SerializeField] int HP;

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    public Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (HP >= 3)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);

        }
        else if (HP == 2)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(false);
        }
        else if (HP == 1)
        {
            heart1.SetActive(true);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }
        else if (HP <= 0)
        {
            heart1.SetActive(false);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }

    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
    }
}
