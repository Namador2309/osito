using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    [SerializeField] private float velocidad;
    [SerializeField] private float daño;

    private Vector2 direccion = Vector2.right;

    public void Inicializar(Vector2 nuevaDireccion)
    {
        direccion = nuevaDireccion.normalized;
    }

    private void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            EnemyController enemigo = collision.GetComponent<EnemyController>();
            BossController jefe = collision.GetComponent<BossController>();

            if (jefe != null)
            {
                jefe.RecibirDanio(daño);
            }
            else if (enemigo != null)
            {
                enemigo.RecibirDaño(daño);
            }

            Destroy(gameObject);
        }
    }
}
