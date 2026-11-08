using UnityEngine;

public class TrampolinSaltar : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float fuerzaSalto = 10f;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement jugador = collision.GetComponentInParent<PlayerMovement>();
        if (jugador == null) return;

        // Ignore the trampoline if the player is already moving upward
        if (collision.attachedRigidbody != null &&
            collision.attachedRigidbody.linearVelocity.y > 0.1f) return;

        jugador.Rebotar(fuerzaSalto);

        if (anim != null)
            anim.SetTrigger("salto");
    }
}