using UnityEngine;

namespace SimplestarGame
{
    /// <summary>
    /// キューブをその場で回転
    /// </summary>
    public class RotateTransform : MonoBehaviour
    {
        [SerializeField, Tooltip("回転する軸オイラー角度")] Vector3 rotateEulerAngles = Vector3.zero;
        void Update()
        {
            this.transform.Rotate(rotateEulerAngles);
        }
    }
}