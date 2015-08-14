/***********************************************************************************************************
 * JELLY CUBE - GAME STARTER KIT - Compatible with Unity 5                                                 *
 * Produced by TROPIC BLOCKS - http://www.tropicblocks.com - http://www.twitter.com/tropicblocks           *
 * Developed by Rodrigo Pegorari - http://www.rodrigopegorari.com                                          *
 ***********************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace JellyCube
{
    public class CubeController : MonoBehaviour
    {
        public Collider m_Cube;

        // m_CanMove: this cube will (slide/move/be pushed) when another cube (moving or rolling) collides
        public bool m_CanMove = true;

        // m_CanPush: this cube will push another cube (if this cube has m_CanMove = true)
        public bool m_CanPush = true;

        // m_CanRoll: this cube can roll after player input (cube player)
        public bool m_CanRoll = true;

        // m_CanShake: this cube will shake after a collision with a cube (moving or rolling) and there is no space to move
        public bool m_CanShake = true;

        public float m_RollSpeed = 0.15f;

        public float m_MoveSpeed = 0.15f;

        public Transform m_Trails;

        public Transform m_Splashs;

        private Vector3 m_LastMove = Vector3.zero;
        
        private Vector3 m_LastDir = Vector3.zero;

        private const float SHAKE_SCALE = 1.5f;

        void Start()
        {
            CubeManager.Instance.Register(this);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawSphere(m_LastMove, 0.1f);
        }

        public void DoRoll(Vector3 dir)
        {
            if (m_CanRoll)
            {
                ResetPosition();

                Vector3 ndir = Vector3.zero;

                if (Mathf.Abs(dir.x) == 1)
                {
                    ndir = dir * m_Cube.bounds.size.x / 2f;
                }
                else if (Mathf.Abs(dir.z) == 1)
                {
                    ndir = dir * m_Cube.bounds.size.z / 2f;
                }

                Vector3 newAxis = m_Cube.ClosestPointOnBounds(m_Cube.transform.position + ndir);
                newAxis.y = m_Cube.bounds.min.y;

                Vector3 thisPos = new Vector3();
                thisPos = m_Cube.transform.position;

                Quaternion thisRot = new Quaternion();
                thisRot = m_Cube.transform.rotation;

                transform.position = newAxis;
                transform.rotation = Quaternion.identity;
                
                m_Cube.transform.rotation = thisRot;
                m_Cube.transform.position = thisPos;

                Vector3 targetRotation = new Vector3(dir.z, dir.y, -dir.x) * 90;

                m_LastDir = dir;
                m_LastMove = m_Cube.transform.position + ndir;

                Vector3 origin = m_Cube.transform.position;
                RaycastHit outHit;

                if (!Physics.Linecast(origin, origin + dir, out outHit))
                {
                    CubeManager.Instance.RegisterMove(this);

                    //You can replace with iTween or any tweener you like
                    Tweener.RotateTo(this.gameObject, transform.rotation.eulerAngles, targetRotation, m_RollSpeed, 0, Tweener.TweenerEaseType.Linear, Complete);
                }
            }
        }

        public void DoPush(Vector3 dir)
        {
            if (m_CanPush)
            {
                Vector3 origin = m_Cube.transform.position;

                RaycastHit outHit = new RaycastHit();

                if (Physics.Linecast(origin, origin + dir, out outHit))
                {
                    //if has any collision object, look for a CubeController its parent object, and then try to move it
                    CubeController cube = outHit.collider.transform.GetComponentInParent<CubeController>();

                    if (cube != null)
                    {
                        cube.DoMove(dir);
                    }
                }
            }
        }


        public void DoMove(Vector3 dir)
        {
            if (m_CanMove)
            {
                m_LastDir = dir;

                Vector3 origin = m_Cube.transform.position;

                RaycastHit outHit;

                //if there isn´t any obstacle, than is possible to move this cube
                if (!Physics.Linecast(origin, origin + dir, out outHit))
                {
                    CubeManager.Instance.RegisterMove(this);

                    CreateSplash();

                    //You can replace with iTween or any tweener you like
                    Tweener.MoveTo(this.gameObject, transform.position, transform.position + dir, m_MoveSpeed, 0, Tweener.TweenerEaseType.EaseOutSine, Complete);
                    
                    return;
                }
            }

            //if is not possible to move, then try to shake it
            DoShake();
        }

        /// <summary>
        /// Complete Method is called after a Roll or Move event
        /// </summary>
        private void Complete()
        {
            DoPush(m_LastDir);

            CreateTrail();

            ResetPosition();

            CubeManager.Instance.UnregisterMove(this);
        }

        public void DoShake()
        {
            if (m_CanShake)
            {
                //You can replace with iTween or any tweener you like
                Tweener.ScaleTo(this.gameObject, new Vector3(1, SHAKE_SCALE, 1), Vector3.one, .5f, 0, Tweener.TweenerEaseType.EaseOutExpo);
            }
        }

        private void ResetPosition()
        {
            //Snap angles to 90 degrees
            transform.eulerAngles = RoundVector(transform.eulerAngles, 90f);
            m_Cube.transform.localEulerAngles = RoundVector(m_Cube.transform.localEulerAngles, 90f);

            //Snap position to .5 units
            this.transform.position = RoundVector(this.transform.position, .5f);
            this.m_Cube.transform.localPosition = RoundVector(this.m_Cube.transform.localPosition, .5f);
        }

        private Vector3 RoundVector(Vector3 value, float snapValue = 1f)
        {
            value.x = Mathf.Round(value.x / snapValue) * snapValue;
            value.y = Mathf.Round(value.y / snapValue) * snapValue;
            value.z = Mathf.Round(value.z / snapValue) * snapValue;

            return value;
        }

        private void CreateTrail()
        {
            if (m_Trails == null)
            {
                return;
            }

            Vector2 trailOffset = new Vector2(Random.Range(0, 2) * 0.5f, Random.Range(0, 2) * 0.5f);
            Quaternion decalRotation = Quaternion.Euler(new Vector3(90, Random.Range(0, 4) * 90f, 0));
            GameObject trail = Instantiate(m_Trails.gameObject, new Vector3(m_Cube.transform.position.x, m_Cube.bounds.min.y + 0.01f, m_Cube.transform.position.z), decalRotation) as GameObject;
            trail.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", trailOffset);
        }

        private void CreateSplash()
        {
            if (m_Splashs == null)
            {
                return;
            }

            Vector2 splashOffset = new Vector2(Random.Range(0, 4) * 0.25f, Random.Range(0, 4) * 0.25f);
            Quaternion decalRotation = Quaternion.Euler(new Vector3(90, Random.Range(0f, 360f), 0));
            GameObject splash = Instantiate(m_Splashs.gameObject, new Vector3(m_Cube.transform.position.x, m_Cube.bounds.min.y + 0.02f, m_Cube.transform.position.z), decalRotation) as GameObject;
            splash.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", splashOffset);
        }

    }
}