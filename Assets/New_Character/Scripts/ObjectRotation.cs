using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    // Choose the axis to rotate around
    public RotationAxis rotationAxis = RotationAxis.Y;

    // Rotation speed in degrees per second
    public float rotationSpeed = 90f;

    void Update()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        Vector3 rotationVector = Vector3.zero;

        switch (rotationAxis)
        {
            case RotationAxis.X:
                rotationVector = new Vector3(rotationAmount, 0f, 0f);
                break;
            case RotationAxis.Y:
                rotationVector = new Vector3(0f, rotationAmount, 0f);
                break;
            case RotationAxis.Z:
                rotationVector = new Vector3(0f, 0f, rotationAmount);
                break;
        }

        transform.Rotate(rotationVector);
    }
}
