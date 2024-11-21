using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float minAngle = 0f;  // ����������� ����
    public float maxAngle = 90f; // ������������ ����
    public float speed = 100f;    // �������� ��������

    private bool rotatingForward = true;

    void Update()
    {
        // �������� ������� ���� �������� �� ��� Z
        float currentAngle = transform.localEulerAngles.z;

        // ����������� ����, ����� �� ��� � ��������� �� -180 �� 180
        if (currentAngle > 180)
            currentAngle -= 360;

        // ��������� ����, �� ������� ����� ��������� �������
        float rotationStep = speed * Time.deltaTime;

        // ������� ������� ������ �� ������������� ����
        if (rotatingForward)
        {
            transform.Rotate(0f, 0f, rotationStep);

            // ���� ��������� ������������ ����, �������� �������� �����
            if (currentAngle >= maxAngle)
            {
                rotatingForward = false;
            }
        }
        else // ������� ������� ����� �� ������������ ����
        {
            transform.Rotate(0f, 0f, -rotationStep);

            // ���� ��������� ����������� ����, �������� �������� ������
            if (currentAngle <= minAngle)
            {
                rotatingForward = true;
            }
        }
    }

    public float GetCurrentAngle() => transform.localEulerAngles.z;
}
