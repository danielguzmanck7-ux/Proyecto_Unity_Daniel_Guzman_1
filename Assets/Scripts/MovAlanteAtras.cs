using UnityEngine;

public class MovAlanteAtras : MonoBehaviour
{
    [SerializeField] float velocidad = 3.3f;
    [SerializeField] LayerMask Pared;
    [SerializeField] float distanciaDeteccionPared = 0.1f;

    BoxCollider2D cajaChoque;
    Rigidbody2D enemigo;
    Animator anim;
    int direccion = 0;
    void Start()
    {
        anim = GetComponent<Animator>();
        enemigo = GetComponent<Rigidbody2D>();
        cajaChoque = GetComponent<BoxCollider2D>();
    }

    void Update()
    {        
        transform.Translate(new Vector3(direccion * velocidad * Time.deltaTime, 0f, 0f));        

        if (enemigo.linearVelocityX == 0)
        {
            anim.SetInteger("estado",0);
        }
        else 
        {
            anim.SetInteger("estado",1);
        }

        cajaChoque.bounds.Expand(0.02f);
        
    }
}
