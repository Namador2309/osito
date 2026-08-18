using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ProyectilJefe : MonoBehaviour
{
    [SerializeField] private float velocidad = 8.5f;
    [SerializeField] private int danio = 1;
    [SerializeField] private float tiempoVida = 4f;

    private Rigidbody2D rb;
    private Vector2 direccion = Vector2.left;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    public void Inicializar(Vector2 nuevaDireccion)
    {
        if (nuevaDireccion.sqrMagnitude > 0f)
        {
            direccion = nuevaDireccion.normalized;
        }

        // El sprite original apunta hacia la izquierda. Esta rotacion hace que
        // la punta del proyectil mire siempre hacia el jugador.
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg - 180f;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evita que el disparo choque con el propio jefe u otros enemigos.
        if (collision.CompareTag("Enemigo"))
        {
            return;
        }

        PlayerController jugador = collision.GetComponentInParent<PlayerController>();
        if (jugador != null)
        {
            if (jugador.enabled)
            {
                Vector2 origenDanio = new Vector2(transform.position.x, transform.position.y);
                jugador.recibedanio(origenDanio, danio);
            }

            Destroy(gameObject);
            return;
        }

        // El proyectil desaparece al tocar paredes, piso u otras superficies.
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
