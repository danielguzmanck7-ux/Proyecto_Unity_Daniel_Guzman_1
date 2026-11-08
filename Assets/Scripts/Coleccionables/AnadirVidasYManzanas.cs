using TMPro;
using UnityEngine;

public class AnadirVidasYManzanas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textoFrutas;
    [SerializeField] TextMeshProUGUI textoVidas;
    [SerializeField] int vidasIniciales = 3;
    [SerializeField] int manzanasPorVida = 3;

    int contadorV;
    int contadorF = 0;

    public int Vidas => contadorV;

    void Awake()
    {
        contadorV = vidasIniciales;
    }

    void Start() => ActualizarTextos();

    public void SumarFruta()
    {
        contadorF++;
        Debug.Log($"Frutas: {contadorF}, por vida: {manzanasPorVida}, vidas: {contadorV}");

        if (contadorF % manzanasPorVida == 0)
        {
            contadorF = 0;
            contadorV++;
        }

        ActualizarTextos();
    }

    public void RestarVida() { contadorV--; ActualizarTextos(); }
    public void SumarVida()  { contadorV++; ActualizarTextos(); }
    public void ReiniciarVidas() { contadorV = vidasIniciales; ActualizarTextos(); }

    void ActualizarTextos()
    {
        textoFrutas.text = contadorF.ToString();
        textoVidas.text = contadorV.ToString();
    }
}