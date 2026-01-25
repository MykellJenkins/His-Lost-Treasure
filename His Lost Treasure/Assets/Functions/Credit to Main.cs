using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CredittoMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CreditsBack());
    }

 IEnumerator CreditsBack()
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("Menu");
    }
}
