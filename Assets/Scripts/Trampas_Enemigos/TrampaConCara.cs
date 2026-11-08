using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class TrampaConCara : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] LayerMask suelo;

    [SerializeField] LayerMask jugador;   // the layer your Player is on

    [Header("Movimiento")]
    [SerializeField] float velocidadBajada = 12f;
    [SerializeField] float velocidadSubida = 3f;

    [Header("Esperas")]
    [SerializeField] float esperaArriba = 1.5f;    // idle at the top before dropping
    [SerializeField] float esperaAbajo = 0.3f;     // idle after hitting the floor
    [SerializeField] float retrasoInicial = 0f;    // offset so several traps don't sync

    enum Fase { EsperandoArriba, Bajando, EsperandoAbajo, Subiendo }

    Fase fase = Fase.EsperandoArriba;
    Rigidbody2D rb;
    Collider2D col;
    Vector2 posOriginal;
    float temporizador;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (anim == null) anim = GetComponent<Animator>();
    }

    void Start()
    {
        posOriginal = rb.position;
        temporizador = esperaArriba + retrasoInicial;
        PonerAnimacion(0);
    }

    void FixedUpdate()
    {
        switch (fase)
        {
            case Fase.EsperandoArriba:
                temporizador -= Time.fixedDeltaTime;
                if (temporizador <= 0f)
                {
                    fase = Fase.Bajando;
                    PonerAnimacion(1);
                }
                break;

            case Fase.Bajando:
                Bajar();
                break;

            case Fase.EsperandoAbajo:
                temporizador -= Time.fixedDeltaTime;
                if (temporizador <= 0f)
                {
                    fase = Fase.Subiendo;
                    PonerAnimacion(2);
                }
                break;

            case Fase.Subiendo:
                Subir();
                break;
        }
    }

    void Bajar()
{
    float paso = velocidadBajada * Time.fixedDeltaTime;

    // The cast now sees both the floor and the player
    RaycastHit2D hit = Physics2D.BoxCast(
        col.bounds.center, col.bounds.size, 0f, Vector2.down, paso,
        suelo.value | jugador.value);

    if (hit.collider != null)
    {
        // Stop exactly at the contact point, without pushing anything
        rb.MovePosition(rb.position + Vector2.down * hit.distance);

        // If it was the player, damage them directly
        PlayerHealth salud = hit.collider.GetComponentInParent<PlayerHealth>();
        if (salud != null)
            salud.Golpe(transform.position);

        fase = Fase.EsperandoAbajo;
        temporizador = esperaAbajo;
        PonerAnimacion(0);
        return;
    }

    rb.MovePosition(rb.position + Vector2.down * paso);
}

    void Subir()
    {
        float nuevaY = Mathf.MoveTowards(
            rb.position.y, posOriginal.y, velocidadSubida * Time.fixedDeltaTime);

        rb.MovePosition(new Vector2(rb.position.x, nuevaY));

        if (Mathf.Approximately(nuevaY, posOriginal.y))
        {
            fase = Fase.EsperandoArriba;
            temporizador = esperaArriba;
            PonerAnimacion(0);
        }
    }

    void PonerAnimacion(int valor)
    {
        if (anim != null)
            anim.SetInteger("subir", valor);   // 0 idle, 1 going down, 2 going up
    }
}
