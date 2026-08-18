using UnityEngine;

public class PortalVictoria : MonoBehaviour
{
    [SerializeField] private GameObject panelVictoria;

    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;

        if (other.GetComponent<PlayerController>() != null)
        {
            activado = true;
            MostrarVictoria();
        }
    }

    private void MostrarVictoria()
    {
        panelVictoria.SetActive(true);
        Time.timeScale = 0f;
    }
}