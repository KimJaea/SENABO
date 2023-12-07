using Unity.VisualScripting;
using UnityEngine;

public class GyroManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject myDog;

    private Gyroscope gyro;

    //private Quaternion rot;

    void Start()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
    }

    private void OnDestroy()
    {
        gyro.enabled = false;
    }

    private void Update()
    {
       Debug.Log("gyro: "+gyro.attitude);
        Quaternion gyroRotation = gyro.attitude;

        // y축 회전 확인
        float yRotation = gyroRotation.eulerAngles.y;
        myDog.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
