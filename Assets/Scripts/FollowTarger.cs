using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarger : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 offset;
    public GameObject follerTarget;
    public float zooSpeed = 22.0f;

    void Start()
    {

        offset = transform.position - follerTarget.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = follerTarget.transform.position + offset;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.fieldOfView += scroll * zooSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 37, 76); //限制取值范围
    }
}