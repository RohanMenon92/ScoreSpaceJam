using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Camera2DFollow : MonoBehaviour
{
    public Transform target;
    Rigidbody2D targetRB;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        targetRB = target.GetComponent<Rigidbody2D>();
        cam = GetComponentInChildren<Camera>();
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
        cam.DOFieldOfView(45, GameConstants.attackMove);
    }

    public void ZoomIn()
    {
        cam.DOFieldOfView(15, GameConstants.attackMove);
    }

    public void CameraShake(float strength)
    {
        cam.transform.eulerAngles = Vector3.zero;
        cam.DOShakeRotation(GameConstants.hitShakeDuration, strength * 3, 10, 90, true).OnComplete(() => { });
    }
}
