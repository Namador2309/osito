using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar escenas

public class MenuGameOver : MonoBehaviour
{
    public void ReiniciarNivel()
    {
        Debug.Log("Botón de reiniciar presionado");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Esto cerrará el juego compilado (.exe)
    }
}