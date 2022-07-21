using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectNodeTextContainer : ObjectNodeUserExtrasMono
    {
        public enum JustifyContent { center = 0, start = 1, end = 2}
        public enum AlignContent { center = 0, right = 1 , left = 2} 

        public JustifyContent justifyContent = JustifyContent.center;
        public AlignContent alignContent = AlignContent.left;

        public float padding = 0.02f;
        public float width = 0.1f;
        public float height = 0.1f;

        public float fontSize = 0.075f;

        public string textContent = "";
        private void Reset()
        {
            displayOptions = false;
            extrasName = "textContainer";

            tooltip = "text container will ignore the mesh, width and height must be provided to get the desired size, dor now default font will be used";
        }

        public override void SetGLTFComputedData()
        {
            Debug.Log(width);
            AddProperty(new ObjectProperty("width", width));
            AddProperty(new ObjectProperty("height", height));
            AddProperty(new ObjectProperty("padding", padding));
            AddProperty(new ObjectProperty("justifyContent", justifyContent.ToString()));
            AddProperty(new ObjectProperty("alignContent", alignContent.ToString()));
            AddProperty(new ObjectProperty("fontSize", fontSize));
            AddProperty(new ObjectProperty("textContent", textContent));
        }

        void OnDrawGizmos()
        {

            // Square
            Gizmos.color = Color.red;
            Vector3 direction = transform.TransformDirection(Vector3.forward) * -.1f;
            
            Gizmos.DrawLine(new Vector3(transform.position.x - width / 2f, transform.position.y - height / 2f, transform.position.z),
                new Vector3(transform.position.x - width / 2f, transform.position.y + height / 2f, transform.position.z));

            Gizmos.DrawLine(new Vector3(transform.position.x + width / 2f, transform.position.y - height / 2f, transform.position.z),
                new Vector3(transform.position.x + width / 2f, transform.position.y + height / 2f, transform.position.z));

            Gizmos.DrawLine(new Vector3(transform.position.x - width / 2f, transform.position.y + height / 2f, transform.position.z),
                new Vector3(transform.position.x + width / 2f, transform.position.y + height / 2f, transform.position.z));

            Gizmos.DrawLine(new Vector3(transform.position.x - width / 2f, transform.position.y - height / 2f, transform.position.z),
                new Vector3(transform.position.x + width / 2f, transform.position.y - height / 2f, transform.position.z));

            // Direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, direction);

            // Font Size
            Gizmos.color = Color.blue;
            float xMove = 0f;
            switch (alignContent)
            {
                case AlignContent.left:
                    xMove = -(width / 2)+padding;
                    break;
                case AlignContent.right:
                    xMove = (width / 2)-padding;
                    break;
            }
            float yMove = 0f;
            switch (justifyContent)
            {
                case JustifyContent.start:
                    //yMove = (height / 2) - fontSize;
                    yMove = (height/2) - (fontSize/2)-padding;
                    break;
                case JustifyContent.end:
                    yMove = -(height / 2) + (fontSize / 2) + padding;
                    break;
            }

            Gizmos.DrawLine(new Vector3(transform.position.x + xMove, (transform.position.y + fontSize / 2f) + yMove, transform.position.z),
                new Vector3(transform.position.x + xMove, (transform.position.y - fontSize / 2f) + yMove, transform.position.z));

        }
    }
}
