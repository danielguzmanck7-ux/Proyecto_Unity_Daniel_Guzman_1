using UnityEngine;

public class CamaraSigueJugador : MonoBehaviour
{
    [SerializeField] Transform objetivo;                       // leave empty to find the object tagged Player
    [SerializeField] Vector2 desplazamiento = new Vector2(0f, 1f);
    [SerializeField] float suavizado = 0.15f;                  // lower = snappier, higher = floatier
    [SerializeField] float distanciaTeletransporte = 12f;      // farther than this, the camera jumps instead of sliding

    [Header("Límites del nivel (opcional)")]
    [SerializeField] bool usarLimites = false;
    [SerializeField] Vector2 limiteMinimo = new Vector2(-20f, -10f);
    [SerializeField] Vector2 limiteMaximo = new Vector2(20f, 10f);

    Vector3 velocidad;
    float z;

    void Start()
    {
        z = transform.position.z;

        if (objetivo == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null) objetivo = jugador.transform;
        }

        if (objetivo != null)
            transform.position = Destino();   // start already centred on the player
    }

    // LateUpdate runs after the player has moved, which avoids jitter
    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = Destino();

        if (Vector2.Distance(transform.position, destino) > distanciaTeletransporte)
        {
            transform.position = destino;
            velocidad = Vector3.zero;
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, destino, ref velocidad, suavizado);
    }

    Vector3 Destino()
    {
        float x = objetivo.position.x + desplazamiento.x;
        float y = objetivo.position.y + desplazamiento.y;

        if (usarLimites)
        {
            x = Mathf.Clamp(x, limiteMinimo.x, limiteMaximo.x);
            y = Mathf.Clamp(y, limiteMinimo.y, limiteMaximo.y);
        }

        return new Vector3(x, y, z);
    }
}