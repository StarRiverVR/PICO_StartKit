
namespace com.vivo.codelibrary
{
    public static class Extended_Bool
    {
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string GetString(this bool bl)
        {
            if (bl)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
    }
}

