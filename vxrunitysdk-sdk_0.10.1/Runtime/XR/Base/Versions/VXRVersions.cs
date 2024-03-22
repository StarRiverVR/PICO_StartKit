using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    /// <summary>
    /// 版本
    /// </summary>
    public struct VXRVersions
    {
        public VXRVersions(int major, int minor, int patch)
        {
            if (major<0 || minor<0 || patch<0)
            {
                System.Exception exception = new System.Exception("VivoOpenXRVersions value can't be less than zero !");
                VLog.Exception(exception);
                throw (exception);
            }

            Major = major;
            Minor = minor;
            Patch = patch;

            lock (lockObj)
            {
                if (hashCode >= int.MaxValue - 1)
                {
                    hashCode = 0;
                }
                else
                {
                    hashCode++;
                }
            }

            _hashCode = hashCode;
        }

        /// <summary>
        /// 大版本
        /// </summary>
        public int Major;

        /// <summary>
        /// 小版本
        /// </summary>
        public int Minor;

        /// <summary>
        /// 补丁版本
        /// </summary>
        public int Patch;

        public override string ToString()
        {
            return string.Format("Major={0},Minor={1},Patch={2}", Major, Minor, Patch);
        }

        public static bool operator ==(VXRVersions b, VXRVersions c)
        {
            return b.Major == c.Major && b.Minor == c.Minor && b.Patch == c.Patch;
        }

        public static bool operator !=(VXRVersions b, VXRVersions c)
        {
            return !(b == c);
        }

        public static bool operator >(VXRVersions b, VXRVersions c)
        {
            if (b.Major > c.Major)
            {
                return true;
            }else if (b.Major < c.Major)
            {
                return false;
            }
            else
            {
                if (b.Minor > c.Minor)
                {
                    return true;
                }else if (b.Minor < c.Minor)
                {
                    return false;
                }
                else
                {
                    if (b.Patch > c.Patch)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static bool operator <(VXRVersions b, VXRVersions c)
        {
            return !(b > c) && b != c;
        }

        public static bool operator >=(VXRVersions b, VXRVersions c)
        {
            return (b > c) || (b==c);
        }

        public static bool operator <=(VXRVersions b, VXRVersions c)
        {
            return (b < c) || (b == c);
        }

        public bool Equals(VXRVersions other)
        {
            return other == this;
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            if (o.GetType() != typeof(VXRVersions))
            {
                return false;
            }

            VXRVersions second = (VXRVersions)o;
            return second == this;
        }

        int _hashCode;

        static int hashCode;

        static object lockObj = new object();

        public override int GetHashCode()
        {
            return _hashCode;
        }

    }

}


