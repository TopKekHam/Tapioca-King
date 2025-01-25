using UnityEngine;

public class SpaceToSkip : MonoBehaviour
{
    [SerializeField] GameObject video;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(video);
        }
    }
}
