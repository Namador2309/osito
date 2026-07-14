using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("Danio")]
    public int cantidadDanio = 1;
    public bool matarInstantaneamente = false; // si es true, resta toda la vida actual

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            int danio = matarInstantaneamente ? player.vidaActual : cantidadDanio;

            player.recibedanio(direccionDanio, danio);
            Debug.Log("El jugador pisó el slime ácido :(");
        }
    }
}