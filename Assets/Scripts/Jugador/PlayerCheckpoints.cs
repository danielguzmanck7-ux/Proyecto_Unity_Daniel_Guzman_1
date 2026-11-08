using UnityEngine;

public class PlayerCheckpoints : MonoBehaviour
{
    [SerializeField] string tagCheckpoint = "Checkpoint";
    [SerializeField] Vector2 desplazamiento = Vector2.zero;  // raise this if you respawn inside the floor

    public Vector3 PuntoRespawn { get; private set; }

    void Awake()
    {
        // Until a checkpoint is touched, respawn at the level start
        PuntoRespawn = transform.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagCheckpoint)) return;

        PuntoRespawn = collision.transform.position + (Vector3)desplazamiento;
    }
}
