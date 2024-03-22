using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace com.vivo.codelibrary
{
    public class SerializableKeyframe : IDataEquata<SerializableKeyframe>
    {
        public bool Equals(SerializableKeyframe other)
        {
            return other == this;
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            if (o.GetType() != typeof(SerializableKeyframe))
            {
                return false;
            }

            SerializableKeyframe second = (SerializableKeyframe)o;
            return second == this;
        }

        int _hashCode;

        static int hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public SerializableKeyframe(Keyframe keyframe)
        {
            if (hashCode >= int.MaxValue - 1)
            {
                hashCode = 0;
            }
            else
            {
                hashCode++;
            }
            _hashCode = hashCode;

            //
            Time = keyframe.time;
            Value = keyframe.value;
            InTangent = keyframe.inTangent;
            OutTangent = keyframe.outTangent;
            TangentMode = keyframe.tangentMode;
            WeightedMode = keyframe.weightedMode;
            InWeight = keyframe.inWeight;
            OutWeight = keyframe.outWeight;
        }

        public SerializableKeyframe(float time, float value)
        {
            if (hashCode >= int.MaxValue - 1)
            {
                hashCode = 0;
            }
            else
            {
                hashCode++;
            }
            _hashCode = hashCode;

            //
            Time = time;
            Value = value;
            InTangent = 0f;
            OutTangent = 0f;
            WeightedMode = 0;
            InWeight = 0f;
            OutWeight = 0f;
            TangentMode = 0;
        }

        public float Time;

        public float Value;

        public float InTangent;

        public float OutTangent;

        public int TangentMode;

        public WeightedMode WeightedMode;

        public float InWeight;

        public float OutWeight;

        public static implicit operator SerializableKeyframe(Keyframe keyframe)
        {
            return new SerializableKeyframe(keyframe);
        }

        public static implicit operator Keyframe(SerializableKeyframe serializableKeyframe)
        {
            Keyframe newData = new Keyframe();
            newData.time = serializableKeyframe.Time;
            newData.value = serializableKeyframe.Value;
            newData.inTangent = serializableKeyframe.InTangent;
            newData.outTangent = serializableKeyframe.OutTangent;
            newData.tangentMode = serializableKeyframe.TangentMode;
            newData.weightedMode = serializableKeyframe.WeightedMode;
            newData.inWeight = serializableKeyframe.InWeight;
            newData.outWeight = serializableKeyframe.OutWeight;
            return newData;
        }
    }
}


