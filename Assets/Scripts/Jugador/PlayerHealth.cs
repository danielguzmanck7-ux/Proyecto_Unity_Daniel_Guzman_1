using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FaseSalud { Normal, Golpeado, Muriendo, Apareciendo, Despertando }

public class PlayerHealth : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] AnadirVidasYManzanas estadisticas;

    [Header("Golpe")]
    [SerializeField] float duracionGolpe = 0.4f;          // hit reaction before disappearing (0 = skip)

    [Header("Invulnerabilidad tras reaparecer")]
    [SerializeField] float duracionInvulnerable = 1f;

    [Header("Escena de muerte")]
    [SerializeField] string escenaMuerte = "Muerte";

    public FaseSalud Fase { get; private set; } = FaseSalud.Normal;

    // True only when the player can move and jump
    public bool PuedeActuar => Fase == FaseSalud.Normal;

    // True during the whole hit, disappear and respawn sequence
    public bool Muerto => Fase == FaseSalud.Muriendo
                       || Fase == FaseSalud.Apareciendo
                       || Fase == FaseSalud.Despertando;

    bool invulnerable = false;
    Coroutine rutinaInvulnerable;
    float gravedadOriginal;

    Rigidbody2D jugador;
    SpriteRenderer sprite;
    PlayerMovement movimiento;
    PlayerAnimation animacion;
    PlayerCheckpoints checkpoints;

    void Awake()
    {
        jugador = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        movimiento = GetComponent<PlayerMovement>();
        animacion = GetComponent<PlayerAnimation>();
        checkpoints = GetComponent<PlayerCheckpoints>();
    }

    void Start()
    {
        gravedadOriginal = jugador.gravityScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Trap"))
        {
            Golpe(collision.transform.position);
        }
    }

    // The parameter is kept so other scripts (e.g. TrampaSubeBaja) can still call Golpe(position)
    public void Golpe(Vector2 origenGolpe)
    {
        if (Fase != FaseSalud.Normal || invulnerable) return;

        estadisticas.RestarVida();
        StartCoroutine(GolpeYReaparecer());
    }

    // Instant death (e.g. falling off the map): costs a life like any hit, even while invulnerable
    public void Morir()
    {
        if (Fase != FaseSalud.Normal) return;

        estadisticas.RestarVida();
        StartCoroutine(GolpeYReaparecer());
    }

    IEnumerator GolpeYReaparecer()
    {
        // 1. Hit reaction, frozen in place
        Fase = FaseSalud.Golpeado;
        if (rutinaInvulnerable != null) StopCoroutine(rutinaInvulnerable);   // stop any blink in progress
        invulnerable = false;
        sprite.enabled = true;
        jugador.linearVelocity = Vector2.zero;
        jugador.gravityScale = 0f;

        yield return new WaitForSeconds(duracionGolpe);

        // 2. Disappear
        Fase = FaseSalud.Muriendo;
        yield return animacion.EsperarAnimacion("Dissappearing");

        // Out of lives: go to the death scene
        if (estadisticas.Vidas <= 0)
        {
            // Saved so the "Reiniciar nivel" button can reload this level
            PlayerPrefs.SetString("UltimoNivel", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(escenaMuerte);
            yield break;
        }

        // 3. Back to the last checkpoint
        transform.position = checkpoints.PuntoRespawn;
        jugador.linearVelocity = Vector2.zero;
        movimiento.ReiniciarSaltos();

        Fase = FaseSalud.Apareciendo;
        yield return animacion.EsperarAnimacion("Appearing");

        // Appearing only exits to Estatico, so hold estado 0 until the Animator gets there
        Fase = FaseSalud.Despertando;
        yield return animacion.EsperarEstado("Estatico");

        jugador.gravityScale = gravedadOriginal;
        Fase = FaseSalud.Normal;

        rutinaInvulnerable = StartCoroutine(Invulnerabilidad());
    }

    // Short protection after respawning, so a trap next to the checkpoint can't chain-hit
    IEnumerator Invulnerabilidad()
    {
        invulnerable = true;

        float fin = Time.time + duracionInvulnerable;
        while (Time.time < fin)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        sprite.enabled = true;
        invulnerable = false;
    }
}