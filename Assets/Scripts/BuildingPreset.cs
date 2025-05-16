using UnityEngine;

[CreateAssetMenu(fileName = "BuildingPreset", menuName = "Crear Edificio", order = 1)]
public class BuildingPreset : ScriptableObject
{
    public enum TipoEdificio { Casa, Granja, Herrería }
    public TipoEdificio tipo;

    public int cost;
    public int costPerTurn;
    public GameObject prefab;

    public int population;
    public int jobs;
    public int food;
}