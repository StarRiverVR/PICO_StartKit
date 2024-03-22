using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMonoPoolFlag : MonoBehaviour
{
    public System.Action PoolClear;

    private void OnDestroy()
    {
        try
        {
            if (PoolClear != null)
            {
                PoolClear.Invoke();
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
    }
}
