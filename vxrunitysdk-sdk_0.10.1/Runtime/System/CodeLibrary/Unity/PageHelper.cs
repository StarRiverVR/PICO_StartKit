using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    public class PageHelper
    {
        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <param name="_allNum">元素总数</param>
        /// <param name="_pageNum">每页元素数量</param>
        /// <returns></returns>
        public static int GetAllPage(int _allNum, int _pageNum)
        {
            if (_allNum <= 0 || _pageNum <= 0)
            {
                return 0;
            }
            int sub = _allNum / _pageNum;
            int yu = _allNum % _pageNum;
            if (yu == 0)
            {
                return sub;
            }
            else
            {
                return sub + 1;
            }
        }

        /// <summary>
        /// 格式化页序号
        /// </summary>
        /// <param name="_allNum">元素总数</param>
        /// <param name="_pageNum">每页元素数量</param>
        /// <param name="_pageIndex">传入页序号使其符合要求</param>
        public static void FormatPage(int _allNum, int _pageNum, ref int _pageIndex)
        {
            int allPage = GetAllPage(_allNum, _pageNum);
            if (allPage <= 0 || _pageIndex < 0 || _pageNum <= 0)
            {
                _pageIndex = 0;
                return;
            }
            if (_pageIndex >= allPage)
            {
                _pageIndex = allPage - 1;
                return;
            }
        }

        /// <summary>
        /// 根据页序号获得索引范围  int[0]<=x<int[1]
        /// </summary>
        /// <param name="_allNum">元素总数</param>
        /// <param name="_pageNum">每页元素数量</param>
        /// <param name="_pageIndex">传入页序号</param>
        /// <returns></returns>
        public static int[] GetPageIndexRange(int _allNum, int _pageNum, int _pageIndex)
        {
            if (_allNum <= 0 || _pageNum <= 0)
            {
                return null;
            }
            int pageIndex = _pageIndex;
            FormatPage(_allNum, _pageNum, ref pageIndex);
            int startIndex = pageIndex;
            int endIndex = startIndex + _pageNum;
            if (endIndex > _allNum)
            {
                endIndex = _allNum;
            }
            int[] res = new int[2];
            res[0] = startIndex;
            res[1] = endIndex;
            return res;
        }
    }
}


