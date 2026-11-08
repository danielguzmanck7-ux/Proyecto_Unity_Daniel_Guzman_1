using System.Collections;
using UnityEngine;

public class EncenderApagarFuego : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Collider2D colisionFuego;   // the collider that hurts
    [SerializeField] float tiempoEncendido = 5f;
    [SerializeField] float tiempoApagado = 5f;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (colisionFuego == null) colisionFuego = GetComponent<Collider2D>();
        StartCoroutine(Ciclo());
    }

    IEnumerator Ciclo()
    {
        while (true)
        {
            Encender();
            yield return new WaitForSeconds(tiempoEncendido);

            Apagar();
            yield return new WaitForSeconds(tiempoApagado);
        }
    }

    public void Encender()
    {
        anim.SetInteger("encendido", 1);
        colisionFuego.enabled = true;
    }

    public void Apagar()
    {
        anim.SetInteger("encendido", 0);
        colisionFuego.enabled = false;
    }
}