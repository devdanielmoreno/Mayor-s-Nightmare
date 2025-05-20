using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    private bool enemigoAtacando = false;

    public bool PuedeAtacar()
    {
        return !enemigoAtacando;
    }

    public void NotificarInicioAtaque()
    {
        enemigoAtacando = true;
    }

    public void NotificarFinAtaque()
    {
        enemigoAtacando = false;
    }
}