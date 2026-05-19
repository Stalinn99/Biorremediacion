    using UnityEngine;

    public class Camera_Controller : MonoBehaviour
    {
        public Transform Player;
        [SerializeField] private float sensibility = 100.0f;
        [SerializeField] private float limiteVertical = 80f;

        private float rotacionVertical = 0.0f;

        void Update()
        {
            float mouseY = Input.GetAxis("Mouse Y") * sensibility * Time.deltaTime;
            rotacionVertical -= mouseY;
            rotacionVertical = Mathf.Clamp(rotacionVertical, -limiteVertical, limiteVertical);
            transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
        }
    }