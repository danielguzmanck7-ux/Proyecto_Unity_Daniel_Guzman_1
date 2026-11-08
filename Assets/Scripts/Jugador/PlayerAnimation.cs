using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    enum Estado
    {
        Estatico = 0,
        Saltar = 1,
        Correr = 2,
        Pegar = 4,
        DobleSalto = 5,
        Caer = 6,
        Dissappearing = 7,
        Appearing = 8
    }

    [SerializeField] float tiempoMaximoEspera = 5f; // safety limit per wait

    Animator anim;
    PlayerMovement movimiento;
    PlayerHealth salud;
    int ultimoEstado = -1;

    void Awake()
    {
        anim = GetComponent<Animator>();
        movimiento = GetComponent<PlayerMovement>();
        salud = GetComponent<PlayerHealth>();
    }

    // LateUpdate runs after PlayerMovement.Update, so the state is always up to date
    void LateUpdate()
    {
        int estado = (int)CalcularEstado();
        if (estado == ultimoEstado) return;

        ultimoEstado = estado;
        anim.SetInteger("estado", estado);
    }

    Estado CalcularEstado()
    {
        switch (salud.Fase)
        {
            case FaseSalud.Golpeado:    return Estado.Pegar;
            case FaseSalud.Muriendo:    return Estado.Dissappearing;
            case FaseSalud.Apareciendo: return Estado.Appearing;
            case FaseSalud.Despertando: return Estado.Estatico;
        }

        if (!movimiento.EnSuelo)
        {
            if (movimiento.VelocidadY < -0.1f) return Estado.Caer;
            return movimiento.SaltosUsados >= 2 ? Estado.DobleSalto : Estado.Saltar;
        }

        return Mathf.Abs(movimiento.Direccion) > 0.1f ? Estado.Correr : Estado.Estatico;
    }

    // Waits until the Animator enters the state and the clip has finished
    public IEnumerator EsperarAnimacion(string nombreEstado, float fraccion = 1f)
{
    float limite = Time.time + tiempoMaximoEspera;
    while (!anim.GetCurrentAnimatorStateInfo(0).IsName(nombreEstado) && Time.time < limite)
        yield return null;

    limite = Time.time + tiempoMaximoEspera;
    while (anim.GetCurrentAnimatorStateInfo(0).IsName(nombreEstado) &&
           anim.GetCurrentAnimatorStateInfo(0).normalizedTime < fraccion &&
           Time.time < limite)
        yield return null;
}

    public IEnumerator EsperarEstado(string nombreEstado)
    {
        float limite = Time.time + tiempoMaximoEspera;
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(nombreEstado) && Time.time < limite)
            yield return null;
    }
}