using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5f;
    public float speed = 2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private PlayerController playerController;
    private Animator animator;

    //daño a enemigo
    [SerializeField] private float vida;
    [SerializeField] private float tiempoAntesDeDestruir = 1f; // duracion aprox de la animacion de muerte
    private bool estaMuerto = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (estaMuerto) return; // evita que se siga moviendo/orientando tras morir

        // Si el jugador ya murio (PlayerController se desactiva en Morir()),
        // el enemigo deja de perseguir y de empujar el cadaver.
        if (playerController != null && !playerController.enabled)
        {
            movement = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            movement = new Vector2(direction.x, 0);
            gestionarOrientacion(direction.x);
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (estaMuerto) return;
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void gestionarOrientacion(float direccionX)
    {
        if (direccionX > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direccionX < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Para dibujar el radio de deteccion en la escena
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void RecibirDaño(float daño)
    {
        if (estaMuerto) return;

        vida -= daño;
        if (vida <= 0)
        {
            Muerte();
        }
    }

    private void Muerte()
    {
        estaMuerto = true;
        movement = Vector2.zero;

        // Desactiva la fisica para que no siga reaccionando a colisiones
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("morir"); // usa el nombre exacto del trigger en tu Animator
        }

        Destroy(gameObject, tiempoAntesDeDestruir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estaMuerto) return;
        if (playerController != null && !playerController.enabled) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            collision.gameObject.GetComponent<PlayerController>().recibedanio(direccionDanio, 1);
            Debug.Log("El enemigo ha chocado con el jugador");
        }
    }
}