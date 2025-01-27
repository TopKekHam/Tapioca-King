using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{

    void Update()
    {
        var camera = Camera.main;

        if (camera != null)
        {
            transform.forward = (transform.position - camera.transform.position).normalized;
        }
    }
}
