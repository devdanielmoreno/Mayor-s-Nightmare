using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject obstaculoPrefab;
    public float spawnRate = 1f;

    void Start()
    {
        InvokeRepeating("Spawn", 1f, spawnRate);
    }

    void Spawn()
    {
        float y = Random.Range(-4f, 4f);
        Vector3 spawnPos = new Vector3(10f, y, 0f);
        Instantiate(obstaculoPrefab, spawnPos, Quaternion.identity);
    }
}