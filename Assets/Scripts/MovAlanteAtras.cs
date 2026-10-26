using UnityEngine;

public class MovAlanteAtras : MonoBehaviour
{
    [SerializeField] float velocidad = 3.3f;
    [SerializeField] LayerMask Pared;
    [SerializeField] float distanciaDeteccionPared = 0.1f;

    BoxCollider2D cajaChoque;
    Rigidbody2D enemigo;
    Animator anim;
    int direccion = 1; // 1 = derecha (+x), -1 = izquierda (-x)

    void Start()
    {
        anim = GetComponent<Animator>();
        enemigo = GetComponent<Rigidbody2D>();
        cajaChoque = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Lanza un rayo desde el centro hacia donde se mueve el enemigo
        float mitadAncho = cajaChoque.bounds.extents.x;
        Vector2 origen = cajaChoque.bounds.center;
        float distancia = mitadAncho + distanciaDeteccionPared;

        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.right * direccion, distancia, Pared);

        // Si hay pared delante, da la vuelta
        if (hit.collider != null)
        {
            direccion *= -1;
        }

        // Movimiento con el Rigidbody2D (no con Translate)
        enemigo.linearVelocity = new Vector2(direccion * velocidad, enemigo.linearVelocity.y);

        // Gira el sprite según la dirección
        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direccion;
        transform.localScale = escala;

        // Animación: 1 si se mueve, 0 si está parado
        int estado = Mathf.Abs(enemigo.linearVelocity.x) > 0.01f ? 1 : 0;
        anim.SetInteger("estado", estado);
    }
}