﻿#if ENABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM_PACKAGE
#define USE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Controls;
#endif

using UnityEngine;

namespace UnityTemplateProjects
{
    public class SimpleCameraController : MonoBehaviour
    {
        class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

                x += rotatedTranslation.x;
                y += rotatedTranslation.y;
                z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
                
                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }
        }
        
        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();
        bool isKeyboard = true;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

        void OnEnable()
        {
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
        }

        Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();
            Vector2 leftStick = InputManager.instance.GetGamepadStick(StickType.LEFT, -1);

            if (InputManager.instance.IsKeyPressed(KeyType.W) || leftStick.y > 0)
            {
                direction += Vector3.forward;
            }
            if (InputManager.instance.IsKeyPressed(KeyType.S) || leftStick.y < 0)
            {
                direction += Vector3.back;
            }
            if (InputManager.instance.IsKeyPressed(KeyType.A) || leftStick.x < 0)
            {
                direction += Vector3.left;
            }
            if (InputManager.instance.IsKeyPressed(KeyType.D) || leftStick.x > 0)
            {
                direction += Vector3.right;
            }
            if (InputManager.instance.IsKeyPressed(KeyType.Q) || InputManager.instance.IsGamepadButtonPressed(ButtonType.LB, -1))
            {
                direction += Vector3.down;
            }
            if (InputManager.instance.IsKeyPressed(KeyType.E) || InputManager.instance.IsGamepadButtonPressed(ButtonType.RB, -1))
            {
                direction += Vector3.up;
            }
            return direction;
        }
        
        void Update()
        {
            Vector3 translation = Vector3.zero;
            if(InputManager.instance.IsAnyKeyPressed() || InputManager.instance.IsAnyMousePressed() || InputManager.instance.GetMouseDelta() != Vector2.zero)
            {
                isKeyboard = true;
            }
            else if(InputManager.instance.IsAnyGamePadInput(-1))
            {
                isKeyboard = false;
            }

            // Exit Sample  
            if (InputManager.instance.IsKeyDown(KeyType.ESC))
            {
                Application.Quit();
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false; 
				#endif
            }
            // Hide and lock cursor when right mouse button pressed

            // Rotation
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Vector2 delta = (isKeyboard) ? InputManager.instance.GetMouseDelta() : InputManager.instance.GetGamepadStick(StickType.RIGHT, -1);
            var mouseMovement = new Vector2(delta.x, delta.y * (invertY ? 1 : -1));
            
            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
            m_TargetCameraState.pitch = Mathf.Clamp(m_TargetCameraState.pitch + mouseMovement.y * mouseSensitivityFactor, -89.9f, 89.9f);
            
            // Translation
            translation = GetInputTranslationDirection() * Time.deltaTime;

            // Speed up movement when shift key held
            if (InputManager.instance.IsKeyPressed(KeyType.L_SHIFT) || InputManager.instance.IsGamepadButtonPressed(ButtonType.LS, -1))
            {
                translation *= 10.0f;

                GetComponent<MultiAudioAgent>().PlayOnce("Dash");
            }

            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            //boost += Input.mouseScrollDelta.y * 0.2f;
            //translation *= Mathf.Pow(2.0f, boost);

            m_TargetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform);
        }
    }

}