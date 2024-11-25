using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace cs5678_2024sp.h_prism.g03
{

    /// <summary>
    /// This class handles the feedback for the Prism interaction technique. When the technique is allowed, the sphere turns magenta, otherwise white. It draws a line from the controller to the prism when the technique is enabled.
    /// </summary>
    public class PrismFeedback : MonoBehaviour
    {
        private Prism m_Prism;
        [SerializeField] private Material m_LineMaterial;
        private Vector3 m_controllerPosition;
        private Vector3 m_prismPosition;
        private GameObject m_InteractorVisual;
        private LineRenderer m_lineRenderer;

        void Start()
        {
            m_Prism = GetComponent<Prism>();

            // add a line renderer to the prism
            m_lineRenderer = gameObject.AddComponent<LineRenderer>();
            m_lineRenderer.material = m_LineMaterial;
            m_lineRenderer.startWidth = 0.01f;
            m_lineRenderer.endWidth = 0.01f;
            m_lineRenderer.positionCount = 2;

            // create a 0.1 size sphere gameobject for m_InteractorVisual and set its transform to the direct interactor's transform
            m_InteractorVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_InteractorVisual.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            m_InteractorVisual.transform.position = m_Prism.GetDirectInteractor().transform.position;
            // m_InteractorVisual.transform.position = m_Prism.m_DirectInteractor.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            m_InteractorVisual.transform.position = m_Prism.GetDirectInteractor().transform.position;

            if (m_Prism.prismEnabled)
            {
                DrawPrismLine();
            }
            else
            {
                m_lineRenderer.enabled = false;
            }

            // check if the m_offsetVector is not zero, and color the sphere purple
            if (m_Prism.prismAllowed)
            {
                m_InteractorVisual.GetComponent<Renderer>().material.color = Color.magenta;
            }
            else
            {
                m_InteractorVisual.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        /// <summary>
        /// Draws a line from the controller to the prism.
        /// </summary>
        private void DrawPrismLine()
        {
            // draw a line from the controller to the prism
            // m_controllerPosition = m_Prism.CurrentControllerPosition;
            m_controllerPosition = m_Prism.GetCurrentControllerPosition();
            m_prismPosition = m_InteractorVisual.transform.position;

            m_lineRenderer.SetPosition(0, m_controllerPosition);
            m_lineRenderer.SetPosition(1, m_prismPosition);
            if (!m_Prism.prismEnabled)
            {
                m_lineRenderer.enabled = false;
            }
            else
            {
                m_lineRenderer.enabled = true;
            }

        }
    }
}