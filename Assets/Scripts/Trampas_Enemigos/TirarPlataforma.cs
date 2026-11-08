using System.Collections;
using UnityEngine;

public class TirarPlataforma : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float tiempoAntesDeCaer = 3f;
    [SerializeField] bool soloDesdeArriba = true;

    [Header("Caída")]
    [Tooltip("Turn off if your 'off' animation already moves the platform")]
    [SerializeField] bool caerConFisica = true;
    [SerializeField] float tiempoAntesDeDestruir = 1f;

    bool activada = false;
    Collider2D col;
    Rigidbody2D rb;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (activada || !collision.gameObject.CompareTag("Player")) return;

        // Ignore touches from the side or from below
        if (soloDesdeArriba &&
            collision.collider.bounds.min.y < col.bounds.max.y - 0.1f) return;

        activada = true;
        StartCoroutine(Caer());
    }

    IEnumerator Caer()
    {
        yield return new WaitForSeconds(tiempoAntesDeCaer);

        if (anim != null)
            anim.SetTrigger("Caer");

        if (caerConFisica && rb != null)
        {
            col.enabled = false;                    // the player drops through it
            rb.bodyType = RigidbodyType2D.Dynamic;  // gravity takes over
        }

        Destroy(gameObject, tiempoAntesDeDestruir);
    }
}