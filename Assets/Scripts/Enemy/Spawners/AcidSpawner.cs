using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidSpawner : MonoBehaviour
{
    public float interval;
    public GameObject Acid;
    void Start()
    {
        InvokeRepeating("Spawn", 0, interval);
    }

    // Update is called once per frame
    private void Spawn()
    {
        GameObject AcidToSpawn = Instantiate(Acid, transform.position, Quaternion.identity);
    }
}
