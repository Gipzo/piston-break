using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPos;
    private static CameraShake _cameraShake;
    public float Amount = 0;
    public float ScreenShakeFactor = 1f;
    public float Decrease = 0.5f;


    void Awake()
    {
        _originalPos = transform.position;
    }

    public static void ShakeCamera(float amount)
    {
        if (_cameraShake == null)
            _cameraShake = Camera.main.GetComponent<CameraShake>();
        _cameraShake.Amount = amount / 10f * _cameraShake.ScreenShakeFactor;
    }


    void Update()
    {
        if (Amount > 0)
        {
            transform.position = _originalPos + Random.insideUnitSphere * Amount;

            Amount -= Time.deltaTime * Decrease * (1f / Mathf.Abs(Amount + 0.1f));
        }
        else
        {
            transform.localPosition = _originalPos;
        }
    }
}