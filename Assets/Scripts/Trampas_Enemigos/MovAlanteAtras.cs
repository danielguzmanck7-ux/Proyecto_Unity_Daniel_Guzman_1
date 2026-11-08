using UnityEngine;

public class MovAlanteAtras : MonoBehaviour
{
    [SerializeField] float velocidad = 3.3f;

    [Header("Detección")]
    [SerializeField] LayerMask Suelo;                      // floor layer(s)
    [SerializeField] LayerMask Pared;                      // optional, leave empty if no walls
    [SerializeField] float distanciaDeteccionPared = 0.1f;
    [SerializeField] float margenBorde = 0.1f;             // how far ahead of the front foot to look for floor
    [SerializeField] float distanciaSuelo = 0.4f;          // how far down to look for floor
    [SerializeField] float tiempoEntreGiros = 0.2f;        // prevents flipping every frame

    BoxCollider2D cajaChoque;
    Rigidbody2D enemigo;
    Animator anim;
    int direccion = 1; // 1 = right (+x), -1 = left (-x)
    float proximoGiroPermitido = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        enemigo = GetComponent<Rigidbody2D>();
        cajaChoque = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        if (Time.time >= proximoGiroPermitido && (HayPared() || SeVaACaer()))
            DarLaVuelta();

        // Move with the Rigidbody2D
        enemigo.linearVelocity = new Vector2(direccion * velocidad, enemigo.linearVelocity.y);

        // Flip the sprite
        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direccion;
        transform.localScale = escala;

        // Animation: 1 if moving, 0 if stopped
        int estado = Mathf.Abs(enemigo.linearVelocity.x) > 0.01f ? 1 : 0;
        anim.SetInteger("estado", estado);
    }

    bool HayPared()
    {
        float mitadAncho = cajaChoque.bounds.extents.x;
        Vector2 origen = cajaChoque.bounds.center;
        float distancia = mitadAncho + distanciaDeteccionPared;

        return Physics2D.Raycast(origen, Vector2.right * direccion, distancia, Pared).collider != null;
    }

    bool SeVaACaer()
    {
        Bounds b = cajaChoque.bounds;

        // Just in front of the front foot, slightly above the bottom of the collider
        Vector2 origen = new Vector2(
            b.center.x + direccion * (b.extents.x + margenBorde),
            b.min.y + 0.05f);

        // If there is no floor below that point, the edge is coming up
        return Physics2D.Raycast(origen, Vector2.down, distanciaSuelo, Suelo).collider == null;
    }

    void DarLaVuelta()
    {
        direccion *= -1;
        proximoGiroPermitido = Time.time + tiempoEntreGiros;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Only turn on side contact, not when the player lands on top
        if (Mathf.Abs(collision.GetContact(0).normal.x) > 0.5f && Time.time >= proximoGiroPermitido)
            DarLaVuelta();
    }
}