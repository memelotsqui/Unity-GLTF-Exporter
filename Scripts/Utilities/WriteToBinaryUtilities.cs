using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using WEBGL_EXPORTER.GLTF;
namespace WEBGL_EXPORTER
{
    public class WriteToBinaryUtilities
    {
        private static BinaryWriter writer;
        private static int byteLength;
        public static void OpenNewBinaryFile(string fileName, string filePath, string extension = "bin")
        {
            if (!filePath.EndsWith("/"))
                filePath += "/";
            writer = new BinaryWriter(File.Open(filePath + fileName + "." + extension, FileMode.Create));
            byteLength = 0;
        }
        public static void WriteBinaryWithComponentType()
        {

        }
        // UNSIGNED SHORT
        public static void WriteUnsignedShorts(uint [] uintArray)
        {
            byteLength += uintArray.Length * 2;
            foreach (uint i in uintArray)
            {
                
                writer.Write((ushort)i);
            }
        }
        public static void WriteUnsignedShorts(int[] intArray)
        {
            byteLength += intArray.Length * 2;      //ushorts are made of 2 bytes
            foreach (int i in intArray)
            {
                writer.Write((ushort)i);
            }
        }
        // END USIGNED SHORT
        // UNSIGNED BYTE
        public static void WriteUnsignedBytes(uint[] uintArray)
        {
            byteLength += uintArray.Length;
            foreach (uint i in uintArray)
            {

                writer.Write((byte)i);
            }
        }
        public static void WriteUnsignedBytes(int[] intArray)
        {
            byteLength += intArray.Length;      //bytes are made of 1 bytes
            foreach (int i in intArray)
            {
                writer.Write((byte)i);
            }
        }
        // END USIGNED BYTE
        // FLOATS
        public static void WriteFloats(float[] floatArray)
        {
            byteLength += floatArray.Length * 4;
            foreach (float i in floatArray)
            {
                writer.Write(i);
            }
        }
        // END FLOATS
        // FLOATS
        public static void WriteInts(int[] intArray)
        {
            byteLength += intArray.Length * 4;
            foreach (float i in intArray)
            {
                writer.Write(i);
            }
        }
        // END FLOATS
        // MATRIX
        public static void WriteMatrix4(Matrix4x4[] matrix_4s)
        {
            Debug.Log("CHECK HERE IF SAVING CORRECTLY THE MATRIX");
            byteLength += matrix_4s.Length * 64;      //float are made of 4 bytes, and matrices are made of 16 float (16*4 = 64)
            foreach (Matrix4x4 m4 in matrix_4s)
            {
                writer.Write(m4.m00); writer.Write(m4.m10); writer.Write(m4.m20); writer.Write(m4.m30);
                writer.Write(m4.m01); writer.Write(m4.m11); writer.Write(m4.m21); writer.Write(m4.m31);
                writer.Write(m4.m02); writer.Write(m4.m12); writer.Write(m4.m22); writer.Write(m4.m32);
                writer.Write(m4.m03); writer.Write(m4.m13); writer.Write(m4.m23); writer.Write(m4.m33);
            }

        }
        // VECTORS
        public static void WriteVector4(Vector4[] vectors, ComponentType component_type = ComponentType.FLOAT)
        {
            if (component_type == ComponentType.FLOAT || component_type == ComponentType.NONE_SET)
            {
                byteLength += vectors.Length * 16;      //float are made of 4 bytes, and vectors are made of 3 float (3*4 = 12)
                foreach (Vector4 v4 in vectors)
                {
                    writer.Write(v4.x);
                    writer.Write(v4.y);
                    writer.Write(v4.z);
                    writer.Write(v4.w);
                }
            }
            if (component_type == ComponentType.UNSIGNED_SHORT || component_type == ComponentType.SHORT)
            {
                byteLength += vectors.Length * 8;      //ushorts are made of 2 bytes, and vectors are made of 4 float (4*2 = 8)
                foreach (Vector4 v4 in vectors)
                {
                    //Debug.Log("vertex");
                    writer.Write((ushort)v4.x);
                    writer.Write((ushort)v4.y);
                    writer.Write((ushort)v4.z);
                    writer.Write((ushort)v4.w);
                }
            }
            if (component_type == ComponentType.UNSIGNED_BYTE || component_type == ComponentType.BYTE)
            {
                byteLength += vectors.Length * 4;      //bytes are made of 1 bytes, and vectors are made of 4 float (4*1 = 4)
                foreach (Vector4 v4 in vectors)
                {
                    //Debug.Log("normal");
                    writer.Write((byte)v4.x);
                    writer.Write((byte)v4.y);
                    writer.Write((byte)v4.z);
                    writer.Write((byte)v4.w);
                }
            }
        }
        public static void WriteVector3(Vector3[] vectors, ComponentType component_type = ComponentType.FLOAT)
        {
            if (component_type == ComponentType.FLOAT || component_type == ComponentType.NONE_SET)
            {
                byteLength += vectors.Length * 12;      //float are made of 4 bytes, and vectors are made of 3 float (3*4 = 12)
                foreach (Vector3 v3 in vectors)
                {
                    writer.Write(v3.x);
                    writer.Write(v3.y);
                    writer.Write(v3.z);
                }
            }
            if (component_type == ComponentType.UNSIGNED_SHORT || component_type == ComponentType.SHORT)
            {
                byteLength += vectors.Length * 8;      //ushorts are made of 2 bytes, and vectors are made of 3 float (3*2 = 6) + 2 to keep boundary of 4
                foreach (Vector3 v3 in vectors)
                {
                    //Debug.Log("vertex");
                    writer.Write((ushort)v3.x);
                    writer.Write((ushort)v3.y);
                    writer.Write((ushort)v3.z);
                    WriteEmptyDuo();
                }
            }
            if (component_type == ComponentType.UNSIGNED_BYTE || component_type == ComponentType.BYTE)
            {
                byteLength += vectors.Length * 4;      //bytes are made of 1 bytes, and vectors are made of 3 float (3*1 = 3) + 1 to keep boundary of 4
                foreach (Vector3 v3 in vectors)
                {
                    //Debug.Log("normal");
                    writer.Write((byte)v3.x);
                    writer.Write((byte)v3.y);
                    writer.Write((byte)v3.z);
                    WriteEmptySingle();                       //keep boundary of 4
                }
            }
        }
        public static void WriteVector2(Vector2[] vectors, ComponentType component_type = ComponentType.FLOAT)
        {
            if (component_type == ComponentType.FLOAT || component_type == ComponentType.NONE_SET)
            {
                byteLength += vectors.Length * 8;      //float are made of 4 bytes, and vectors are made of 3 float (2*4 = 8)
                foreach (Vector2 v2 in vectors)
                {
                    writer.Write(v2.x);
                    writer.Write(v2.y);
                }
            }
            if (component_type == ComponentType.UNSIGNED_SHORT || component_type == ComponentType.SHORT)
            {
                byteLength += vectors.Length * 4;      //ushorts are made of 2 bytes, and vectors are made of 2 float (2*2 = 4)
                foreach (Vector2 v2 in vectors)
                {
                    writer.Write((ushort)v2.x);
                    writer.Write((ushort)v2.y);
                }
            }
            if (component_type == ComponentType.UNSIGNED_BYTE || component_type == ComponentType.BYTE)
            {
                byteLength += vectors.Length * 2;      //bytes are made of 1 bytes, and vectors are made of 2 float (2*1 = 2)
                foreach (Vector2 v2 in vectors)
                {
                    writer.Write((byte)v2.x);
                    writer.Write((byte)v2.y);
                }
            }

        }
        // SPACING
        public static void WriteEmptySingle(int qty = 1)
        {
            for (int i = 0; i < qty; i++)
            {
                writer.Write((byte)0);
            }
        }
        public static void WriteEmptyDuo()
        {
            writer.Write((short)0);
        }
        public static int GetByteLenght()
        {
            return byteLength;
        }
        public static void CloseBinaryFile()
        {
            
            writer.Close();
        }
    }
}
