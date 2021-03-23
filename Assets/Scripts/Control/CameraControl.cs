using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraControl : MonoBehaviour
{

    public enum Smooth
    {
        Disabled = 0,
        Enabled = 1
    };

    [Header("General")] public float sensitivity = 2; // чувствительность мышки
    public float distance = 5; // расстояние между камерой и игроком
    public float height = 2.3f; // высота

    [Header("Over The Shoulder")] public float offsetPosition; // смешение камеры вправо или влево, 0 = центр

    [Header("Smooth Movement")] public Smooth smooth = Smooth.Enabled;
    public float speed = 8; // скорость сглаживания

    [SerializeField] private Transform player;


    void LateUpdate()
    {
        if (player)
        {
            Vector3 position = player.position - (transform.rotation * Vector3.forward * distance);
            position = position + (transform.rotation * Vector3.right * offsetPosition); // сдвиг по горизонтали
            position = new Vector3(position.x, player.position.y + height, position.z); // корректировка высоты

            transform.LookAt(player);

            if (Input.GetKey(KeyCode.E))
            {
                transform.RotateAround(player.position, Vector3.up, -sensitivity);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.RotateAround(player.position, Vector3.up, sensitivity);
            }

            var mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheel != 0)
            {
                height -= sensitivity * mouseWheel;
                distance -= sensitivity * mouseWheel;
            }

            if (smooth == Smooth.Disabled) transform.position = position;
            else transform.position = Vector3.Lerp(transform.position, position, speed * Time.deltaTime);
        }
    }
}