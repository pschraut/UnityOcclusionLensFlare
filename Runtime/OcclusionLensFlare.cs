//
// Occlusion Lens Flare for Unity. Copyright (c) 2021 Peter Schraut (www.console-dev.de). See LICENSE.md
// https://github.com/pschraut/UnityOcclusionLensFlare
//
using UnityEngine;

namespace Oddworm.Framework
{
    [HelpURL("https://github.com/pschraut/UnityOcclusionLensFlare")]
    [RequireComponent(typeof(LensFlare))]
    [DefaultExecutionOrder(1000)]
    public class OcclusionLensFlare : MonoBehaviour
    {
        [Tooltip("A reference to the renderer that represents the sun that's rendered into the occlusion texture.")]
        [SerializeField] Renderer m_OcclusionSun = default;

        [Tooltip("A reference to the camera that is used to render the occlusion texture.")]
        [SerializeField] Camera m_OcclusionCamera = default;

        [Tooltip("A reference to the render texture where the lens flare occlusion is rendered into.")]
        [SerializeField] RenderTexture m_OcclusionTexture = default;

        [Tooltip("A reference to the shader that is used to render the occlusion texture.")]
        [SerializeField] Shader m_OcclusionShader = default;

        Plane[] m_FrustumPlanes = new Plane[6];

        public Renderer occlusionSun
        {
            get => m_OcclusionSun;
            set => m_OcclusionSun = value;
        }

        public Camera occlusionCamera
        {
            get => m_OcclusionCamera;
        }

        public RenderTexture occlusionTexture
        {
            get => m_OcclusionTexture;
        }

        public Shader occlusionShader
        {
            get => m_OcclusionShader;
        }

        void Start()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (m_OcclusionSun == null)
                Debug.LogError($"The occlusion sun property in '{name}' must not be 'None'.", this);

            if (m_OcclusionCamera == null)
                Debug.LogError($"The occlusion camera property in '{name}' must not be 'None'.", this);

            if (m_OcclusionShader == null)
                Debug.LogError($"The occlusion shader property in '{name}' must not be 'None'.", this);

            if (m_OcclusionTexture == null)
                Debug.LogError($"The occlusion texture property in '{name}' must not be 'None'.", this);

            if (m_OcclusionTexture.mipmapCount == 0)
                Debug.LogError($"The {nameof(RenderTexture)} '{m_OcclusionTexture.name}' must use mipmaps to propertly work with the {nameof(OcclusionLensFlare)}", this);
#endif

            m_OcclusionCamera.enabled = false; // Don't let Unity render the camera, we do this in this Component instead
            m_OcclusionCamera.backgroundColor = new Color(0, 0, 0, 0);

            Shader.SetGlobalTexture("_FlareOcclusionTexture", m_OcclusionTexture);
        }

        void LateUpdate()
        {
            if (m_OcclusionCamera == null || m_OcclusionTexture == null || m_OcclusionSun == null || m_OcclusionShader == null)
            {
                ClearOcclusionTexture();
                return;
            }

            var camera = Camera.main;
            if (camera == null)
            {
                ClearOcclusionTexture();
                return;
            }

//#if UNITY_EDITOR
//            if (!camera.TryGetComponent<FlareLayer>(out var flareLayer))
//                Debug.LogError($"Camera '{camera.name}' requires the '{nameof(FlareLayer)}' Component.");
//#endif

            var sunBounds = m_OcclusionSun.bounds;
            var sunDistance = (sunBounds.center - camera.transform.position).magnitude;
            var sunHalfSize = Mathf.Max(sunBounds.extents.x, Mathf.Max(sunBounds.extents.y, sunBounds.extents.z));

            // If the sun is outside the view frustum of the actual scene,
            // don't even try to render the occlusion texture, because the sun is not visible.
            GeometryUtility.CalculateFrustumPlanes(camera, m_FrustumPlanes);
            if (!GeometryUtility.TestPlanesAABB(m_FrustumPlanes, sunBounds))
            {
                ClearOcclusionTexture();
                return;
            }

            // Fit sun object into camera view
            // https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
            m_OcclusionCamera.nearClipPlane = camera.nearClipPlane;
            m_OcclusionCamera.farClipPlane = sunDistance + sunHalfSize;
            m_OcclusionCamera.fieldOfView = 2.0f * Mathf.Atan(sunHalfSize / sunDistance) * Mathf.Rad2Deg;
            m_OcclusionCamera.transform.position = camera.transform.position;
            m_OcclusionCamera.transform.LookAt(m_OcclusionSun.transform, Vector3.up);

            // And finally render the occlusion texture
            m_OcclusionTexture.DiscardContents();
            m_OcclusionCamera.targetTexture = m_OcclusionTexture;
            m_OcclusionCamera.RenderWithShader(m_OcclusionShader, "RenderType");
            m_OcclusionCamera.targetTexture = null;
        }

        void ClearOcclusionTexture()
        {
            if (m_OcclusionCamera == null || m_OcclusionTexture == null)
                return;

            var activeRT = RenderTexture.active;
            try
            {
                m_OcclusionTexture.DiscardContents();
                RenderTexture.active = m_OcclusionTexture;
                GL.Clear(true, true, m_OcclusionCamera.backgroundColor);
            }
            finally
            {
                RenderTexture.active = activeRT;
            }
        }
    }
}
