using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.vivo.codelibrary
{
    public interface ISimplePoolData : IDisposable
    {
        bool IsUsed { get; }

        void PutIn();

        void PutOut();

        bool Disposed { get; }
    }

    public class SimpleData : ISimplePoolData
    {
        bool isUsed = false;

        public bool IsUsed
        {
            get
            {
                return isUsed;
            }
        }

        void ISimplePoolData.PutIn()
        {
            isUsed = false;
        }

        void ISimplePoolData.PutOut()
        {
            isUsed = true;
        }

        bool disposed = false;

        public bool Disposed
        {
            get
            {
                return disposed;
            }
        }

        void IDisposable.Dispose()
        {
            disposed = true;
        }
    }

}

