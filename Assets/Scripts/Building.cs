using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingPreset preset;
    [SerializeField] private Collider mainCollider;
    
    private int trabajadoresActuales = 0; // 👷 Contador de trabajadores

    public Collider GetCollider() => mainCollider;
    

    public bool TieneEspacioDisponible()
    {
        int trabajadoresEnEsteEdificio = 0;

        foreach (GameObject aldeanoObj in City.instance.aldeanos)
        {
            Aldeano aldeano = aldeanoObj.GetComponent<Aldeano>();

            if (aldeano != null && aldeano.home == this)
            {
                if ((aldeano.tipo == TipoAldeano.Granjero && this.CompareTag("Granja")) ||
                    (aldeano.tipo == TipoAldeano.Guerrero && this.CompareTag("Herrería")))
                {
                    trabajadoresEnEsteEdificio++;
                }
            }
        }

        Debug.Log($"🏗️ {this.name} - Trabajadores: {trabajadoresEnEsteEdificio}/2");
        return trabajadoresEnEsteEdificio < 2;
    }

    public void AgregarTrabajador()
    {
        if (TieneEspacioDisponible())
        {
            trabajadoresActuales++;
        }
    }

    public void RemoverTrabajador()
    {
        if (trabajadoresActuales > 0)
        {
            trabajadoresActuales--;
        }
    }

    public int GetTrabajadoresActuales()
    {
        return trabajadoresActuales;
    }
}
