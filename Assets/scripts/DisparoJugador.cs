using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparoJugador : MonoBehaviour
{ 

  [Header("Disparo jugador")]
  [SerializeField] private Transform puntoDisparo;
  [SerializeField] private GameObject balaPrefab;
  [SerializeField] private Animator animator;
   private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        // Disparo del jugador-update
        if (Input.GetButtonDown("Fire1"))
        {
            Disparar();
        }
        animator.SetBool("Disparando", Input.GetButton("Fire1"));

    }

        //DISPARO DEL JUGADOR
        private void Disparar()
{

    if (balaPrefab != null && puntoDisparo != null)
    {
        Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
            
    }
}
   
        

}
