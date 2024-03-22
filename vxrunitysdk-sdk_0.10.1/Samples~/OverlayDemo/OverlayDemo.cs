using com.vivo.openxr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayDemo : MonoBehaviour
{

    float rotaSpeed = 30;

    private enum ShowLayerState {     
        ShowQuadStatic,
        ShowQuadDynamic,
        ShowCylinderStatic,
        ShowCylinderDynamic,
        ShowDepthSort
    }
    
    public List<Texture> ChangeTexture;
    public List<GameObject> PreImags;
    private ShowLayerState _curShowState = ShowLayerState.ShowQuadStatic;

    public List<GameObject> QuadStaticObjs;
    public List<GameObject> QuadDynamicObjs;
    public List<GameObject> CylinderStaticObjs;
    public List<GameObject> CylinderDynamicObjs;
    public List<GameObject> DepthSortObjs;
    public VXROverlay QuadDymaicOverlay;
    public VXROverlay CylinderDymacOverlay;
    public List<GameObject> ButtonHightLightImgs;
    public Transform RotateCube;

    private Dictionary<ShowLayerState, List<GameObject>> _objMap;
    private int _selectIndex = 0;
    private int _showDynamicIndex = 0;  

    void Start()
    {
        _objMap = new Dictionary<ShowLayerState, List<GameObject>>();
        _objMap[ShowLayerState.ShowQuadStatic] = QuadStaticObjs;
        _objMap[ShowLayerState.ShowQuadDynamic] = QuadDynamicObjs;
        _objMap[ShowLayerState.ShowCylinderStatic] = CylinderStaticObjs;
        _objMap[ShowLayerState.ShowCylinderDynamic] = CylinderDynamicObjs;
        _objMap[ShowLayerState.ShowDepthSort] = DepthSortObjs;
        ShowLayerObjs();
        StartCoroutine(ShowDynamicTextuer());
    }

    IEnumerator ShowDynamicTextuer()
    {        
        while (true)
        {
            yield return new WaitForSeconds(2);
            if (_curShowState == ShowLayerState.ShowCylinderDynamic || _curShowState == ShowLayerState.ShowQuadDynamic)
            {      
                ChangePreImage();
                _showDynamicIndex = ++_showDynamicIndex > ChangeTexture.Count - 1 ? 0 : _showDynamicIndex;
            }
        }
    }

    void ShowLayerObjs()
    {
        ChangePreImage();
        for (int i = 0; i < ButtonHightLightImgs.Count; i++)
        {
            ButtonHightLightImgs[i].SetActive(i == _selectIndex);
        } 
        foreach (var item in _objMap)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                item.Value[i].SetActive(item.Key == _curShowState);
            }
        }
    }

    void ChangePreImage()
    {
        Texture leftNextTexture = ChangeTexture[_showDynamicIndex];
        Texture rightNextTexture = ChangeTexture[_showDynamicIndex];
        switch (_curShowState)
        {
            case ShowLayerState.ShowQuadDynamic:
                if (QuadDymaicOverlay != null)
                {
                    QuadDymaicOverlay.SetTextures(leftNextTexture, rightNextTexture);
                }
                break;
            case ShowLayerState.ShowCylinderDynamic:
                if (CylinderDymacOverlay != null)
                {
                    CylinderDymacOverlay.SetTextures(leftNextTexture, rightNextTexture);
                }
                break;
            default:
                break;
        }
        for (int i = 0; i < ChangeTexture.Count; i++)
        {
            PreImags[i].SetActive(i == _showDynamicIndex);
            PreImags[i + 3].SetActive(i == _showDynamicIndex);
        }
    }

    public void ChangeShowInfo()
    {        
        _selectIndex = ++_selectIndex > ButtonHightLightImgs.Count - 1 ? 0 : _selectIndex;        
        switch (_selectIndex)
        {   
            case 0:
                _curShowState = ShowLayerState.ShowQuadStatic;
                break; 
            case 1:
                _curShowState = ShowLayerState.ShowQuadDynamic;
                break; 
            case 2:
                _curShowState = ShowLayerState.ShowCylinderStatic;
                break; 
            case 3:
                _curShowState = ShowLayerState.ShowCylinderDynamic;
                break;
            case 4:
                _curShowState = ShowLayerState.ShowDepthSort;
                break;
            default:
                break;
        }        
        ShowLayerObjs();
    }  

    void Update()
    {
        RotateCube.Rotate(Vector3.up * rotaSpeed * Time.deltaTime);       
    }
}
