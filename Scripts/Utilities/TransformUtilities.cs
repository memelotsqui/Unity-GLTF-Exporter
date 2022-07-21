using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtilities { 
    /// <summary>
    /// Converts target unity matrix to gltf matrix
    /// </summary>
    /// <param name="targetMatrix"></param>
    /// <returns></returns>
    public static Matrix4x4 GetGLTFMatrix(Matrix4x4 targetMatrix)
    {
        // MODIFY THE MATRIX TO HAVE NEGATIVE Z IN TRANSLATION AND X,Y ROTATION NEGATIVE

        //return new Matrix4x4(new Vector4(targetMatrix.m00, targetMatrix.m01, -targetMatrix.m02, targetMatrix.m03),
        //                    new Vector4(targetMatrix.m10, targetMatrix.m11, -targetMatrix.m12, targetMatrix.m13),
        //                    new Vector4(-targetMatrix.m20, -targetMatrix.m21, targetMatrix.m22, -targetMatrix.m23),
        //                    new Vector4(targetMatrix.m30, targetMatrix.m31, targetMatrix.m32, targetMatrix.m33));


        Matrix4x4 result = new Matrix4x4(new Vector4(targetMatrix.m00, targetMatrix.m10, -targetMatrix.m20, targetMatrix.m30),
                                    new Vector4(targetMatrix.m01, targetMatrix.m11, -targetMatrix.m21, targetMatrix.m31),
                                    new Vector4(-targetMatrix.m02, -targetMatrix.m12, targetMatrix.m22, targetMatrix.m32),
                                    new Vector4(targetMatrix.m03, targetMatrix.m13, -targetMatrix.m23, targetMatrix.m33));

        return result;

        //Matrix4x4 result = new Matrix4x4(new Vector4(targetMatrix.m00, targetMatrix.m10, -targetMatrix.m20, targetMatrix.m30),
        //                                new Vector4(targetMatrix.m01, targetMatrix.m11, -targetMatrix.m21, targetMatrix.m31),
        //                                new Vector4(-targetMatrix.m02, -targetMatrix.m12, targetMatrix.m22, targetMatrix.m32),
        //                                new Vector4(targetMatrix.m03, targetMatrix.m13, -targetMatrix.m23, targetMatrix.m33));

        //return result;

        //return new Matrix4x4(new Vector4(result.m00, result.m01, result.m02, result.m03),
        //                    new Vector4(result.m10, result.m11, result.m12, result.m13),
        //                    new Vector4(result.m20, result.m21, result.m22, result.m23),
        //                    new Vector4(result.m30, result.m31, result.m32, result.m33));


        //return result;

    }
    //public static Matrix4x4 GetRelativeMatrix(Matrix4x4 targetMatrix)
    //{

    //}
}
