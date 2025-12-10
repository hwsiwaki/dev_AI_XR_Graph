using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Paroxe.PdfRenderer.Internal.Viewer
{

    public class PDFViewerSearchButton : UIBehaviour
    {
        [SerializeField] PDFSearchPanel panel;

        void Start()
        {
            panel.Close();
        }
        public void OnClick()
        {
#if !UNITY_WEBGL
            panel.Toggle();
#endif
        }
    }
}