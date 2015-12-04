#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
class FMODEmitterGizmo
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmo(FMOD_StudioEventEmitter studioEmitter, GizmoType gizmoType)
    {
        if (studioEmitter.asset != null && studioEmitter.enabled &&
            (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isPlaying))
        {
            FMOD.Studio.EventDescription desc = null;
            desc = FMODEditorExtension.GetEventDescription(studioEmitter.asset.id);

            if (desc != null)
            {
                float max, min;
                desc.getMaximumDistance(out max);
                desc.getMinimumDistance(out min);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(studioEmitter.transform.position, min);
                Gizmos.DrawWireSphere(studioEmitter.transform.position, max);
            }
        }
    }
}
#endif
