// скрипт для вращения камеры вокруг игрока в зависимости от движения мыши
// автор Pravda xd

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotator : MonoBehaviour
{

    // переменные для хранения позиции мыши на оси X и Y
    private float mouseX;
    private float mouseY;

    // настройка чувствительности мыши
    [Range(50f, 500f)]
    public float sentMouse = 200f;

    // ограничения на вращение по оси Y
    [Range(-90f, 90f)]
    public float minYAngle = -90f;
    [Range(-90f, 90f)]
    public float maxYAngle = 90f;

    // объект игрока для вращения вокруг него камеры
    public Transform player;

    // текущий угол вращения по осям X и Y
    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start()
    {
        // закрепляем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        // получаем значения движения мыши по осям X и Y и умножаем на чувствительность и время между кадрами
        mouseX = Input.GetAxis("Mouse X") * sentMouse * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sentMouse * Time.deltaTime;

        // вращение вокруг оси X
        // уменьшаем угол вращения по оси X на значение движения мыши по Y (ускорение камеры вверх и вниз)
        rotationX -= mouseY;
        // ограничиваем угол вращения по оси Y с помощью функции Mathf.Clamp
        rotationX = Mathf.Clamp(rotationX, minYAngle, maxYAngle);

        // вращение вокруг оси Y
        // увеличиваем угол вращения по оси Y на значение движения мыши по X (поворот камеры вокруг игрока)
        rotationY += mouseX;

        // применяем полученные углы вращения к повороту камеры
        // вращение по оси X
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        // вращение по оси Y
        player.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}