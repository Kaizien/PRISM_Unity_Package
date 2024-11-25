using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace cs5678_2024sp.h_prism.g03
{
    /// <summary>
    /// <para>
    /// This class handles the input for the Prism interaction technique. It listens for the input action that toggles the the Prism Allowed state. 
    /// </para>
    /// <para>
    /// If the prism activation is allowed, the technique is enabled when an object is selected. The technique is disabled when the object is deselected.
    /// </para>
    /// </summary>
    public class PrismInput : MonoBehaviour
    {
        //reference to PRISM Logic script
        private Prism m_Prism;

        // Input Action Property
        public InputActionProperty togglePrism;
        


        // Start is called before the first frame update
        void Start()
        {
            m_Prism = GetComponent<Prism>();
        }

        void OnEnable()
        {
            // Enable the Input Action
            togglePrism.action.Enable();
            togglePrism.action.performed += OnAllowPrism;
        }

        void OnDisable()
        {
            // Disable the Input Action
            togglePrism.action.Disable();
            togglePrism.action.performed -= OnAllowPrism;
        }

        /// <summary>
        /// Toggles the Prism technique on and off.
        /// </summary>
        public void TogglePrism()
        {
            //object has been selected Enable Prism
            if (m_Prism.prismAllowed)
            {
                m_Prism.prismEnabled = !m_Prism.prismEnabled;
                Debug.Log("Item Selection event by direct interactor! Prism status: " + m_Prism.prismEnabled);
            }
        }

        /// <summary>
        /// Callback function for the input action that toggles the Prism technique on and off.
        /// </summary>
        /// <param name="context"></param>
        // Use an Input Action to toggle the Prism
        private void OnAllowPrism(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // change prism allowed
                m_Prism.prismAllowed = !m_Prism.prismAllowed;
                // if prism is not allowed, disable the prism
                if (!m_Prism.prismAllowed)
                {
                    m_Prism.prismEnabled = false;
                }
            }
        }
    }
} 