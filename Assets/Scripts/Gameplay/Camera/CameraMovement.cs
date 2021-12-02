using System;
using Event_Manager;
using Event_Manager.Events;
using Helpers;
using UnityEngine;
using Grid = Gameplay.Grid_System.Core.Grid;

namespace Gameplay.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        #region Events

        private void OnEnable()
        {
            EventManager.AddListener<GridInitializedEvent>(OnGridInit);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<GridInitializedEvent>(OnGridInit);
        }

        private void OnGridInit(GridInitializedEvent eventParam)
        {
            SetupCamera(eventParam.grid);
        }

        #endregion

        [SerializeField]
        private float moveSpeed = 50f;

        [SerializeField]
        private float scrollZoomSpeed = 5f;

        [SerializeField]
        private float maxCameraSize;

        [SerializeField]
        private float minCameraSize;

        private float currentZoom;

        private UnityEngine.Camera mainCamera;

        private float moveX, moveY;
        private Vector3 moveDir;
        private Vector3 NextPosition => transform.position + moveDir * moveSpeed * Time.deltaTime;
        private float maxX, maxY, minX, minY;

        private Grid grid;
        private float lastAspect;

        private bool IsNextPositionInBounds() => NextPosition.x.IsBetween(minX, maxX) && NextPosition.y.IsBetween(minY, maxY);


        private void Awake()
        {
            mainCamera = UnityEngine.Camera.main;
        }

        private void SetupCamera(Grid g)
        {
            grid = g;
            lastAspect = mainCamera.aspect;
            transform.position = grid.CenterPosition.SetZ(-10);
            minCameraSize = (grid.Height + grid.Width + grid.CellSize) / 5f;
            maxCameraSize = minCameraSize * 2f;
            currentZoom = (maxCameraSize + minCameraSize) / 2f;
            scrollZoomSpeed = grid.CellSize * 2;
            moveSpeed = (grid.Height + grid.Width) * grid.CellSize / 8;
            maxX = grid.CenterPosition.x + ((grid.Width / 2f) * grid.CellSize);
            maxY = grid.CenterPosition.y + ((grid.Height / 3f) * grid.CellSize);
            minX = grid.CenterPosition.x - ((grid.Width / 2f) * grid.CellSize);
            minY = grid.CenterPosition.y - ((grid.Height / 3f) * grid.CellSize);
        }

        private void Update()
        {
            if (Math.Abs(lastAspect - mainCamera.aspect) > .01f) SetupCamera(grid);

            currentZoom -= Input.GetAxis("Mouse ScrollWheel") * scrollZoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minCameraSize, maxCameraSize);
            mainCamera.orthographicSize = currentZoom;

            moveX = moveY = 0;

            if (Input.GetKey(KeyCode.W))
            {
                moveY = 1f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                moveY = -1f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                moveX = -1f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                moveX = 1f;
            }

            moveDir = new Vector3(moveX, moveY).normalized;

            if (IsNextPositionInBounds())
                transform.position = NextPosition;
        }
    }
}