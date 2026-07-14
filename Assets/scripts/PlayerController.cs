using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad;
    public float jumpForce = 10f;

    [Header("Deteccion de suelo")]
    public float longitudRaycast = 0.1f;
    public LayerMask suelo;

    [Header("Vida y danio")]
    public int vidaMaxima = 5;
    public int vidaActual;
    public float tiempoInvencibilidad = 1f;   // duracion del momento de recuperacion
    public float fuerzaRebote = 5f;
    public float parpadeoIntervalo = 0.1f;    // opcional: parpadeo visual al recibir danio

    private Rigidbody2D rigidBody;
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private bool recibiendoDanio;
    private bool invencible;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vidaActual = vidaMaxima;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        CheckGrounded();

        // Mientras esta recibiendo danio, se bloquea el movimiento
        // horizontal para que solo aplique el rebote/knockback.
        if (!recibiendoDanio)
        {
            ProcesarMovimiento();
            ProcesarSalto();
        }

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("recibedanio", recibiendoDanio);
    }

    void ProcesarMovimiento()
    {
        float inputMovimiento = Input.GetAxis("Horizontal");
        animator.SetFloat("movement", inputMovimiento);
        rigidBody.velocity = new Vector2(inputMovimiento * velocidad, rigidBody.velocity.y);
        gestionarOrientacion();
    }

    void ProcesarSalto()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
        }
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, suelo);
        isGrounded = hit.collider != null;
        Debug.DrawRay(transform.position, Vector2.down * longitudRaycast, isGrounded ? Color.green : Color.red);
    }

    void gestionarOrientacion()
    {
        if (rigidBody.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (rigidBody.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void recibedanio(Vector2 direccion, int cantDanio)
    {
        // Si esta invencible (en su periodo de recuperacion), ignora
        // cualquier danio adicional.
        if (invencible) return;

        vidaActual -= cantDanio;
        recibiendoDanio = true;
        invencible = true;

        // Reinicia la velocidad vertical/horizontal antes de aplicar el
        // rebote, para que el knockback sea consistente.
        rigidBody.velocity = Vector2.zero;
        Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
        rigidBody.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(RecuperacionDanio());
        }
    }

    private IEnumerator RecuperacionDanio()
    {
        // Parpadeo visual opcional mientras dura la invencibilidad
        if (spriteRenderer != null)
        {
            float tiempoTranscurrido = 0f;
            while (tiempoTranscurrido < tiempoInvencibilidad)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(parpadeoIntervalo);
                tiempoTranscurrido += parpadeoIntervalo;
            }
            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(tiempoInvencibilidad);
        }

        terminaRecibirDanio();
        invencible = false;
    }

    public void terminaRecibirDanio()
    {
        recibiendoDanio = false;
    }

    private void Morir()
    {
        Debug.Log("El jugador ha muerto");
        recibiendoDanio = false;
        animator.SetBool("recibedanio", false);
        animator.SetBool("isGrounded", true); // evitar que se quede en el estado de salto si muere en el aire
        animator.SetTrigger("morir");
        this.enabled = false;

        //probablemente agregar la pantalla de Game Over o reiniciar el nivel acá
    }
}
