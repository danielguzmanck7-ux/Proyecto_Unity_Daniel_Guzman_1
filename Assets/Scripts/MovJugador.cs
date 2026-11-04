using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovJugador : MonoBehaviour
{
    [Header("Capas")]
    [SerializeField] LayerMask Floor;

    [Header("Movimiento")]
    [SerializeField] float velocidad = 5f;
    [SerializeField] float velocidadj = 5f;

    [Header("Daño")]
    [SerializeField] float duracionGolpe = 0.4f;
    [SerializeField] AnadirVidasYManzanas estadisticas;
    int contadorSaltos = 0;

    bool recibiendoGolpe = false;
    bool muerto = false;
    Rigidbody2D jugador;
    BoxCollider2D cuadradoChoque;
    Animator anim;
    SpriteRenderer sprite;

    InputAction mover;
    InputAction saltar;

    [Header("Muerte")]
    [SerializeField] float duracionMuerte = 1f;      // length of the Dissappearing clip
    [SerializeField] float duracionAparecer = 1f;    // length of the Appearing clip
    Vector3 puntoRespawn;
    float gravedadOriginal;

    void Start()
    {
        mover = InputSystem.actions.FindAction("Mover");
        saltar = InputSystem.actions.FindAction("Saltar");
        jugador = GetComponent<Rigidbody2D>();
        cuadradoChoque = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        puntoRespawn = transform.position;
        gravedadOriginal = jugador.gravityScale;
        
        
    }

    void Update()
    {
        if (muerto || recibiendoGolpe)
            return;

        Moverse();
        Saltar();
        ActualizarAnimacion();
        
    }

    bool TocarSuelo()
    {
        Bounds bounds = cuadradoChoque.bounds;

        Vector2 pies = new Vector2(bounds.center.x, bounds.min.y - 0.02f);
        Vector2 tamano = new Vector2(bounds.size.x * 0.8f, 0.08f);

        return Physics2D.OverlapBox(pies, tamano, 0f, Floor) != null;
    }
    void Saltar()
    {
        if (!saltar.WasPressedThisFrame()) return;

        // Solo salta si le queda algún salto disponible
        if (contadorSaltos >= 2) return;

        jugador.linearVelocity = new Vector2(jugador.linearVelocity.x, velocidadj);
        contadorSaltos++;

        anim.SetInteger("estado", contadorSaltos == 1 ? 1 : 5);
    }

    void Moverse()
    {
        float c = mover.ReadValue<Vector2>().x;
        
        jugador.linearVelocity = new Vector2(c * velocidad, jugador.linearVelocity.y);
        

        // Girar el sprite según la dirección
        if (c > 0.01f) sprite.flipX = false;
        else if (c < -0.01f) sprite.flipX = true;

        // Restablecer saltos solo cuando está en el suelo y no subiendo
        if (TocarSuelo() && jugador.linearVelocity.y <= 0.01f)
        {
            contadorSaltos = 0;
        }
        else if (!TocarSuelo() && contadorSaltos == 0)
        {
            contadorSaltos = 1;
        }
    }

    void ActualizarAnimacion()
    {
        if (muerto) return;

        bool enSuelo = TocarSuelo() && jugador.linearVelocity.y <= 0.01f;
        float c = mover.ReadValue<Vector2>().x;

        int estado;

        if (!enSuelo)
        {
            if (jugador.linearVelocity.y < -0.1f)
                estado = 6; // caer
            else
                estado = anim.GetInteger("estado") == 5 ? 5 : 1;
        }
        else if (Mathf.Abs(c) > 0.1f)
        {
            estado = 2; // correr
        }
        else
        {
            estado = 0; // estático
        }

        anim.SetInteger("estado", estado);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Trap"))
        {
            Golpe();
        }
    }

public void Golpe()
{
    if (recibiendoGolpe || muerto) return;

    estadisticas.RestarVida();

    if (estadisticas.Vidas <= 0)
    {
        StartCoroutine(MorirYReaparecer());
        return;
    }

    StartCoroutine(GolpeCoroutine());
}

    IEnumerator GolpeCoroutine()
    {
        recibiendoGolpe = true;
        anim.SetInteger("estado", 4);
        jugador.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duracionGolpe);

        recibiendoGolpe = false;
    }

    IEnumerator MorirYReaparecer()
{
    muerto = true;
    recibiendoGolpe = false;
    jugador.linearVelocity = Vector2.zero;
    jugador.gravityScale = 0f;
    anim.SetInteger("estado", 7);

    yield return new WaitForSeconds(duracionMuerte);

    transform.position = puntoRespawn;
    estadisticas.ReiniciarVidas();
    contadorSaltos = 0;
    anim.SetInteger("estado", 8);

    yield return new WaitForSeconds(duracionAparecer);

    jugador.gravityScale = gravedadOriginal;
    anim.SetInteger("estado", 0);
    muerto = false;
}
}