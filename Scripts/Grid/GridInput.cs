namespace Pathfinding.Scripts
{
    using UnityEngine;
    using System;

    public class GridInput : MonoBehaviour
    {
        [Header("Settings")]
        public LayerMask gridLayer;
        public float gridHeight = 0f; 

        public event Action<Vector3> OnLeftClick;  
        public event Action<Vector3> OnRightClick;
        public event Action OnClearInput;

        private Camera _mainCamera;

        void Start()
        {
            _mainCamera = Camera.main;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClearInput?.Invoke(); 
                return;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Vector3? hitPoint = GetGridHitPosition();

                if (hitPoint.HasValue)
                {
                    if (Input.GetMouseButtonDown(0)) 
                        OnLeftClick?.Invoke(hitPoint.Value);
                    
                    else if (Input.GetMouseButtonDown(1)) 
                        OnRightClick?.Invoke(hitPoint.Value); 
                }
            }
        }

        private Vector3? GetGridHitPosition()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane gridPlane = new Plane(Vector3.up, new Vector3(0, gridHeight, 0));

            if (gridPlane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            return null;
        }
    }
}