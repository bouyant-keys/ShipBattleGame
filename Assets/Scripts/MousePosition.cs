using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour
{
    public Transform _mousePos;
    public Plane _plane = new Plane(Vector3.down, 0);

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (_plane.Raycast(ray, out float distance)) _mousePos.position = ray.GetPoint(distance);
    }
}
