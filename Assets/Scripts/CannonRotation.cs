using UnityEngine;
using System.Collections.Generic;

public class CannonRotation : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public float rotationAmount = 30f; 
    public List<Transform> cannons;

    private Quaternion initialRotation;
    private float currentRotationAmount;

    void Start()
    {
        initialRotation = transform.rotation;
        currentRotationAmount = 0f;
    }

    void Update()
    {
        foreach (Transform cannon in cannons)
        {
            Quaternion targetRotation;
            float localRotationAmount = currentRotationAmount % (rotationAmount * 2);
            if (localRotationAmount <= rotationAmount)
            {
                targetRotation = Quaternion.Euler(0f, localRotationAmount, 0f) * initialRotation;
            }
            else
            {
                targetRotation = Quaternion.Euler(0f, -(localRotationAmount - rotationAmount * 2), 0f) * initialRotation;
            }

            cannon.rotation = Quaternion.RotateTowards(cannon.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        currentRotationAmount += rotationSpeed * Time.deltaTime;
    }
}
