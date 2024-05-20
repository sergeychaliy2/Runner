using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Cannon : MonoBehaviour
{
    [Header("Cannon_Settings")]
    [Space]
    public GameObject cannonPrefab; 
    public int poolSize = 2;
    public float fireInterval = 3f;
    public float returnDelay = 3f; 

    private List<GameObject> spherePool; 
    private float timer = 0f;

    void Start()
    {
        InitializePool();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            GameObject sphere = GetSphereFromPool();
            if (sphere == null)
            {
                return;
            }

            sphere.transform.position = transform.position;
            sphere.SetActive(true);

            sphere.GetComponent<Rigidbody>().velocity = transform.forward * 10f; 
            StartCoroutine(ReturnSphereToPool(sphere));
            timer = 0f;
        }
    }

    void InitializePool()
    {
        spherePool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject sphere = Instantiate(cannonPrefab, transform.position, Quaternion.identity);
            sphere.SetActive(false); 
            spherePool.Add(sphere);
        }
    }

    GameObject GetSphereFromPool()
    {
        foreach (GameObject sphere in spherePool)
        {
            if (!sphere.activeInHierarchy)
            {
                return sphere;
            }
        }
        return null;
    }

    IEnumerator ReturnSphereToPool(GameObject sphere)
    {
        yield return new WaitForSeconds(returnDelay);
        sphere.SetActive(false);
    }
}
