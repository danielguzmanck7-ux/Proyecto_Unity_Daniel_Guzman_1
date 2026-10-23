using UnityEngine;

public class MovAlanteAtras : MonoBehaviour
{
    [SerializeField]
    float velocidad = 3.3f;
    int direccion = 1;
    
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {        
        transform.Translate(new Vector3(direccion * velocidad * Time.deltaTime, 0f, 0f));
        anim.SetInteger("estado",0);

        if (transform.position.x > 9f)
        {
            direccion = -1;
        }
        else if (transform.position.x < -9f)
        {
            direccion = 1;
        }      
        
    }
}
