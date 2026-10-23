using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovJugador : MonoBehaviour
{
    [SerializeField] LayerMask Floor;
    [SerializeField] LayerMask Pared;
    [SerializeField] float velocidad = 5f;
    [SerializeField] float velocidadj = 5f;
    [SerializeField] float fuerzaPared = 6f;
    [SerializeField] float duracionGolpe = 0.4f;

    int contadorSaltos = 0;
    bool recibiendoGolpe = false;
    int ladoPared = 0; // -1 pared a la izquierda, 1 pared a la derecha

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
        if (recibiendoGolpe) return;

        Moverse();
        if (!SaltoPared()) Saltar();
        ActualizarAnimacion();
    }

    bool TocarSuelo()
    {
        Vector2 pies = new Vector2(cuadradoChoque.bounds.center.x,
                                   cuadradoChoque.bounds.min.y - 0.05f);
        Vector2 tamano = new Vector2(0.05f, 0.05f);

        return Physics2D.OverlapBox(pies, tamano, 0f, Floor) != null;
    }

    bool TocarPared()
    {
        // Comprueba a los dos lados del jugador
        Vector2 centro = cuadradoChoque.bounds.center;
        float mitad = cuadradoChoque.bounds.extents.x + 0.05f;
        Vector2 tamano = new Vector2(0.05f, cuadradoChoque.bounds.size.y * 0.5f);

        bool derecha = Physics2D.OverlapBox(centro + Vector2.right * mitad, tamano, 0f, Pared) != null;
        bool izquierda = Physics2D.OverlapBox(centro + Vector2.left * mitad, tamano, 0f, Pared) != null;

        if (derecha) ladoPared = -1;   // la pared está a la derecha, saltamos hacia la izquierda
        else if (izquierda) ladoPared = 1;

        return derecha || izquierda;
    }

    // Salto de pared: si estamos en el aire tocando una pared y pulsamos salto
    bool SaltoPared()
    {
        if (TocarSuelo() || !TocarPared()) return false;
        if (!saltar.WasPressedThisFrame()) return false;

        jugador.linearVelocity = new Vector2(ladoPared * fuerzaPared, velocidadj);
        contadorSaltos = 1; // permite un doble salto después
        anim.SetInteger("estado", 3);
        return true;
    }

    void Saltar()
    {
        if (!saltar.WasPressedThisFrame()) return;

        if (TocarSuelo() || contadorSaltos < 2)
        {
            jugador.linearVelocity = new Vector2(jugador.linearVelocityX, velocidadj);
            contadorSaltos++;

            // Primer salto -> "salta" (1), segundo salto -> "dobleSalto" (5)
            anim.SetInteger("estado", contadorSaltos == 1 ? 1 : 5);
        }
    }

    void Moverse()
    {
        float c = mover.ReadValue<Vector2>().x;

        jugador.linearVelocity = new Vector2(c * velocidad, jugador.linearVelocityY);

        if (TocarSuelo()) contadorSaltos = 0;
    }

    void ActualizarAnimacion()
    {
        bool enSuelo = TocarSuelo();
        float c = mover.ReadValue<Vector2>().x;

        int estado;

        if (!enSuelo)
        {
            estado = anim.GetInteger("estado");
            if (estado != 1 && estado != 5 && estado != 3) estado = 1;
        }
        else if (Mathf.Abs(c) > 0.1f)
        {
            estado = 2;
        }
        else
        {
            estado = 0;
        }

        anim.SetInteger("estado", estado);
    }

    // Golpe: lo llama el enemigo (tag Enemy) al tocar al jugador
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Golpe();
        }
    }

    public void Golpe()
    {
        if (recibiendoGolpe) return;
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