using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectAccessors
    {
        //statics
        public static int globalIndex = -1;
        public static List<ObjectAccessors> allUniqueAccessors;
        public static List<ObjectAccessors> sortedAccessorsList;
        public static bool sortedElements = false;

        //vars
        //on creation
        public int start_index = -1;
        public ComponentType componentType;
        public Type type;
        public Min_Max min_max;
        //public Min_Max min_max_quantization;
        public float[] quantization_scale;
        public float[] quantization_offset;
        public bool isQuantized = false;
        string optionaIdentifier = "";

        //type of information
        int[] scalarInt;
        float[] scalarFloat;
        Vector2[] vector2Info;
        Vector3[] vector3Info;
        Vector4[] vector4Info;

        Matrix4x4[] matrix4Info;

        public int export_index = -1;
        public ObjectBufferView bufferView;
        public int byteSize;
        public int byteOffset;
        
        public bool normalized = false;
        public int accessorCount = 0;

        //sparse not supported atm

        public static void Reset()
        {
            globalIndex = 0;
            allUniqueAccessors = new List<ObjectAccessors>();
            sortedAccessorsList = new List<ObjectAccessors>();
            sortedElements = false;
        }

        // ACCESSORS CREATION
        public ObjectAccessors(int[] int_values, bool req_min_max = false, bool unsigned = true, string optional_identifier = "")
        {//
            start_index = globalIndex;
            scalarInt = int_values;
            

            Min_Max min_max_info = new Min_Max(int_values);
            type = Type.SCALAR;
            if (req_min_max)
                min_max = min_max_info;

            if (Mathf.RoundToInt(min_max_info.max[0]) < 255)
            {
                byteSize = int_values.Length;               // UNSIGNED BYTES REQUIRE 1 BYTES EACH
                if (unsigned)
                    componentType = ComponentType.UNSIGNED_BYTE;
                else
                    componentType = ComponentType.BYTE;
            }
            else if (Mathf.RoundToInt(min_max_info.max[0]) < 65535)
            {
                byteSize = int_values.Length * 2;           // UNSIGNED SHORT REQUIRE 2 BYTES EACH
                if (unsigned)
                    componentType = ComponentType.UNSIGNED_SHORT;
                else
                    componentType = ComponentType.SHORT;
            }
            else
            {
                Debug.LogError("Mesh indices are above 65535, errors will occur, please reduce mesh size in 3d application");
                byteSize = int_values.Length * 4;           // UNSIGNED INT REQUIRE 4 BYTES EACH
                if (unsigned)
                    componentType = ComponentType.UNSIGNED_INT;
                else
                    componentType = ComponentType.FLOAT;
            }

            optionaIdentifier = optional_identifier;
            accessorCount = int_values.Length;

            allUniqueAccessors.Add(this);
            globalIndex++;
        }
        public ObjectAccessors(float[] float_values, bool req_min_max = false, string optional_identifier = "")
        {
            start_index = globalIndex;
            scalarFloat = float_values;
            byteSize = float_values.Length * 4;         // FLOAT REQUIRE 4 BYTES

           componentType = ComponentType.FLOAT;
            type = Type.SCALAR;
            if (req_min_max)
                min_max = new Min_Max(float_values);
            optionaIdentifier = optional_identifier;
            normalized = false;
            accessorCount = float_values.Length;

            allUniqueAccessors.Add(this);
            globalIndex++;
        }
        public ObjectAccessors(Vector2[] v2_values, bool req_min_max = false, string optional_identifier = "", ComponentType quantize_type = ComponentType.NONE_SET, bool quantize_change_values = true)
        {
            start_index = globalIndex;
            Min_Max min_max_quantization = new Min_Max(v2_values);
            vector2Info = new Vector2[v2_values.Length];
            bool _saved = false;

            if (quantize_type != ComponentType.NONE_SET && quantize_type != ComponentType.FLOAT)
            {
                // VALUES IN QUANTIZATION MUTS BE NORMALIZED
                if (quantize_change_values == true)
                {
                    // DO IT NO MATTER ANYTHING
                    componentType = quantize_type;
                    for (int i = 0; i < vector2Info.Length; i++)
                    {
                        vector2Info[i] = new Vector2(GetQuantizeVal(v2_values[i].x, min_max_quantization.min[0], min_max_quantization.max[0], quantize_type, true),
                                                    GetQuantizeVal(v2_values[i].y, min_max_quantization.min[1], min_max_quantization.max[1], quantize_type, true));
                    }
                    // SAVE THE SCALE AND OFFSET VALUES FOR DECOMPRESSION
                    quantization_scale = min_max_quantization.GetQuantizationScale();
                    quantization_offset = min_max_quantization.GetQuantizationOffset();
                    _saved = true;
                }
                // IF VALUES HAVENT BEEN SAVED THEN:
                if (!_saved)
                {
                    // IF COMPONENT TYPE IS USHORT/UBYTE, MAKE SURE MAX RANGE VALUE IS 1
                    if (quantize_type == ComponentType.UNSIGNED_BYTE || quantize_type == ComponentType.UNSIGNED_SHORT) 
                    {
                        if (min_max_quantization.GetQuantizationScale()[0] <= 1 && min_max_quantization.GetQuantizationScale()[1] <= 1)
                        {
                            for (int i = 0; i < vector2Info.Length; i++)                                                                 //SAVE IT AS GIVEN NORMALIZED FROM 0 TO 1
                            {
                                float _xval = v2_values[i].x - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[0]);       // MAKE SURE VALUE GO FROM 0 TO 1
                                float _yval = v2_values[i].y - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[1]);
                                vector2Info[i] = new Vector2(GetQuantizeVal(_xval, 0, 1, quantize_type, false),
                                                            GetQuantizeVal(_yval, 0, 1, quantize_type, false));
                                
                            }
                            
                            _saved = true;
                        }
                    }

                    // IF COMPONENT TYPE IS SHORT/BYTE, MAKE SURE MAX RANGE VALUE IS 2
                    if (quantize_type == ComponentType.BYTE || quantize_type == ComponentType.SHORT)
                    {
                        if (min_max_quantization.GetQuantizationScale()[0] <= 2 && min_max_quantization.GetQuantizationScale()[1] <= 2)
                        {
                            // TO DO... SAVE VALUES FROM -1 TO 1
                        }
                    }
                        
                }

                if (_saved)         // IF UP TO HERE, VALUES WERE SAVED, STORE THE ADDITIONAL QUANTIZATION DATA NEEDED.
                {
                    //BYTE SIZE IS SAVED LIKE THIS, SINCE NO 4 BOUNDARIES IN VECTOR2 IS NECESSARY
                    if (quantize_type == ComponentType.UNSIGNED_BYTE)
                        byteSize = v2_values.Length * 2;
                    if (quantize_type == ComponentType.UNSIGNED_SHORT)
                        byteSize = v2_values.Length * 4;

                    componentType = quantize_type;
                    normalized = true;
                    isQuantized = true;
                }
            }

            if (!_saved)            // IF UP TO HERE NO DATA WAS SAVED, THEN STORE FLOAT VECTOR2 
            {
                vector2Info = v2_values;
                byteSize = v2_values.Length * 8;            // VECTOR 2 REQUIRE 2 FLOAT (4 BYTES EACH) 2*4=8
                componentType = ComponentType.FLOAT;
                normalized = false;
                
            }

            if (req_min_max)
                min_max = new Min_Max(vector2Info);
            type = Type.VEC2;
            optionaIdentifier = optional_identifier;
            accessorCount = v2_values.Length;
            allUniqueAccessors.Add(this);
            globalIndex++;
        }

        public ObjectAccessors(Vector3[] v3_values, Vector3 v3_modifier, bool req_min_max = false, string optional_identifier = "", ComponentType quantize_type = ComponentType.NONE_SET, bool quantize_change_values = true)
        {
            start_index = globalIndex;
            Min_Max min_max_quantization = new Min_Max(v3_values, v3_modifier);
            vector3Info = new Vector3[v3_values.Length];
            bool _saved = false;

            if (quantize_type != ComponentType.NONE_SET && quantize_type != ComponentType.FLOAT)
            {
                if (quantize_change_values)
                {
                    for (int i = 0; i < vector3Info.Length; i++)
                    {
                        vector3Info[i] = new Vector3(GetQuantizeVal(v3_values[i].x * v3_modifier.x, min_max_quantization.min[0], min_max_quantization.max[0], quantize_type, true),
                                                    GetQuantizeVal(v3_values[i].y * v3_modifier.y, min_max_quantization.min[1], min_max_quantization.max[1], quantize_type, true),
                                                    GetQuantizeVal(v3_values[i].z * v3_modifier.z, min_max_quantization.min[2], min_max_quantization.max[2], quantize_type, true));
                    }
                    // SAVE THE SCALE AND OFFSET VALUES FOR DECOMPRESSION
                    quantization_scale = min_max_quantization.GetQuantizationScale();
                    quantization_offset = min_max_quantization.GetQuantizationOffset();
                    _saved = true;
                }

                if (!_saved)
                {
                    // IF COMPONENT TYPE IS USHORT/UBYTE, MAKE SURE MAX RANGE VALUE IS 1
                    if (quantize_type == ComponentType.UNSIGNED_BYTE || quantize_type == ComponentType.UNSIGNED_SHORT)
                    {
                        if (min_max_quantization.GetQuantizationScale()[0] <= 1 && min_max_quantization.GetQuantizationScale()[1] <= 1)
                        {
                            for (int i = 0; i < vector3Info.Length; i++)                                                                 //SAVE IT AS GIVEN NORMALIZED FROM 0 TO 1
                            {
                                float _xval = (v3_values[i].x * v3_modifier.x) - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[0]);       // MAKE SURE VALUE GO FROM 0 TO 1
                                float _yval = (v3_values[i].y * v3_modifier.y) - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[1]);
                                float _zval = (v3_values[i].z * v3_modifier.z) - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[2]);
                                vector3Info[i] = new Vector3(GetQuantizeVal(_xval, 0, 1, quantize_type, false),
                                                            GetQuantizeVal(_yval, 0, 1, quantize_type, false),
                                                            GetQuantizeVal(_zval, 0, 1, quantize_type, false));

                            }
                            _saved = true;
                        }
                    }

                    // IF COMPONENT TYPE IS SHORT/BYTE, MAKE SURE MAX RANGE VALUE IS 2
                    if (quantize_type == ComponentType.BYTE || quantize_type == ComponentType.SHORT)
                    {
                        //if (min_max_quantization.GetQuantizationScale()[0] <= 2 && min_max_quantization.GetQuantizationScale()[1] <= 2)
                        //{
                            for (int i = 0; i < vector3Info.Length; i++)                                                                 //SAVE IT AS GIVEN NORMALIZED FROM 0 TO 1
                            {
                                // TO DO... CHECK IF VALUES ARE FROM -1 TO 1
                                vector3Info[i] = new Vector3(GetQuantizeVal(v3_values[i].x * v3_modifier.x, 0, 1, quantize_type, false),
                                                            GetQuantizeVal(v3_values[i].y * v3_modifier.y, 0, 1, quantize_type, false),
                                                            GetQuantizeVal(v3_values[i].z * v3_modifier.z, 0, 1, quantize_type, false));
                            }
                            _saved = true;
                        //} 
                    }
                }
                if (_saved)         // IF UP TO HERE, VALUES WERE SAVED, STORE THE ADDITIONAL QUANTIZATION DATA NEEDED.
                {
                    componentType = quantize_type;
                    byteSize = v3_values.Length * GetByteSize(quantize_type, 3);
                    normalized = true;
                    isQuantized = true;
                }
            }

            if (!_saved)
            {
                for (int i =0; i < vector3Info.Length; i++)
                {
                    vector3Info[i] = new Vector3(v3_values[i].x * v3_modifier.x, v3_values[i].y * v3_modifier.y, v3_values[i].z * v3_modifier.z);
                }
                byteSize = v3_values.Length * 12;                   // VECTOR 3 REQUIRE 3 FLOAT (4 BYTES EACH) 3*4=12
                componentType = ComponentType.FLOAT;
                normalized = false;
            }


            if (req_min_max)
                min_max = new Min_Max(vector3Info, new Vector3(1,1,1)); // it no longer requires modifier, their values were modified previously

            type = Type.VEC3;
            optionaIdentifier = optional_identifier;
            accessorCount = v3_values.Length;
            allUniqueAccessors.Add(this);
            globalIndex++;

            //if (quantize_type == ComponentType.NONE_SET || quantize_type == ComponentType.FLOAT)
            //{                                                       // NON QUANTIZED VALUES
            //    vector3Info = v3_values;
            //    byteSize = v3_values.Length * 12;                   // VECTOR 3 REQUIRE 3 FLOAT (4 BYTES EACH) 3*4=12
            //    componentType = ComponentType.FLOAT;
            //    normalized = false;
            //}
            //else
            //{                                                       // QUANTIZED VALUES
            //    vector3Info = new Vector3[v3_values.Length];
            //    Min_Max min_max_quantization = new Min_Max(v3_values);      // SAVE ORIGINAL VALUES
            //    for (int i =0; i < vector3Info.Length; i++)
            //    {
            //        vector3Info[i] = new Vector3(GetQuantizeVal(v3_values[i].x, min_max_quantization.min[0], min_max_quantization.max[0], quantize_type, quantize_change_values),
            //                                    GetQuantizeVal(v3_values[i].y, min_max_quantization.min[1], min_max_quantization.max[1], quantize_type, quantize_change_values),
            //                                    GetQuantizeVal(v3_values[i].z, min_max_quantization.min[2], min_max_quantization.max[2], quantize_type, quantize_change_values));
            //    }
            //    if (quantize_change_values)
            //    {
            //        quantization_scale = min_max_quantization.GetQuantizationScale();
            //        quantization_offset = min_max_quantization.GetQuantizationOffset();
            //    }
            //    byteSize = v3_values.Length * GetByteSize(quantize_type, 3);
            //    componentType = quantize_type;
            //    normalized = true;
            //    isQuantized = true;
            //}
            //if (req_min_max)
            //    min_max = new Min_Max(vector3Info);
            //type = Type.VEC3;

            //optionaIdentifier = optional_identifier;
            
            //accessorCount = v3_values.Length;

            //allUniqueAccessors.Add(this);
            //globalIndex++;
        }
        //END ACCESSOR CREATION
        // VECTOR 4 AS INT
        public ObjectAccessors(Vector4[] v4_values, string optional_identifier = "", ComponentType component_type = ComponentType.BEST_INT)
        {
            start_index = globalIndex;
            Min_Max min_max = new Min_Max(v4_values, new Vector4(1f, 1f, 1f, 1f));
            vector4Info = new Vector4[v4_values.Length];


            // SAVE VALUE AS IT IS!
            if (IsIntType(component_type))
            {
                Debug.Log("alskjndlkas");
                for (int i = 0; i < v4_values.Length; i++)
                {
                    
                    vector4Info[i] = new Vector4(v4_values[i].x,
                                                v4_values[i].y,
                                                v4_values[i].z,
                                                v4_values[i].w);
                    Debug.Log(vector4Info[i].x);
                    //Debug.Log(vector4Info[i]);
                }

                if (component_type == ComponentType.BEST_INT) componentType = GetBestInt(min_max.GetMinValueFromVectors(), min_max.GetMaxValueFromVectors());
                else componentType = component_type;

                //Debug.Log(componentType);
                byteSize = v4_values.Length * GetByteSize(componentType, 4);
                normalized = false;
                isQuantized = false;

            }

            type = Type.VEC4;
            optionaIdentifier = optional_identifier;
            accessorCount = v4_values.Length;
            allUniqueAccessors.Add(this);
            globalIndex++;

        }
        // VECTOR 4 AS FLOAT
        public ObjectAccessors(Vector4[] v4_values, Vector4 v4_modifier, bool req_min_max = false, string optional_identifier = "", ComponentType quantize_type = ComponentType.NONE_SET, bool quantize_change_values = true)
        {
            start_index = globalIndex;
            Min_Max min_max_quantization = new Min_Max(v4_values, v4_modifier);
            vector4Info = new Vector4[v4_values.Length];
            bool _saved = false;

            if (quantize_type != ComponentType.NONE_SET && quantize_type != ComponentType.FLOAT)
            {
                if (quantize_change_values)
                {
                    for (int i = 0; i < vector4Info.Length; i++)
                    {
                        vector4Info[i] = new Vector4(GetQuantizeVal(v4_values[i].x * v4_modifier.x, min_max_quantization.min[0], min_max_quantization.max[0], quantize_type, true),
                                                    GetQuantizeVal(v4_values[i].y * v4_modifier.y, min_max_quantization.min[1], min_max_quantization.max[1], quantize_type, true),
                                                    GetQuantizeVal(v4_values[i].z * v4_modifier.z, min_max_quantization.min[2], min_max_quantization.max[2], quantize_type, true),
                                                    GetQuantizeVal(v4_values[i].w * v4_modifier.w, min_max_quantization.min[2], min_max_quantization.max[2], quantize_type, true));
                    }
                    // SAVE THE SCALE AND OFFSET VALUES FOR DECOMPRESSION
                    quantization_scale = min_max_quantization.GetQuantizationScale();
                    quantization_offset = min_max_quantization.GetQuantizationOffset();
                    _saved = true;
                }

                if (!_saved)
                {
                    // IF COMPONENT TYPE IS USHORT/UBYTE, MAKE SURE MAX RANGE VALUE IS 1
                    if (quantize_type == ComponentType.UNSIGNED_BYTE || quantize_type == ComponentType.UNSIGNED_SHORT)
                    {
                        
                        if (min_max_quantization.GetQuantizationScale()[0] <= 1 && min_max_quantization.GetQuantizationScale()[1] <= 1)
                        {
                            int quantVal = quantize_type == ComponentType.UNSIGNED_BYTE ? 256 : 65536;
                            for (int i = 0; i < vector4Info.Length; i++)                                                                 //SAVE IT AS GIVEN NORMALIZED FROM 0 TO 1
                            {
                                float _xval = (v4_values[i].x * v4_modifier.x) - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[0]);       // MAKE SURE VALUE GO FROM 0 TO 1
                                float _yval = (v4_values[i].y * v4_modifier.y) - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[1]);
                                float _zval = (v4_values[i].z * v4_modifier.z) - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[2]);
                                float _wval = (v4_values[i].w * v4_modifier.w) - Mathf.FloorToInt(min_max_quantization.GetQuantizationOffset()[3]);
                                vector4Info[i] = new Vector4(GetQuantizeVal(_xval, 0, 1, quantize_type, false),
                                                            GetQuantizeVal(_yval, 0, 1, quantize_type, false),
                                                            GetQuantizeVal(_zval, 0, 1, quantize_type, false),
                                                            GetQuantizeVal(_wval, 0, 1, quantize_type, false));
                                //-vector4Info[i].x - vector4Info[i].y - vector4Info[i].z + quantVal);
                               
                            }
                            _saved = true;
                        }
                    }

                    // IF COMPONENT TYPE IS SHORT/BYTE, MAKE SURE MAX RANGE VALUE IS 2
                    if (quantize_type == ComponentType.BYTE || quantize_type == ComponentType.SHORT)
                    {
                        //if (min_max_quantization.GetQuantizationScale()[0] <= 2 && min_max_quantization.GetQuantizationScale()[1] <= 2)
                        //{
                        for (int i = 0; i < vector4Info.Length; i++)                                                                 //SAVE IT AS GIVEN NORMALIZED FROM 0 TO 1
                        {
                            // TO DO... CHECK IF VALUES ARE FROM -1 TO 1
                            vector4Info[i] = new Vector4(GetQuantizeVal(v4_values[i].x * v4_modifier.x, 0, 1, quantize_type, false),
                                                        GetQuantizeVal(v4_values[i].y * v4_modifier.y, 0, 1, quantize_type, false),
                                                        GetQuantizeVal(v4_values[i].z * v4_modifier.z, 0, 1, quantize_type, false),
                                                        GetQuantizeVal(v4_values[i].w * v4_modifier.w, 0, 1, quantize_type, false));
                        }
                        _saved = true;
                        //} 
                    }
                }
                if (_saved)         // IF UP TO HERE, VALUES WERE SAVED, STORE THE ADDITIONAL QUANTIZATION DATA NEEDED.
                {
                    componentType = quantize_type;
                    byteSize = v4_values.Length * GetByteSize(quantize_type, 4);
                    normalized = true;
                    isQuantized = true;
                }
            }

            if (!_saved)
            {
                for (int i = 0; i < vector4Info.Length; i++)
                {
                    vector4Info[i] = new Vector4(v4_values[i].x * v4_modifier.x, v4_values[i].y * v4_modifier.y, v4_values[i].z * v4_modifier.z, v4_values[i].w * v4_modifier.w);
                }
                byteSize = v4_values.Length * 16;                   // VECTOR 4 REQUIRE 4 FLOAT (4 BYTES EACH) 4*4=16
                componentType = ComponentType.FLOAT;
                normalized = false;
            }

            Debug.LogWarning("weights");
            //for (int i = 0; i < vector4Info.Length; i++)
            //{
            //    Debug.Log(vector4Info[i] + "   =   " + v4_values[i].x +"," + v4_values[i].y + "," + v4_values[i].z + "," + v4_values[i].w);
            //}

            if (req_min_max)
                min_max = new Min_Max(vector4Info, new Vector3(1, 1, 1)); // it no longer requires modifier, their values were modified previously

            type = Type.VEC4;
            optionaIdentifier = optional_identifier;
            accessorCount = v4_values.Length;
            allUniqueAccessors.Add(this);
            globalIndex++;
        }
        private ComponentType GetBestInt(float min, float max)
        {
            if (min < 0)
            {
                //signed int
                if (min > -127 && max < 127)
                {
                    return ComponentType.BYTE;
                }
                if (min > -32767 && max < 32767)
                {
                    return ComponentType.SHORT;
                }
                return ComponentType.FLOAT;

            }
            else
            {
                //unsigned
                if (max < 255)
                {
                    return ComponentType.UNSIGNED_BYTE;
                }
                if (max < 65535)
                {
                    return ComponentType.UNSIGNED_SHORT;
                }
                return ComponentType.UNSIGNED_INT;
            }
        }

        private bool IsIntType(ComponentType comp_type)
        {
            if (comp_type == ComponentType.BEST_INT ||
                comp_type == ComponentType.BYTE ||
                comp_type == ComponentType.UNSIGNED_BYTE ||
                comp_type == ComponentType.SHORT ||
                comp_type == ComponentType.UNSIGNED_SHORT ||
                comp_type == ComponentType.UNSIGNED_INT)
                return true;
            return false;
        }

        

        public ObjectAccessors(Matrix4x4[] matrix_4_values, string optional_identifier = "")
        {
            start_index = globalIndex;
            matrix4Info = matrix_4_values;
     
            byteSize = matrix_4_values.Length * 64;                 // 4X4 MATRIX REQQUIRES 16 FLOATS(4) TOTAL OF 16X4=64
            componentType = ComponentType.FLOAT;
            normalized = false;

            type = Type.MAT4;
            optionaIdentifier = optional_identifier;
            accessorCount = matrix_4_values.Length;
            allUniqueAccessors.Add(this);
            globalIndex++;
        }

        private int GetByteSize(ComponentType component_type, int _values)
        {
            int result = _values;
            if (component_type == ComponentType.BYTE || component_type == ComponentType.UNSIGNED_BYTE)
            {
                result *= 1;
            }
            if (component_type == ComponentType.UNSIGNED_SHORT || component_type == ComponentType.SHORT)
            {
                result *= 2;
            }
            if (component_type == ComponentType.FLOAT || component_type == ComponentType.UNSIGNED_INT)
            {
                result *= 4;
            }

            int _mod = result % 4;
            if (_mod != 0)
            {
                result += (4-_mod);
            }
            return result;
            //Debug.LogWarning("INCORRECT COMPONENT TYPE, PLEASE FIX");
        }


        private int GetQuantizeVal(float _val, float _min, float _max, ComponentType component_type, bool normalize_val)
        {
            // DOCS: https://github.com/KhronosGroup/glTF/blob/master/extensions/2.0/Khronos/KHR_mesh_quantization/README.md

            if (normalize_val && _min != _max)          // WE MUST MAKE SURE MIN AND MAX ARE DIFFERENT, ELSE WE WILL GET 0
            {
                if (component_type == ComponentType.UNSIGNED_BYTE || component_type == ComponentType.UNSIGNED_SHORT)
                    _val = Min_Max.Normalize(_val, _min, _max);         // VALUE WILL GO FROM 0 TO 1
                if (component_type == ComponentType.BYTE || component_type == ComponentType.SHORT)
                    _val = Min_Max.Normalize(_val, _min, _max,true);    // VALUE WILL GO FROM -1 TO 1
            }
            if (normalize_val && _min == _max)
            {
                _val = 0;
            }

            switch (component_type)
            {
                case ComponentType.BYTE:
                    return Mathf.RoundToInt(_val * 127.0f);
                case ComponentType.UNSIGNED_BYTE:
                    return Mathf.RoundToInt(_val * 255.0f);
                case ComponentType.SHORT:
                    return Mathf.RoundToInt(_val * 32767.0f);
                case ComponentType.UNSIGNED_SHORT:
                    return Mathf.RoundToInt(_val * 65535.0f);
            }
            Debug.LogWarning("WRONG COMPONENT TYPE IN QUANTIZATION VALUE, PLEASE FIX");
            return Mathf.RoundToInt(_val);

        }


        //WHEN WRITING TO BINARY WE WILL SORT ELEMENTS TOO, THIS FUNCTION MUST BE CALLED BEFORE ANYTHING ELSE
        public static void WriteToBinary(string file_name, string file_location)
        {
            WriteToBinaryUtilities.OpenNewBinaryFile(file_name, file_location);
            WriteToBinaryByType();
            WriteToBinaryUtilities.CloseBinaryFile();
        }
        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "\"accessors\": [";
            if (sortedAccessorsList != null)
            {
                foreach (ObjectAccessors accessors in sortedAccessorsList)
                {
                    result += accessors.GetAccessorData() + ",\n";
                }
            }
            result = StringUtilities.RemoveCharacterFromString(result, 2, false);
            result += "]";
            if (add_end_comma)
                result += ",\n";
            return result;
        }
        private static void WriteToBinaryByType()
        {
            List<ObjectAccessors> scalars_byte = new List<ObjectAccessors>();
            List<ObjectAccessors> scalars_short = new List<ObjectAccessors>();
            List<ObjectAccessors> scalars_float = new List<ObjectAccessors>();
            List<ObjectAccessors> scalars_int = new List<ObjectAccessors>();
            List<ObjectAccessors> vertices = new List<ObjectAccessors>();
            List<ObjectAccessors> joints = new List<ObjectAccessors>();
            List<ObjectAccessors> weights = new List<ObjectAccessors>();
            List<ObjectAccessors> matrix4 = new List<ObjectAccessors>();
            List<ObjectAccessors> normals = new List<ObjectAccessors>();
            List<ObjectAccessors> uvs_float = new List<ObjectAccessors>();
            List<ObjectAccessors> uvs_short = new List<ObjectAccessors>();
            List<ObjectAccessors> uvs_byte = new List<ObjectAccessors>();

            for (int i = 0; i < allUniqueAccessors.Count; i++)
            {
                ObjectAccessors oa = allUniqueAccessors[i];
                if (oa.type == Type.SCALAR)
                {
                    switch (oa.componentType)
                    {
                        case ComponentType.BYTE:
                        case ComponentType.UNSIGNED_BYTE:
                            scalars_byte.Add(oa);
                            break;
                        case ComponentType.SHORT:
                        case ComponentType.UNSIGNED_SHORT:
                            scalars_short.Add(oa);
                            break;
                        case ComponentType.UNSIGNED_INT:
                            scalars_int.Add(oa);
                            break;
                        case ComponentType.FLOAT:
                            scalars_float.Add(oa);
                            break;
                    }
                    continue;
                }
                if (oa.type == Type.VEC2)
                {
                    if (oa.componentType == ComponentType.UNSIGNED_BYTE || oa.componentType == ComponentType.BYTE)
                        uvs_byte.Add(oa);
                    else if (oa.componentType == ComponentType.UNSIGNED_SHORT || oa.componentType == ComponentType.SHORT)
                        uvs_short.Add(oa);
                    else
                        uvs_float.Add(oa);
                    continue;
                }
                if (oa.type == Type.VEC3)
                {
                    if (oa.min_max != null)
                    {
                        vertices.Add(oa);
                        continue;
                    }
                    else
                    {
                        normals.Add(oa);
                        continue;
                    }
                }
                if (oa.type == Type.VEC4)
                {
                    if (oa.optionaIdentifier == "joints")
                    {
                        Debug.Log("============ joints ===========");
                        joints.Add(oa);
                        continue;
                    }
                    if (oa.optionaIdentifier == "weights")
                    {
                        Debug.Log("============ weights ===========");
                        weights.Add(oa);
                        continue;
                    }
                }
                if (oa.type == Type.MAT4)
                {
                    matrix4.Add(oa);
                    continue;
                }
            }


            //SCALARS
            ObjectBufferView bv;
            if (scalars_byte.Count > 0)
            {
                Debug.Log("scalar byte");
                bv=new ObjectBufferView();
                
                for (int i =0; i < scalars_byte.Count;i++)
                {
                    scalars_byte[i].bufferView = bv;
                    scalars_byte[i].byteOffset = bv.GetCurrentAndAddDataToBufferView(scalars_byte[i].byteSize);
                    AddToSortedList(scalars_byte[i]);
                    WriteToBinaryUtilities.WriteUnsignedBytes(scalars_byte[i].scalarInt);
                    ObjectBuffer.AddByteSize(scalars_byte[i].byteSize);
                }
                if (!ObjectBufferView.HasFourthAmountOfBytes(bv.byteSize))
                {
                    int offset_buffer = 4 - (bv.byteSize % 4);
                    WriteToBinaryUtilities.WriteEmptySingle(offset_buffer);
                    ObjectBuffer.AddByteSize(offset_buffer);
                }
            }

            if (scalars_short.Count > 0)
            {
                bv = new ObjectBufferView();

                for (int i = 0; i < scalars_short.Count; i++)
                {
                    scalars_short[i].bufferView = bv;
                    scalars_short[i].byteOffset = bv.GetCurrentAndAddDataToBufferView(scalars_short[i].byteSize);
                    AddToSortedList(scalars_short[i]);
                    WriteToBinaryUtilities.WriteUnsignedShorts(scalars_short[i].scalarInt);
                    ObjectBuffer.AddByteSize(scalars_short[i].byteSize);
                }
                if (!ObjectBufferView.HasFourthAmountOfBytes(bv.byteSize))
                {
                    int offset_buffer = 4 - (bv.byteSize % 4);
                    WriteToBinaryUtilities.WriteEmptySingle(offset_buffer);
                    ObjectBuffer.AddByteSize(offset_buffer);
                }
            }
            //FULL INTS
            if (scalars_int.Count > 0)
            {
                bv = new ObjectBufferView();

                for (int i = 0; i < scalars_int.Count; i++)
                {
                    scalars_int[i].bufferView = bv;
                    scalars_int[i].byteOffset = bv.GetCurrentAndAddDataToBufferView(scalars_int[i].byteSize);
                    AddToSortedList(scalars_int[i]);
                    WriteToBinaryUtilities.WriteInts(scalars_int[i].scalarInt);
                    ObjectBuffer.AddByteSize(scalars_int[i].byteSize);
                }
            }
            //FLOATS
            if (scalars_float.Count > 0)
            {
                bv = new ObjectBufferView();

                for (int i = 0; i < scalars_float.Count; i++)
                {
                    scalars_float[i].bufferView = bv;
                    scalars_float[i].byteOffset = bv.GetCurrentAndAddDataToBufferView(scalars_float[i].byteSize);
                    AddToSortedList(scalars_float[i]);
                    WriteToBinaryUtilities.WriteFloats(scalars_float[i].scalarFloat);
                    ObjectBuffer.AddByteSize(scalars_float[i].byteSize);
                }
            }
            //VERTICES
            if (vertices.Count > 0)
            {
                bv = new ObjectBufferView();
                if (ExportToGLTF.options.quantizeGLTF)
                    bv.byteStride = vertices[0].GetByteSize(vertices[0].componentType, 3);

                foreach (ObjectAccessors oa in vertices)
                {
                    oa.bufferView = bv;
                    oa.byteOffset = bv.GetCurrentAndAddDataToBufferView(oa.byteSize);
                    AddToSortedList(oa);
                    //oa.componentType
                    WriteToBinaryUtilities.WriteVector3(oa.vector3Info, oa.componentType);
                    ObjectBuffer.AddByteSize(oa.byteSize);
                }
            }

            //NORMALS
            if (normals.Count > 0)
            {
                bv = new ObjectBufferView();
                if (ExportToGLTF.options.quantizeGLTF)
                    bv.byteStride = normals[0].GetByteSize(normals[0].componentType, 3);

                foreach (ObjectAccessors oa in normals)
                {
                    oa.bufferView = bv;
                    oa.byteOffset = bv.GetCurrentAndAddDataToBufferView(oa.byteSize);
                    AddToSortedList(oa);
                    WriteToBinaryUtilities.WriteVector3(oa.vector3Info, oa.componentType);
                    ObjectBuffer.AddByteSize(oa.byteSize);
                }
            }
            //JOINTS
            if (joints.Count > 0)
            {
                bv = new ObjectBufferView();
                Debug.Log("checkType joints");
                Debug.Log("check if byteStride is not necessary");

                foreach (ObjectAccessors oa in joints)
                {
                    oa.bufferView = bv;
                    oa.byteOffset = bv.GetCurrentAndAddDataToBufferView(oa.byteSize);
                    AddToSortedList(oa);
                    WriteToBinaryUtilities.WriteVector4(oa.vector4Info, oa.componentType);
                    ObjectBuffer.AddByteSize(oa.byteSize);
                }

            }
            // WEIGHTS
            if (weights.Count > 0)
            {
                bv = new ObjectBufferView();
                Debug.Log("checkType weights");
                Debug.Log("check if byteStride is not necessary");

                foreach (ObjectAccessors oa in weights)
                {
                    oa.bufferView = bv;
                    oa.byteOffset = bv.GetCurrentAndAddDataToBufferView(oa.byteSize);
                    AddToSortedList(oa);
                    WriteToBinaryUtilities.WriteVector4(oa.vector4Info, oa.componentType);
                    ObjectBuffer.AddByteSize(oa.byteSize);
                }

            }
            // MATRIX 4X4
            if (matrix4.Count > 0)
            {
                bv = new ObjectBufferView();
                Debug.Log("check matrix4");

                foreach (ObjectAccessors oa in matrix4)
                {
                    oa.bufferView = bv;
                    oa.byteOffset = bv.GetCurrentAndAddDataToBufferView(oa.byteSize);
                    AddToSortedList(oa);
                    WriteToBinaryUtilities.WriteMatrix4(oa.matrix4Info);
                    ObjectBuffer.AddByteSize(oa.byteSize);
                }
            }
            //UVS
            //BYTES
            if (uvs_byte.Count > 0)
            {
                bv = new ObjectBufferView();
                //if (ExportToGLTF.options.quantizeGLTF)
                    //bv.byteStride = uvs_byte[0].GetByteSize(uvs_byte[0].componentType, 2);

                foreach (ObjectAccessors oa in uvs_byte)
                {
                    oa.bufferView = bv;
                    oa.byteOffset = bv.GetCurrentAndAddDataToBufferView(oa.byteSize);
                    AddToSortedList(oa);
                    WriteToBinaryUtilities.WriteVector2(oa.vector2Info, oa.componentType);
                    ObjectBuffer.AddByteSize(oa.byteSize);
                }
                if (!ObjectBufferView.HasFourthAmountOfBytes(bv.byteSize))
                {
                    int offset_buffer = 4 - (bv.byteSize % 4);
                    WriteToBinaryUtilities.WriteEmptySingle(offset_buffer);
                    ObjectBuffer.AddByteSize(offset_buffer);
                }
            }
            //SHORTS
            if (uvs_short.Count > 0)
            {
                Debug.Log("test short");
                bv = new ObjectBufferView();
                //if (ExportToGLTF.options.quantizeGLTF)
                    //bv.byteStride = uvs_short[0].GetByteSize(uvs_short[0].componentType, 2);

                foreach (ObjectAccessors oa in uvs_short)
                {
                    oa.bufferView = bv;
                    oa.byteOffset = bv.GetCurrentAndAddDataToBufferView(oa.byteSize);
                    AddToSortedList(oa);
                    WriteToBinaryUtilities.WriteVector2(oa.vector2Info, oa.componentType);
                    ObjectBuffer.AddByteSize(oa.byteSize);
                }
            }
            //FLOATS
            if (uvs_float.Count > 0)
            {
                bv = new ObjectBufferView();
                //if (ExportToGLTF.options.quantizeGLTF)
                    //bv.byteStride = uvs_float[0].GetByteSize(uvs_float[0].componentType, 2);

                foreach (ObjectAccessors oa in uvs_float)
                {
                    oa.bufferView = bv;
                    oa.byteOffset = bv.GetCurrentAndAddDataToBufferView(oa.byteSize);
                    AddToSortedList(oa);
                    WriteToBinaryUtilities.WriteVector2(oa.vector2Info, oa.componentType);
                    ObjectBuffer.AddByteSize(oa.byteSize);
                }
            }
        }
        private static void AddToSortedList(ObjectAccessors oa)
        {
            oa.export_index = sortedAccessorsList.Count;
            sortedAccessorsList.Add(oa);
        }

        private string GetAccessorData()
        {
            bool min_max_as_int = false;
            if (componentType == ComponentType.UNSIGNED_SHORT || componentType == ComponentType.SHORT || componentType == ComponentType.UNSIGNED_INT)
                min_max_as_int = true;


            string result = "";
            result += "{\n" +
                "\"bufferView\" : " + bufferView.bufferIndex + ",\n" +
                "\"byteOffset\" : " + byteOffset + ",\n" +
                "\"componentType\" : " + (int)componentType + ",\n" +
                "\"count\" : " + accessorCount + ",\n";
            if (normalized)
            {
                result += "\"normalized\" : true,\n";
            }
            if (min_max != null) {
                result += "\"max\" : [" + min_max.GetValues(false, min_max_as_int) + "],\n";
                result += "\"min\" : [" + min_max.GetValues(true, min_max_as_int) + "],\n";
            }
                result += "\"type\" : \"" + type + "\"\n" +
                "}";
                // sparse not supported atm
            return result;
        }

    }
}
