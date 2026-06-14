using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float velocidad;
    private Rigidbody2D rigidBody;
    public Animator animator;
    public float jumpForce = 10f;
    public float longitudRaycast = 0.1f;
    public LayerMask capaSuelo;

    private bool isGrounded;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGrounded();
        ProcesarMovimiento();
        ProcesarSalto();
        // Actualiza solo el Bool del Animator
        animator.SetBool("isGrounded", isGrounded);
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
            // Ya no usamos Trigger; el Animator reaccionará a isGrounded = false
        }
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
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
}
