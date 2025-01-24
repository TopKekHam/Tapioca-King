using UnityEngine;

public class WobbleAnimationScript : MonoBehaviour
{
    float loopTimer;
    float directionChangeTimer;

    Vector3 dir;
    Vector3 rotDir;

    private void Start()
    {
        dir = Vector3.zero;
        rotDir = Vector3.zero;
        directionChangeTimer = Time.time + 0.05f;
    }

    void Update()
    {
        if (Time.time > directionChangeTimer)
        {
            dir =  new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * Time.deltaTime*0.5f;
            rotDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Time.deltaTime * 15f;
            directionChangeTimer = Time.time + Random.Range(1.15f,3.35f);
        }
        transform.localPosition += dir;
        transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z)+rotDir;

    }
}
