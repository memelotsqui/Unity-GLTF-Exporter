using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectNodeMirror : ObjectNodeUserExtrasMono
    {
        public ReflectionProbe mirrorReflectionProbe;
        public int mirrorQuality = 70;
        const string texturePreName = "mirr_";

        private void Reset()
        {
            displayOptions = false;
            extrasName = "mirror";
            if (mirrorReflectionProbe != null)
            {
                //this.AddProperty();
            }

            tooltip = "Mirror will reflect objects in front of this mesh, make sure red line is facing forward, and that this object is assigned to an object with mesh component. \n\nMirrors are expensive, dont use many";
        }
        public override void SetGLTFComputedData()
        {
            //if (optionalIdentifier == -1)       // FIRST TIME ONLY, ASSIGN MIRRORS ID
            //{
            //    int counter = 0;
            //    ObjectNodeMirror[] mirrors = transform.root.GetComponentsInChildren<ObjectNodeMirror>();
            //    foreach (ObjectNodeMirror om in mirrors)
            //    {
            //        om.optionalIdentifier = counter;
            //        counter++;
            //    }
            //}
            if (mirrorReflectionProbe != null)
            {
                int _cubemap_index = ObjectExtrasCubeTextures.GetCubemapIndex(mirrorReflectionProbe.bakedTexture as Cubemap, mirrorQuality);
                AddProperty(new ObjectProperty("envMap", _cubemap_index));
            }
        }
        void OnDrawGizmos()
        {

            // Draws a blue line from this transform to the target
            Gizmos.color = Color.red;
            Vector3 direction = transform.TransformDirection(Vector3.forward) * -2;
            Gizmos.DrawRay(transform.position, direction);

        }
    }
}
