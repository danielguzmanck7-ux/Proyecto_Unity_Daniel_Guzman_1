using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovJugador : MonoBehaviour
{
    [Header("Capas")]
    [SerializeField] LayerMask Floor;
    [SerializeField] LayerMask Pared;

    [Header("Movimiento")]
    [SerializeField] float velocidad = 5f;
    [SerializeField] float velocidadj = 5f;
    [SerializeField] float fuerzaPared = 6f;

    [Header("Salto de pared")]
    [SerializeField] float duracionImpulsoPared = 0.15f;
    [SerializeField] float distanciaDeteccionPared = 0.1f;

    [Header("Daño")]
    [SerializeField] float duracionGolpe = 0.4f;
    [SerializeField] int vidasMaximas = 3;

    int vidas;
    int contadorSaltos = 0;
    int ladoPared = 0;

    bool recibiendoGolpe = false;
    bool impulsoParedActivo = false;
    bool muerto = false;
    bool saltoParedEnCurso = false;

    Rigidbody2D jugador;
    BoxCollider2D cuadradoChoque;
    Animator anim;
    SpriteRenderer sprite;

    InputAction mover;
    InputAction saltar;

    void Start()
    {
        mover = InputSystem.actions.FindAction("Mover");
        saltar = InputSystem.actions.FindAction("Saltar");

        jugador = GetComponent<Rigidbody2D>();
        cuadradoChoque = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        vidas = vidasMaximas;
    }

    void Update()
    {
        if (muerto || recibiendoGolpe)
            return;

        Moverse();

        if (!SaltoPared())
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

    bool TocarPared()
    {
        Bounds bounds = cuadradoChoque.bounds;

        Vector2 tamano = new Vector2(distanciaDeteccionPared, bounds.size.y * 0.8f);

        Vector2 centroDerecha = new Vector2(bounds.max.x + distanciaDeteccionPared / 2f, bounds.center.y);
        Vector2 centroIzquierda = new Vector2(bounds.min.x - distanciaDeteccionPared / 2f, bounds.center.y);

        bool derecha = Physics2D.OverlapBox(centroDerecha, tamano, 0f, Pared) != null;
        bool izquierda = Physics2D.OverlapBox(centroIzquierda, tamano, 0f, Pared) != null;

        if (derecha)
        {
            ladoPared = -1;
            return true;
        }

        if (izquierda)
        {
            ladoPared = 1;
            return true;
        }

        ladoPared = 0;
        return false;
    }

    bool SaltoPared()
    {
        if (TocarSuelo()) return false;
        if (!TocarPared()) return false;
        if (!saltar.WasPressedThisFrame()) return false;

        jugador.linearVelocity = new Vector2(ladoPared * fuerzaPared, velocidadj);

        // Tras el salto de pared el jugador puede hacer un doble salto
        contadorSaltos = 1;

        StartCoroutine(BloquearMovimientoPared());

        saltoParedEnCurso = true;
        anim.SetInteger("estado", 3);

        return true;
    }

    IEnumerator BloquearMovimientoPared()
    {
        impulsoParedActivo = true;
        yield return new WaitForSeconds(duracionImpulsoPared);
        impulsoParedActivo = false;
        saltoParedEnCurso = false;
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

        if (!impulsoParedActivo)
        {
            jugador.linearVelocity = new Vector2(c * velocidad, jugador.linearVelocity.y);
        }

        // Girar el sprite según la dirección
        if (c > 0.01f) sprite.flipX = false;
        else if (c < -0.01f) sprite.flipX = true;

        // Restablecer saltos solo cuando está en el suelo y no subiendo
        if (TocarSuelo() && jugador.linearVelocity.y <= 0.01f)
        {
            contadorSaltos = 0;
        }
    }

    void ActualizarAnimacion()
    {
        if (muerto) return;

        bool enSuelo = TocarSuelo();
        float c = mover.ReadValue<Vector2>().x;

        int estado;

        if (saltoParedEnCurso)
        {
            estado = 3;
        }
        else if (!enSuelo)
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

        vidas--;

        if (vidas <= 0)
        {
            Morir();
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

    void Morir()
    {
        muerto = true;
        jugador.linearVelocity = Vector2.zero;
        anim.SetInteger("estado", 7);
    }
}