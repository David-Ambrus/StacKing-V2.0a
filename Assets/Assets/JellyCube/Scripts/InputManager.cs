/***********************************************************************************************************
 * JELLY CUBE - GAME STARTER KIT - Compatible with Unity 5                                                 *
 * Produced by TROPIC BLOCKS - http://www.tropicblocks.com - http://www.twitter.com/tropicblocks           *
 * Developed by Rodrigo Pegorari - http://www.rodrigopegorari.com                                          *
 ***********************************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JellyCube
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        private bool m_LockControls = false;

        private Vector2 m_MoveDirection = Vector3.zero;

        private bool m_Moved = false;

        private float m_TouchHoldTimer = 0;

        private const float TOUCH_HOLD_TIME_LIMIT = 0.2f;

        private const float TOUCH_SENSIBILITY = 2f;

        void Awake()
        {
            Instance = this;
        }

        public void LockControls()
        {
            m_LockControls = true;
        }

        public void UnlockControls()
        {
            m_LockControls = false;
        }

        void LateUpdate()
        {
            //During move operations, input is locked
            if (m_LockControls)
            {
                return;
            }

            #region TouchEvents
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        m_Moved = false;
                        m_MoveDirection = Vector2.zero;
                        m_TouchHoldTimer = 0;

                        break;

                    case TouchPhase.Ended:
                        m_Moved = false;
                        m_MoveDirection = Vector2.zero;

                        break;

                    case TouchPhase.Stationary:
                        m_Moved = false;
                        m_TouchHoldTimer += Time.deltaTime;

                        //if touch is holding for more than 0.2 seconds, then makes the cube rolling forever
                        if (m_TouchHoldTimer > TOUCH_HOLD_TIME_LIMIT)
                        {
                            CubeManager.Instance.RollCube(new Vector3(m_MoveDirection.x, 0, m_MoveDirection.y));
                        }

                        break;

                    case TouchPhase.Moved:

                        m_TouchHoldTimer = 0;

                        if (!m_Moved && touch.deltaPosition.sqrMagnitude > TOUCH_SENSIBILITY)
                        {
                            m_MoveDirection = Input.touches[0].deltaPosition;

                            if (Mathf.Abs(m_MoveDirection.x) == Mathf.Abs(m_MoveDirection.y))
                            {
                                m_MoveDirection = Vector2.zero;
                            }
                            else if (Mathf.Abs(m_MoveDirection.x) > Mathf.Abs(m_MoveDirection.y))
                            {
                                m_MoveDirection.x = m_MoveDirection.x > 0 ? 1 : -1;
                                m_MoveDirection.y = 0;
                            }
                            else
                            {
                                m_MoveDirection.x = 0;
                                m_MoveDirection.y = m_MoveDirection.y > 0 ? 1 : -1;
                            }

                            //if m_MoveDirection is different than Vector2.zero
                            if (m_MoveDirection.sqrMagnitude > .1f)
                            {
                                CubeManager.Instance.RollCube(new Vector3(m_MoveDirection.x, 0, m_MoveDirection.y));
                                m_Moved = true;
                            }
                        }

                        break;
                }

            }
            #endregion

            #region KeyboardEvents
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.ReloadLevel();
            }
            
            if (Input.GetKey(KeyCode.RightArrow))
            {
                CubeManager.Instance.RollCube(new Vector3(1, 0, 0));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                CubeManager.Instance.RollCube(new Vector3(-1, 0, 0));
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                CubeManager.Instance.RollCube(new Vector3(0, 0, 1));
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                CubeManager.Instance.RollCube(new Vector3(0, 0, -1));
            }
            #endregion

        }

    }
}