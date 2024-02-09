using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    PlayerController target;
    float rotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        target = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        



        rotation = target.yrot;
        transform.position = target.GetPlayerPosition() + (Quaternion.AngleAxis(rotation, Vector3.up)*(Vector3.forward+Vector3.up*1.25f)*2.25f);


        Vector3 direction = target.GetPlayerPosition() - (transform.position+Vector3.up*-2f);
        direction.Normalize();
        transform.rotation = Quaternion.LookRotation(direction);


        
    }
    private void LateUpdate()
    {
        
    }
}
