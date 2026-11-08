using UnityEngine;

public class GestionarAnimacionesCheckpoints : MonoBehaviour
{
    [SerializeField] Animator anim;

    bool activado = false;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (activado || !collision.CompareTag("Player")) return;

        activado = true;
        anim.SetInteger("Contacto", 1);   // never goes back to 0
    }
}