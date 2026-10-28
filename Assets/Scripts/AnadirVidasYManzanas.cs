using TMPro;
using UnityEngine;

public class AnadirVidasYManzanas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textoFrutas;
    [SerializeField] TextMeshProUGUI textoVidas;

    int contadorV = 3;
    int contadorF = 0;

    void Start()
    {
        ActualizarTextos();
    }

    public void SumarFruta()
    {
        contadorF++;
        ActualizarTextos();
    }

    public void RestarVida()
    {
        contadorV--;
        ActualizarTextos();
    }

    public void SumarVida()
    {
        contadorV++;
        ActualizarTextos();
    }

    void ActualizarTextos()
    {
        textoFrutas.text = contadorF.ToString();
        textoVidas.text = contadorV.ToString();
    }
}