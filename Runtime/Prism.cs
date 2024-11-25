using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace cs5678_2024sp.h_prism.g03
{

    /// <summary>
    /// <para>
    /// Logic component for the prism object manipulation technique based on the paper: https://ieeexplore.ieee.org/stamp/stamp.jsp?tp=&amp;arnumber=1492759. 
    /// </para>
    /// <para>
    /// This implementation is axis by axis. The controller's velocity is calculated and used to determine the offset of the controller.
    /// </para>
    /// </summary>
    public class Prism : MonoBehaviour
    {

        [SerializeField] private float m_MinV = 0.05f; //0.05 m/sec from paer
        [SerializeField] private float m_MaxV = 0.75f; //0.25 m/sec from paper
        [SerializeField] private float m_SC = 0.1f; // Scaling Constant: WE BELIEVE WE SET THIS OURSELVES. WE SET IT TO MINV+MAXV/2 TO START. REFINE THIS! RY+GH ON 3/23/24

        // References to the other PRISM scripts
        private PrismFeedback m_PrismFeedback;
        private PrismInput m_PrismInput;
        
        
        //Controller Related Variables
        [Header("Controller Related Variables")] [SerializeField]
        private Transform m_Controller; // The controller that is being tracked
        [SerializeField] private XRDirectInteractor m_DirectInteractor; // The direct interactor
        private Vector3 m_ControllerVelocityVector; // The velocity of the controller
        private Vector3 m_CurrentControllerPosition; // The current position of the controller
        private Vector3 m_PreviousControllerPosition; // The previous position of the controller
        private Vector3 m_OffsetVector; // The offset vector
        private Vector3 m_deltaPos; 
        private bool m_PrismEnabled; // Is the PRISM technique enabled?

        private bool m_PrismAllowed = true; // Is the PRISM technique allowed?

        // Start is called before the first frame update
        void Start()
        {
            // Get the other PRISM scripts
            m_PrismFeedback = GetComponent<PrismFeedback>();
            m_PrismInput = GetComponent<PrismInput>();

            // Set the initial state of the PRISM technique
            // m_PrismEnabled = false;

            // Set the initial controller velocity
            // m_ControllerVelocityVector = Vector3.zero;

            m_CurrentControllerPosition = m_Controller.transform.position;
            m_OffsetVector = m_CurrentControllerPosition;
            m_DirectInteractor.transform.position = m_OffsetVector;
        }
        
        
        

        // Update is called once per frame
        void Update()
        {
            if (m_PrismEnabled)
            {
                
                // calculate new velocity 
                m_PreviousControllerPosition = m_CurrentControllerPosition;
                m_CurrentControllerPosition = m_Controller.transform.position;
                m_deltaPos = m_CurrentControllerPosition - m_PreviousControllerPosition;
                float deltaTime = Time.deltaTime;
                m_ControllerVelocityVector = m_deltaPos / deltaTime;

                if (RecoverOffset(m_ControllerVelocityVector))
                {
                    m_DirectInteractor.transform.position = m_CurrentControllerPosition;
                }
                else
                {
                    // Calculate Offsets
                    float xOffset = CalculateDeltaOffset(m_ControllerVelocityVector.x, m_deltaPos.x, m_OffsetVector.x,
                        m_CurrentControllerPosition.x);
                    float yOffset = CalculateDeltaOffset(m_ControllerVelocityVector.y, m_deltaPos.y, m_OffsetVector.y,
                        m_CurrentControllerPosition.y);
                    float zOffset = CalculateDeltaOffset(m_ControllerVelocityVector.z, m_deltaPos.z, m_OffsetVector.z,
                        m_CurrentControllerPosition.z);

                    // set offsets
                    m_OffsetVector.x = xOffset;
                    m_OffsetVector.y = yOffset;
                    m_OffsetVector.z = zOffset;

                    m_DirectInteractor.transform.position = m_OffsetVector;
                }
            }

            if (m_PrismEnabled != true)
            {
                m_CurrentControllerPosition = m_Controller.transform.position;
                m_OffsetVector = m_CurrentControllerPosition;
                m_DirectInteractor.transform.position = m_OffsetVector;
            }
        }


        ///-------------------------------------------------------------------------------------------------
        ///----------------------------------PUBLIC METHODS------------------------------------------------

        /// <summary>
        /// Minimum Velocity of the controller.
        /// </summary>
        public float minV
        {
            get => m_MinV;
            set => m_MinV = value;
        }
        
        /// <summary>
        /// Returns the current controller position.
        /// </summary>
        public Vector3 CurrentControllerPosition
        {
            get => m_CurrentControllerPosition;
        }

        /// <summary>
        /// Maximum Velocity of the controller.
        /// </summary>
        public float maxV
        {
            get => m_MaxV;
            set => m_MaxV = value;
        }

        /// <summary>
        /// Returns the Transform of the controller that is being tracked.
        /// </summary>
        public Transform controller
        {
            get => m_Controller;
            set => m_Controller = value;
        }

        /// <summary>
        /// Returns the current state of the prism technique.
        /// </summary>
        public bool prismEnabled
        {
            get => m_PrismEnabled;
            set => m_PrismEnabled = value;
        }

        /// <summary>
        /// Returns whether the prism technique is allowed.
        /// </summary>
        public bool prismAllowed{
            get => m_PrismAllowed;
            set => m_PrismAllowed = value;
        }

      /* public PrismState prismState
        {
            get => m_PrismState;
            set => m_PrismState = value;
        }*/

        /// <summary>
        /// Returns the current position of the controller.
        /// </summary>
        public Vector3 GetCurrentControllerPosition()
        {
            return m_CurrentControllerPosition;
        }

        /// <summary>
        /// Returns the Direct Interactor.
        /// </summary>
        public XRDirectInteractor GetDirectInteractor()
        {
            return m_DirectInteractor;
        }

        /// <summary>
        /// Returns the offset vector.
        /// </summary>
        public Vector3 GetOffsetVector()
        {
            return m_OffsetVector;
        }


        ///-------------------------------------------------------------------------------------------------
        ///--------------------------------END PRIVATE METHODS----------------------------------------------
        ///-------------------------------------------------------------------------------------------------

        private float CalculateDeltaOffset(float velocity, float deltaPos, float currentPos, float controllerPos)
        {
            float magnitude = MathF.Abs(velocity);
            if (magnitude < m_MinV)
            {
                // Still
                return currentPos;
            }
            else if (magnitude >= m_MinV && magnitude < m_MaxV)
            {
                // Scaled
                float scale = magnitude / m_SC;
                scale = MathF.Min(scale, 1);
                float scaledDeltaPos = scale * deltaPos;
                float newPos = currentPos + scaledDeltaPos;
                return newPos;
            }
            else
            {
                // Offset Recovery -- used if you reset per axis instead of all together
                return controllerPos;
            }
        }

        private bool RecoverOffset(Vector3 velocity)
        {
            float magnitudeX = MathF.Abs(velocity.x);
            float magnitudeY = MathF.Abs(velocity.y);
            float magnitudeZ = MathF.Abs(velocity.z);
            return (magnitudeX > m_MaxV) && (magnitudeY > m_MaxV) && (magnitudeZ > m_MaxV);
        }
    }
}