using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    // List of collectible prefabs to choose from
    public List<GameObject> collectiblePrefabs;


    // Start is called before the first frame update
    void Start()
    {
        // Randomly pick a collectible from the list
        int randomIndex = Random.Range(0, collectiblePrefabs.Count);
        GameObject collectible = collectiblePrefabs[randomIndex];

        // Instantiate the collectible at the current spawn point
        Instantiate(collectible, gameObject.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
