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

    int contadorSaltos = 0;
    int ladoPared = 0;

    bool recibiendoGolpe = false;
    bool impulsoParedActivo = false;

    Rigidbody2D jugador;
    BoxCollider2D cuadradoChoque;
    Animator anim;

    InputAction mover;
    InputAction saltar;

    void Start()
    {
        mover = InputSystem.actions.FindAction("Mover");
        saltar = InputSystem.actions.FindAction("Saltar");

        jugador = GetComponent<Rigidbody2D>();
        cuadradoChoque = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (recibiendoGolpe)
            return;

        Moverse();

        if (!SaltoPared())
            Saltar();

        ActualizarAnimacion();
    }

    // Comprueba si el jugador está tocando el suelo.
    bool TocarSuelo()
    {
        Bounds bounds = cuadradoChoque.bounds;

        Vector2 pies = new Vector2(
            bounds.center.x,
            bounds.min.y - 0.02f
        );

        Vector2 tamano = new Vector2(
            bounds.size.x * 0.8f,
            0.08f
        );

        return Physics2D.OverlapBox(
            pies,
            tamano,
            0f,
            Floor
        ) != null;
    }

    // Detecta paredes a izquierda y derecha.
    bool TocarPared()
    {
        Bounds bounds = cuadradoChoque.bounds;

        float distancia = bounds.extents.x
                          + distanciaDeteccionPared / 2f;

        Vector2 tamano = new Vector2(
            distanciaDeteccionPared,
            bounds.size.y * 0.8f
        );

        Vector2 centroDerecha = new Vector2(
            bounds.max.x + distanciaDeteccionPared / 2f,
            bounds.center.y
        );

        Vector2 centroIzquierda = new Vector2(
            bounds.min.x - distanciaDeteccionPared / 2f,
            bounds.center.y
        );

        bool derecha = Physics2D.OverlapBox(
            centroDerecha,
            tamano,
            0f,
            Pared
        ) != null;

        bool izquierda = Physics2D.OverlapBox(
            centroIzquierda,
            tamano,
            0f,
            Pared
        ) != null;

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

    // Salto de pared.
    bool SaltoPared()
    {
        if (TocarSuelo())
            return false;

        if (!TocarPared())
            return false;

        if (!saltar.WasPressedThisFrame())
            return false;

        // Impulso en dirección contraria a la pared.
        jugador.linearVelocity = new Vector2(
            ladoPared * fuerzaPared,
            velocidadj
        );

        // Permite realizar un doble salto después.
        contadorSaltos = 1;

        // Evita que Moverse() cancele el impulso.
        StartCoroutine(BloquearMovimientoPared());

        anim.SetInteger("estado", 3);

        return true;
    }

    IEnumerator BloquearMovimientoPared()
    {
        impulsoParedActivo = true;

        yield return new WaitForSeconds(duracionImpulsoPared);

        impulsoParedActivo = false;
    }

    // Salto normal y doble salto.
    void Saltar()
    {
        if (!saltar.WasPressedThisFrame())
            return;

        if (TocarSuelo())
            contadorSaltos = 0;

        if (contadorSaltos >= 2)
            return;

        jugador.linearVelocity = new Vector2(
            jugador.linearVelocityX,
            velocidadj
        );

        contadorSaltos++;

        if (contadorSaltos == 1)
        {
            anim.SetInteger("estado", 1);
        }
        else
        {
            anim.SetInteger("estado", 5);
        }
    }

    // Movimiento horizontal.
    void Moverse()
    {
        float c = mover.ReadValue<Vector2>().x;

        // No cancelar el impulso horizontal del salto de pared.
        if (!impulsoParedActivo)
        {
            jugador.linearVelocity = new Vector2(
                c * velocidad,
                jugador.linearVelocityY
            );
        }

        // Restablecer los saltos al tocar el suelo.
        if (TocarSuelo())
        {
            contadorSaltos = 0;
        }
    }

    // Actualiza las animaciones según el movimiento.
    void ActualizarAnimacion()
{
    bool enSuelo = TocarSuelo();
    float c = mover.ReadValue<Vector2>().x;

    int estado;

    if (!enSuelo)
    {
        if (jugador.linearVelocityY < -0.1f)
        {
            estado = 6; // Caer
        }
        else
        {
            int estadoActual = anim.GetInteger("estado");

            if (estadoActual == 5)
                estado = 5; // DobleSalto
            else if (estadoActual == 3)
                estado = 3; // SaltoPared
            else
                estado = 1; // Saltar
        }
    }
    else if (Mathf.Abs(c) > 0.1f)
    {
        estado = 2; // Correr
    }
    else
    {
        estado = 0; // Estatico
    }

    anim.SetInteger("estado", estado);
}

    // Detectar el contacto con el enemigo.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Trap"))
        {
            Golpe();
        }
    }

    public void Golpe()
    {
        if (recibiendoGolpe)
            return;

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
}