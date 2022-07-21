using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public enum Target { NONE_SET = 0, ARRAY_BUFFER = 34962, ELEMENT_ARRAY_BUFFER = 34963 }
    public enum ComponentType { NONE_SET = 0, BYTE = 5120, UNSIGNED_BYTE = 5121, SHORT = 5122, UNSIGNED_SHORT = 5123, UNSIGNED_INT = 5125, FLOAT = 5126 , BEST_INT = 1}
    public enum ComponentTypeSelected {BYTE = 0, SHORT = 1 , FLOAT = 2 }
    public enum Type { GET_FROM_BUFFER_VIEW, SCALAR, VEC2, VEC3, VEC4, MAT2, MAT3, MAT4 }
    public enum TextureExportType {NONE_SET = -1,DEFAULT = 0, WEBP = 1, KTX2 =2}
    public enum TextureType {DEFAULT = 0, NORMAL = 1, METALLIC_SMOOTHNESS = 2, LIGHTMAP = 3 , CUBEMAP = 4}
    public enum SmartType { basic = 0, character = 1, space = 2, ui = 3 }

    public enum TextureDivideFactor { NONE_SET = -1, FULL = 1, HALF = 2, QUARTER = 4, EIGHT = 8, SIXTEENTH = 16 }

    //public enum ExportCubemapType { CUBEMAP = 0, REFLECTION_PROBE =1}


}
