using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparoJugador : MonoBehaviour
{
    [Header("Disparo jugador")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private GameObject balaPrefab;
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoLaser;

    [Header("Referencia del jugador")]
    [SerializeField] private Transform jugador;

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

    private void Disparar()
    {
        if (balaPrefab == null || puntoDisparo == null || jugador == null)
            return;

        // Sonido del láser
        if (audioSource != null && sonidoLaser != null)
        {
            audioSource.PlayOneShot(sonidoLaser);
        }

        GameObject nuevaBala = Instantiate(
            balaPrefab,
            puntoDisparo.position,
            Quaternion.identity
        );

        Bala bala = nuevaBala.GetComponent<Bala>();

        if (bala != null)
        {
            if (jugador.localScale.x > 0)
            {
                bala.Inicializar(Vector2.right);
            }
            else
            {
                bala.Inicializar(Vector2.left);
            }
        }
    }
}