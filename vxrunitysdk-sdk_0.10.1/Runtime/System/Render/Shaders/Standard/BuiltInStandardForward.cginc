// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef BUILT_IN_STANDARD_FORWARD_INCLUDED
#define BUILT_IN_STANDARD_FORWARD_INCLUDED

#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#   define UNITY_STANDARD_SIMPLE 1
#endif

#include "BuiltInStandardConfig.cginc"

#if UNITY_STANDARD_SIMPLE
    #include "BuiltInStandardCoreForwardSimpleCg.cginc"

    VertexOutputBaseSimple vertBase(VertexInput v) 
    { 
        return vertForwardBaseSimple(v); 
    }

    VertexOutputForwardAddSimple vertAdd(VertexInput v) 
    { 
        return vertForwardAddSimple(v); 
    }

    half4 fragBase (VertexOutputBaseSimple i) : SV_Target 
    { 
        return fragForwardBaseSimpleInternal(i); 
    }

    half4 fragAdd (VertexOutputForwardAddSimple i) : SV_Target 
    { 
        return fragForwardAddSimpleInternal(i); 
    }
#else
    #include "BuiltInStandardCore.cginc"

    VertexOutputForwardBase vertBase(VertexInput v) 
    { 
        return vertForwardBase(v); 
    }

    VertexOutputForwardAdd vertAdd(VertexInput v) 
    { 
        return vertForwardAdd(v); 
    }

    half4 fragBase (VertexOutputForwardBase i) : SV_Target 
    { 
        return fragForwardBaseInternal(i); 
    }

    half4 fragAdd (VertexOutputForwardAdd i) : SV_Target 
    { 
        return fragForwardAddInternal(i); 
    }
#endif

#endif // BUILT_IN_STANDARD_FORWARD_INCLUDED
