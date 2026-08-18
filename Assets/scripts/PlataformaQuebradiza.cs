using System.Collections;
using UnityEngine;

public class PlataformaQuebradiza : MonoBehaviour
{
    [Header("Tiempos")]
    [SerializeField] private float tiempoAntesDeCaer = 0.5f;
    [SerializeField] private float tiempoAntesDeRegenerar = 2f;

    private Rigidbody2D rb;
    private Vector3 posicionInicial;
    private bool activada = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        posicionInicial = transform.position;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activada) return;

        if (other.GetComponent<PlayerController>() != null)
        {
            activada = true;
            StartCoroutine(CaerYRegenerar());
        }
    }

    private IEnumerator CaerYRegenerar()
    {
        yield return new WaitForSeconds(tiempoAntesDeCaer);

        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(tiempoAntesDeRegenerar);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        transform.position = posicionInicial;
        activada = false;
    }
}