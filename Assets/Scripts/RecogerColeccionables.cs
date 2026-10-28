using UnityEngine;

public class RecogerColeccionables : MonoBehaviour
{
    Animator anim;
    [SerializeField] AnadirVidasYManzanas estadisticas;
    [SerializeField] float tiempoAntesDeDestruir = 0.2f;

    int manzanasRecogidas = 0;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        manzanasRecogidas++;
        Debug.Log("recogidos " + manzanasRecogidas);

        anim.SetInteger("contacto", 1);

        if (estadisticas != null)
            estadisticas.SumarFruta();

        // Desactiva el collider para que no se recoja dos veces
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, tiempoAntesDeDestruir);
    }
}