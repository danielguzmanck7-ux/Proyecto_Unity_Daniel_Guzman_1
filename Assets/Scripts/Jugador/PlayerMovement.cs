using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Capas")]
    [SerializeField] LayerMask Floor;

    [Header("Caída")]
    [SerializeField] float limiteY = -10f;   // below this height the player dies

    [Header("Movimiento")]
    [SerializeField] float velocidad = 5f;
    [SerializeField] float velocidadj = 5f;

    // Read by PlayerAnimation
    public float Direccion { get; private set; }
    public int SaltosUsados => contadorSaltos;
    public float VelocidadY => jugador.linearVelocity.y;
    public bool EnSuelo => TocarSuelo() && jugador.linearVelocity.y <= 0.01f;

    int contadorSaltos = 0;

    Rigidbody2D jugador;
    BoxCollider2D cuadradoChoque;
    SpriteRenderer sprite;
    PlayerHealth salud;

    InputAction mover;
    InputAction saltar;

    void Awake()
    {
        jugador = GetComponent<Rigidbody2D>();
        cuadradoChoque = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        salud = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        mover = InputSystem.actions.FindAction("Mover");
        saltar = InputSystem.actions.FindAction("Saltar");
    }

    void Update()
    {
        if (!salud.PuedeActuar) return;

        if (transform.position.y < limiteY)
        {
            salud.Morir();
            return;
        }

        Moverse();
        Saltar();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-1000f, limiteY, 0f), new Vector3(1000f, limiteY, 0f));
    }

    bool TocarSuelo()
    {
        Bounds bounds = cuadradoChoque.bounds;

        Vector2 pies = new Vector2(bounds.center.x, bounds.min.y - 0.02f);
        Vector2 tamano = new Vector2(bounds.size.x * 0.8f, 0.08f);

        return Physics2D.OverlapBox(pies, tamano, 0f, Floor) != null;
    }

    void Moverse()
    {
        Direccion = mover.ReadValue<Vector2>().x;

        jugador.linearVelocity = new Vector2(Direccion * velocidad, jugador.linearVelocity.y);

        // Flip the sprite
        if (Direccion > 0.01f) sprite.flipX = false;
        else if (Direccion < -0.01f) sprite.flipX = true;

        if (EnSuelo)
        {
            contadorSaltos = 0;
        }
        else if (!TocarSuelo() && contadorSaltos == 0)
        {
            // Walking off a ledge counts as the first jump
            contadorSaltos = 1;
        }
    }

    void Saltar()
    {
        if (!saltar.WasPressedThisFrame()) return;
        if (contadorSaltos >= 2) return;

        jugador.linearVelocity = new Vector2(jugador.linearVelocity.x, velocidadj);
        contadorSaltos++;
    }

    // Called by the trampoline
    public void Rebotar(float fuerza)
    {
        if (salud.Muerto) return;

        jugador.linearVelocity = new Vector2(jugador.linearVelocity.x, fuerza);
        contadorSaltos = 1;
    }

    // Called by PlayerHealth when respawning
    public void ReiniciarSaltos()
    {
        contadorSaltos = 0;
    }
}