using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    [SerializeField] private float velocidad;
    [SerializeField] private float daño;

    private void Update()
    {
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            EnemyController enemigo = collision.GetComponent<EnemyController>();
            if (enemigo != null)
            {
                enemigo.RecibirDaño(daño);
            }
            Destroy(gameObject);
        }
    }

}
