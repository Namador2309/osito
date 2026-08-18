using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class BossController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;

    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 12f;

    [Header("Movimiento")]
    [SerializeField] private float detectionRadius = 18f;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float distanciaMinima = 2.2f;

    [Header("Combate")]
    [SerializeField] private int danioContacto = 1;
    [SerializeField] private float tiempoEntreGolpes = 1f;

    [Header("Disparo por el ojo")]
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private float intervaloDisparo = 1.7f;
    [SerializeField] private float distanciaMinimaDisparo = 4f;
    [SerializeField] private float distanciaMaximaDisparo = 15f;
    [SerializeField] private Vector2 offsetOjo = new Vector2(-1.55f, 3.1f);
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoDisparo;

    [Header("Animaciones")]
    [SerializeField] private float duracionIntroduccion = 1.4f;
    [SerializeField] private float tiempoAntesDeDestruir = 1f;

    private Rigidbody2D rb;
    private Collider2D bossCollider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    private float vidaActual;
    private float direccionMovimiento;
    private float proximoGolpePermitido;
    private float proximoDisparoPermitido;
    private bool combateActivo;
    private bool estaMuerto;
    private bool reproduciendoIntroduccion;

    private Vector3 posicionCombate;
    private Vector3 escalaCombate;
    private Vector3 posicionMuerte;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (player == null)
        {
            GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
            if (objetoJugador != null)
            {
                player = objetoJugador.transform;
            }
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        vidaActual = vidaMaxima;
        posicionCombate = transform.position;
        escalaCombate = transform.localScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        StartCoroutine(ReproducirIntroduccion());
    }

    private IEnumerator ReproducirIntroduccion()
    {
        combateActivo = false;
        direccionMovimiento = 0f;
        reproduciendoIntroduccion = true;
        bossCollider.enabled = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Se mantiene el primer cuadro hasta que el jugador se acerque para
        // evitar que toda la introduccion ocurra fuera de camara.
        animator.Play("boss-inicio", 0, 0f);
        animator.speed = 0f;

        while (player != null && Vector2.Distance(transform.position, player.position) > detectionRadius)
        {
            yield return null;
        }

        // El ojo abriendose es la introduccion existente del jefe.
        animator.speed = 1f;
        yield return new WaitForSeconds(duracionIntroduccion);

        if (estaMuerto)
        {
            yield break;
        }

        // La animacion de inicio modifica posicion y escala. Se restauran antes
        // de mostrar el cuerpo completo para que el jefe quede en su arena.
        transform.position = posicionCombate;
        transform.localScale = escalaCombate;
        rb.position = new Vector2(posicionCombate.x, posicionCombate.y);

        reproduciendoIntroduccion = false;
        animator.Play("boss-default", 0, 0f);
        rb.bodyType = RigidbodyType2D.Dynamic;
        bossCollider.enabled = true;
        combateActivo = true;
        proximoDisparoPermitido = Time.time + 0.6f;
    }

    private void Update()
    {
        if (!combateActivo || estaMuerto || player == null)
        {
            direccionMovimiento = 0f;
            return;
        }

        if (playerController != null && !playerController.enabled)
        {
            direccionMovimiento = 0f;
            return;
        }

        float distancia = Vector2.Distance(transform.position, player.position);
        float distanciaHorizontal = Mathf.Abs(player.position.x - transform.position.x);

        if (distancia > detectionRadius)
        {
            direccionMovimiento = 0f;
            return;
        }

        float direccionJugador = Mathf.Sign(player.position.x - transform.position.x);
        OrientarHaciaJugador(direccionJugador);

        bool estaEnRangoDeDisparo = proyectilPrefab != null
            && distancia >= distanciaMinimaDisparo
            && distancia <= distanciaMaximaDisparo;

        if (estaEnRangoDeDisparo)
        {
            direccionMovimiento = 0f;
            IntentarDisparar(direccionJugador);
        }
        else if (distanciaHorizontal > distanciaMinima)
        {
            direccionMovimiento = direccionJugador;
        }
        else
        {
            direccionMovimiento = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!combateActivo || estaMuerto || rb.bodyType != RigidbodyType2D.Dynamic)
        {
            return;
        }

        rb.velocity = new Vector2(direccionMovimiento * speed, rb.velocity.y);
    }

    private void IntentarDisparar(float direccionJugador)
    {
        if (Time.time < proximoDisparoPermitido || player == null)
        {
            return;
        }

        float ladoOjo = direccionJugador >= 0f ? 1f : -1f;
        Vector3 offsetLocal = new Vector3(Mathf.Abs(offsetOjo.x) * ladoOjo, offsetOjo.y, 0f);
        Vector3 origenDisparo = transform.TransformPoint(offsetLocal);
        Vector2 objetivo = (Vector2)player.position + Vector2.up * 0.25f;
        Vector2 direccionDisparo = (objetivo - (Vector2)origenDisparo).normalized;

        GameObject nuevoProyectil = Instantiate(proyectilPrefab, origenDisparo, Quaternion.identity);
        ProyectilJefe proyectil = nuevoProyectil.GetComponent<ProyectilJefe>();

        if (proyectil != null)
        {
            proyectil.Inicializar(direccionDisparo);
        }

        if (audioSource != null && sonidoDisparo != null)
        {
            audioSource.PlayOneShot(sonidoDisparo);
        }

        proximoDisparoPermitido = Time.time + intervaloDisparo;
    }

    private void LateUpdate()
    {
        // boss-inicio tambien tenia una posicion absoluta grabada. El ojo debe
        // abrirse en la posicion real del jefe, no desplazarse por la escena.
        if (reproduciendoIntroduccion)
        {
            transform.position = posicionCombate;
        }

        // boss-death tenia posiciones absolutas grabadas. Mantener esta posicion
        // evita que el jefe se teletransporte al morir despues de perseguir.
        else if (estaMuerto)
        {
            transform.position = posicionMuerte;
        }
    }

    private void OrientarHaciaJugador(float direccion)
    {
        if (spriteRenderer == null || direccion == 0f)
        {
            return;
        }

        // El dibujo original mira hacia la izquierda.
        spriteRenderer.flipX = direccion > 0f;
    }

    public void RecibirDanio(float cantidad)
    {
        if (!combateActivo || estaMuerto || cantidad <= 0f)
        {
            return;
        }

        vidaActual -= cantidad;

        if (vidaActual <= 0f)
        {
            Morir();
        }
        else
        {
            StartCoroutine(MostrarDanio());
        }
    }

    private IEnumerator MostrarDanio()
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        spriteRenderer.color = new Color(1f, 0.45f, 0.45f, 1f);
        yield return new WaitForSeconds(0.12f);

        if (!estaMuerto)
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void Morir()
    {
        estaMuerto = true;
        combateActivo = false;
        direccionMovimiento = 0f;
        posicionMuerte = transform.position;

        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        bossCollider.enabled = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        animator.Play("boss-death", 0, 0f);
        Destroy(gameObject, tiempoAntesDeDestruir);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!combateActivo || estaMuerto || Time.time < proximoGolpePermitido)
        {
            return;
        }

        PlayerController jugadorGolpeado = collision.gameObject.GetComponent<PlayerController>();
        if (jugadorGolpeado == null || !jugadorGolpeado.enabled)
        {
            return;
        }

        proximoGolpePermitido = Time.time + tiempoEntreGolpes;
        Vector2 origenDanio = new Vector2(transform.position.x, transform.position.y);
        jugadorGolpeado.recibedanio(origenDanio, danioContacto);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
