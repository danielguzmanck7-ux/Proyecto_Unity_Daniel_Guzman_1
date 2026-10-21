using UnityEngine;
using UnityEngine.InputSystem;

public class MovJugador : MonoBehaviour
{
    // cinebench camara
    [SerializeField]
    LayerMask Floor;
    
    [SerializeField]
    float velocidad = 5f;
    
    [SerializeField]
    float velocidadj = 5f;

    int contadorSaltos = 0;

    Rigidbody2D jugador;
    BoxCollider2D cuadradoChoque;

    InputAction mover;
    InputAction saltar;
    void Start()
    {
        mover = InputSystem.actions.FindAction("Mover");
        saltar = InputSystem.actions.FindAction("Saltar");
        jugador = GetComponent<Rigidbody2D>();
        cuadradoChoque = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        Moverse();
        Saltar();
    }

    bool TocarSuelo()
    {
        Vector2 pies = new Vector2(cuadradoChoque.bounds.center.x
        ,cuadradoChoque.bounds.min.y - 0.05f);
        
        Vector2 tamano = new Vector2(0.05f,0.05f);

        return Physics2D.OverlapBox(pies,tamano,0f,Floor) != null;
    }

    void Saltar()
    {
        if(!saltar.WasPressedThisFrame())return;

        if(TocarSuelo() || contadorSaltos < 2)
        {
            jugador.linearVelocity = new Vector2(
                jugador.linearVelocityX,
                velocidadj
            );
            contadorSaltos++;
        }
    }
    void Moverse()
    {
        float c = mover.ReadValue<Vector2>().x;

        jugador.linearVelocity = new Vector2(
                c * velocidad,
                jugador.linearVelocityY
            );

        if(TocarSuelo())contadorSaltos=0;
        
    }
}
