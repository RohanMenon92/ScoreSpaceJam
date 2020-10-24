using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    bool isInRange;
    PlayerScript player;

    public List<GunPort> gunPorts = new List<GunPort>();
    public float rotateSpeed = 0.75f;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerRange();
        TryToAim();
    }

    void CheckPlayerRange()
    {
        isInRange = Vector3.Distance(player.transform.position, transform.position) < GameConstants.turretRange;
    }

    void TryToAim()
    {
        if(isInRange)
        {
            Vector3 targetPosition = player.GetPredictedAim();
            targetPosition.z = transform.position.z;
            Quaternion lookOnLook = Quaternion.LookRotation(targetPosition - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * rotateSpeed);

            foreach(GunPort gun in gunPorts)
            {
                gun.Fire(true, Vector3.zero);
            }
        }
    }


}
