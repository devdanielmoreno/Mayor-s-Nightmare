using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool enSupervivencia = false;
    void Awake()
    {
        instance = this;
    }

    public void IniciarSupervivencia()
    {
        enSupervivencia = true;
        SceneManager.LoadScene("SupervivenciaScene");
    }

    public void FinalizarSupervivencia()
    {
        enSupervivencia = false;
        SceneManager.LoadScene("JuegoScene");
    }
    
}