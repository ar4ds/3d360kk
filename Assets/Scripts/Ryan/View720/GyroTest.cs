
using UnityEngine;
using System.Collections;
 
public class GyroTest : MonoBehaviour
{
    // Fields
    private readonly Quaternion baseIdentity = Quaternion.Euler(90f, 0f, 0f);
    private Quaternion baseOrientation = Quaternion.Euler(90f, 0f, 0f);
    private Quaternion baseOrientationRotationFix = Quaternion.identity;
    private Quaternion calibration = Quaternion.identity;
    private Quaternion cameraBase = Quaternion.identity;
    private bool debug = true;
    public static bool gyroAvaiable;
    private bool gyroEnabled = true;
    private Quaternion gyroInitialRotation;
    public static bool gyroOff;
    private Quaternion initialRotation;
    private readonly Quaternion landscapeLeft = Quaternion.Euler(0f, 0f, -90f);
    private readonly Quaternion landscapeRight = Quaternion.Euler(0f, 0f, 90f);
    private const float lowPassFilterFactor = 0.1f;
    private Quaternion offsetRotation;
    private Quaternion referanceRotation = Quaternion.identity;
    private readonly Quaternion upsideDown = Quaternion.Euler(0f, 0f, 180f);
 
    // Methods
    private void AttachGyro()
    {
        this.gyroEnabled = true;
        this.ResetBaseOrientation();
        this.UpdateCalibration(true);
        this.UpdateCameraBaseRotation(true);
        this.RecalculateReferenceRotation();
    }
 
    private void Awake()
    {
        gyroAvaiable = SystemInfo.supportsGyroscope;
    }
 
    private static Quaternion ConvertRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
 
    private void DetachGyro()
    {
        this.gyroEnabled = false;
    }
 
    private Quaternion GetRotFix()
    {
        return Quaternion.identity;
    }
 
    private void RecalculateReferenceRotation()
    {
        this.referanceRotation = Quaternion.Inverse(this.baseOrientation) * Quaternion.Inverse(this.calibration);
    }
 
    private void ResetBaseOrientation()
    {
        this.baseOrientationRotationFix = this.GetRotFix();
        this.baseOrientation = this.baseOrientationRotationFix * this.baseIdentity;
    }
 
    protected void Start()
    {
 
            Input.gyro.enabled = true;
            base.enabled = true;
        this.AttachGyro();
        this.initialRotation = base.transform.localRotation;
        this.gyroInitialRotation = Input.gyro.attitude;
    }
 
    private void Update()
    {
        gyroOff = PlayerPrefs.GetInt("gyro-off") == 1;
        if (this.gyroEnabled )
        {
            base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, this.cameraBase * (ConvertRotation(this.referanceRotation * Input.gyro.attitude) * this.GetRotFix()), 0.5f);//0.1f
        }
    }
 
    private void UpdateCalibration(bool onlyHorizontal)
    {
        if (onlyHorizontal)
        {
            Vector3 toDirection = (Vector3) (Input.gyro.attitude * -Vector3.forward);
            toDirection.z = 0f;
            if (toDirection == Vector3.zero)
            {
                this.calibration = Quaternion.identity;
            }
            else
            {
                this.calibration = Quaternion.FromToRotation((Vector3) (this.baseOrientationRotationFix * Vector3.up), toDirection);
            }
        }
        else
        {
            this.calibration = Input.gyro.attitude;
        }
    }
 
    private void UpdateCameraBaseRotation(bool onlyHorizontal)
    {
        if (onlyHorizontal)
        {
            Vector3 forward = base.transform.forward;
            forward.y = 0f;
            if (forward == Vector3.zero)
            {
                this.cameraBase = Quaternion.identity;
            }
            else
            {
                this.cameraBase = Quaternion.FromToRotation(Vector3.forward, forward);
            }
        }
        else
        {
            this.cameraBase = base.transform.rotation;
        }
    }
}
