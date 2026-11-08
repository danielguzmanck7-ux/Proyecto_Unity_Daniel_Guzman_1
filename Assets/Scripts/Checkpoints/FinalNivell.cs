using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalNivel : MonoBehaviour
{
    [Tooltip("Leave empty to load the next scene in the Build Settings order")]
    [SerializeField] string nombreEscenaSiguiente = "";

    bool cargando = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (cargando || !collision.CompareTag("Player")) return;
        cargando = true;

        if (!string.IsNullOrEmpty(nombreEscenaSiguiente))
        {
            SceneManager.LoadScene(nombreEscenaSiguiente);
            return;
        }

        int siguiente = SceneManager.GetActiveScene().buildIndex + 1;

        if (siguiente < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguiente);
        }
        else
        {
            Debug.LogWarning("No hay más escenas en el Build Settings.");
            cargando = false;
        }
    }
}