using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float minAngle = 0f;  // ћинимальный угол
    public float maxAngle = 90f; // ћаксимальный угол
    public float speed = 100f;    // —корость вращени€

    private bool rotatingForward = true;

    void Update()
    {
        // ѕолучаем текущий угол вращени€ по оси Z
        float currentAngle = transform.localEulerAngles.z;

        // Ќормализуем угол, чтобы он был в диапазоне от -180 до 180
        if (currentAngle > 180)
            currentAngle -= 360;

        // ¬ычисл€ем угол, на который нужно повернуть стрелку
        float rotationStep = speed * Time.deltaTime;

        // ¬ращаем стрелку вперед до максимального угла
        if (rotatingForward)
        {
            transform.Rotate(0f, 0f, rotationStep);

            // ≈сли достигнут максимальный угол, начинаем вращение назад
            if (currentAngle >= maxAngle)
            {
                rotatingForward = false;
            }
        }
        else // ¬ращаем стрелку назад до минимального угла
        {
            transform.Rotate(0f, 0f, -rotationStep);

            // ≈сли достигнут минимальный угол, начинаем вращение вперед
            if (currentAngle <= minAngle)
            {
                rotatingForward = true;
            }
        }
    }

    public float GetCurrentAngle() => transform.localEulerAngles.z;
}
