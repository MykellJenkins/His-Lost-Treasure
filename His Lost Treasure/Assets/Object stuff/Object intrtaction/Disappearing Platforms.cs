using System.Collections;
using UnityEngine;

public class DisappearingPlatforms : MonoBehaviour
{
    [Header("Timing")]
    public float disappearDelay = 0.5f;
    public float fadeDuration = 0.5f;
    public float respawnDelay = 2f;

    private Collider col;
    private MeshRenderer mesh;
    private Material mat;
    private bool isActive = true;

    void Awake()
    {
        col = GetComponent<Collider>();
        mesh = GetComponent<MeshRenderer>();

        // IMPORTANT: Instance material so we don’t affect shared material
        mat = mesh.material;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DisappearRoutine());
        }
    }

    IEnumerator DisappearRoutine()
    {
        isActive = false;

        yield return new WaitForSeconds(disappearDelay);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));

        // Disable collision after fade
        col.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Re-enable
        col.enabled = true;

        // Fade back in
        yield return StartCoroutine(Fade(0f, 1f));

        isActive = true;
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = mat.color;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            mat.color = new Color(color.r, color.g, color.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }

        // Ensure final value
        mat.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
