using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    private GameObject[] Clouds;

    public Transform CloudPrefab1, CloudPrefab2;

    private int cloudNumber;

    private float cloudProbability, randomScale;

    private Vector3 cloudPosition;

    // Start is called before the first frame update
    void Start()
    {
        cloudNumber = 8;
        Clouds = new GameObject[cloudNumber];

        //Spawn cloudNumber of clouds in random set position
        for (int i = 0; i < cloudNumber; i++)
        {
            cloudProbability = Random.Range(-100.0f, 100.0f);
            randomScale = Random.Range(0.2f, 1f);
            cloudPosition = new Vector3(Random.Range(-11f, 11f), Random.Range(-1.25f, 5f), 1 * -randomScale);

            if (cloudProbability >= 0)
            {
                GameObject go = Instantiate(CloudPrefab1, cloudPosition, transform.rotation).gameObject;
                go.transform.parent = GameObject.Find("Clouds").transform;
                go.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                Clouds[i] = go;
            }
            else
            {
                GameObject go = Instantiate(CloudPrefab2, cloudPosition, transform.rotation).gameObject;
                go.transform.parent = GameObject.Find("Clouds").transform;
                go.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                Clouds[i] = go;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Destory clouds if off screen to the left and spawn another at random set spawn on the right of the screen
        for(int i = 0; i < Clouds.Length; i++)
        {
            if (Clouds[i].transform.position.x < -15.4)
            {
                cloudProbability = Random.Range(-100.0f, 100.0f);
                randomScale = Random.Range(0.2f, 1f);
                cloudPosition = new Vector3(15.4f, Random.Range(-1.25f, 5f), 1 * -randomScale);

                Destroy(Clouds[i]);

                if (cloudProbability >= 0)
                {
                    GameObject go = Instantiate(CloudPrefab1, cloudPosition, transform.rotation).gameObject;
                    go.transform.parent = GameObject.Find("Clouds").transform;
                    go.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                    Clouds[i] = go;
                }
                else
                {
                    GameObject go = Instantiate(CloudPrefab2, cloudPosition, transform.rotation).gameObject;
                    go.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                    go.transform.parent = GameObject.Find("Clouds").transform;
                    Clouds[i] = go;
                }
                //i--;
            }
        }
    }
}
