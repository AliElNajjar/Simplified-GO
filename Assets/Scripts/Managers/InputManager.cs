using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action<Vector3> OnInput;


    private void Start() => enabled = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnInput?.Invoke(GetMouseWorldPosition());
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

}
