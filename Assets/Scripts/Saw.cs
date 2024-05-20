using UnityEngine;

public class Saw : MonoBehaviour
{
    [Header("Speed_Saw")]
    [Space]
    public float rotationSpeed = 100f; 

    void Update()
    {
        float angle = rotationSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation *= rotation;
    }
}
