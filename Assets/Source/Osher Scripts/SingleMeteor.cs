using UnityEngine;

public class SingleMeteor : MonoBehaviour
{
    Vector3 rotation;

    void Start()
    {
        rotation = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));
        Destroy(gameObject, 6);
    }

    void Update()
    {
        transform.Translate(Vector3.right + Vector3.down * 5 * Time.deltaTime);
        transform.Rotate(rotation * Time.deltaTime);
    }
}
