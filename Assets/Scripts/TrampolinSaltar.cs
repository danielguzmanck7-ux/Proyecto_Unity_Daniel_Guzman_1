using System.Collections;
using UnityEngine;

public class TrampolinSaltar : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float tiempoEncendido = 5f;
    [SerializeField] float tiempoApagado = 5f;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
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
        anim.SetInteger("salto", 1);
        if (CompareTag("Player"))
        {
            
        }
    }

    public void Apagar()
    {
        anim.SetInteger("salto", 0);
    }
}
