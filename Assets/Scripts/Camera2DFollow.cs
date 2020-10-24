using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Camera2DFollow : MonoBehaviour
{
    public Transform target;
    Rigidbody2D targetRB;
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        targetRB = target.GetComponent<Rigidbody2D>();
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, 
            new Vector3(target.position.x, target.position.y, GameConstants.cameraDistance) + (new Vector3(targetRB.velocity.x, targetRB.velocity.y, 0) * GameConstants.cameraLeadConstant), 
            Time.fixedDeltaTime * 5);
    }

    public void ZoomNormal()
    {

    }

    public void ZoomIn()
    {
        
    }

    public void ZoomOut()
    {

    }
}
