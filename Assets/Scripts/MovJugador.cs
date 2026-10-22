using UnityEngine;
using UnityEngine.InputSystem;

public class MovJugador : MonoBehaviour
{
    [SerializeField] LayerMask Floor;
    [SerializeField] LayerMask Pared;
    [SerializeField] float velocidad = 5f;
    [SerializeField] float velocidadj = 5f;

    int contadorSaltos = 0;

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
        Moverse();
        Saltar();
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
        Vector2 pies = new Vector2(cuadradoChoque.bounds.center.x,
                                   cuadradoChoque.bounds.min.y - 0.05f);
        Vector2 tamano = new Vector2(0.05f, 0.05f);

        return Physics2D.OverlapBox(pies, tamano, 0f, Pared) != null;
    }

    void Saltar()
    {
        if (!saltar.WasPressedThisFrame()) return;

        if (TocarSuelo() || contadorSaltos < 2)
        {
            jugador.linearVelocity = new Vector2(jugador.linearVelocityX, velocidadj);
            contadorSaltos++;

            // Primer salto desde el suelo o en el aire -> "salta" (1)
            // Segundo salto -> "dobleSalto" (5)
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
            // En el aire, el estado de salto se mantiene (1 o 5)
            // Si ya estaba en salto/doble salto, no lo cambiamos aquí
            estado = anim.GetInteger("estado");
            if (estado != 1 && estado != 5 && estado != 3) estado = 1;
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
}