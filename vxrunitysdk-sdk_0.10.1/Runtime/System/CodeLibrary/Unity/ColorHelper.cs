using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace com.vivo.codelibrary
{
    public class ColorHelper
    {
        static Color GrayColor = new Color(189f / 255f, 189f / 255f, 189f / 255f, 1f);

        static Color WhiteColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 1f);

        static Color GreenColor = new Color(22 / 255f, 210 / 255f, 39 / 255f, 1f);

        static Color BlueColor = new Color(67 / 255f, 215 / 255f, 216 / 255f, 1f);

        static Color PurpleColor = new Color(170 / 255f, 67 / 255f, 216 / 255f, 1f);

        static Color OrangeColor = new Color(227 / 255f, 135 / 255f, 29 / 255f, 1f);

        static Color RedColor = new Color(210 / 255f, 56 / 255f, 33 / 255f, 1f);

        static Color BlackColor = new Color(41 / 255f, 41 / 255f, 41 / 255f, 1f);

        /// <summary>
        /// 获得品质色彩
        /// </summary>
        /// <param name="colorEnum"></param>
        /// <returns></returns>
        public static Color GetColorQuality(QualityEnum colorEnum)
        {
            switch (colorEnum)
            {
                case QualityEnum.Gray:
                    {
                        return GrayColor;
                    }
                case QualityEnum.White:
                    {
                        return WhiteColor;
                    }
                case QualityEnum.Green:
                    {
                        return GreenColor;
                    }
                case QualityEnum.Blue:
                    {
                        return BlueColor;
                    }
                case QualityEnum.Purple:
                    {
                        return PurpleColor;
                    }
                case QualityEnum.Orange:
                    {
                        return OrangeColor;
                    }
                case QualityEnum.Red:
                    {
                        return RedColor;
                    }
                case QualityEnum.Black:
                    {
                        return BlackColor;
                    }
            }
            return BlackColor;
        }

        /// <summary>
        /// 获得漂浮色彩
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="moreThanZero"></param>
        /// <param name="FloatTipEnum"></param>
        /// <param name="isCritical"></param>
        /// <returns></returns>
        public static Color GetFloatTextColor(string txt, bool moreThanZero, FloatTipEnum FloatTipEnum, bool isCritical = false)
        {
            Color res = new Color(213f / 255f, 233f / 255f, 69f / 255f, 1f);
            switch (FloatTipEnum)
            {
                case FloatTipEnum.Hp:
                    {
                        if (moreThanZero)
                        {
                            if (isCritical)
                            {
                                res = new Color(0f / 255f, 255f / 255f, 13f / 255f, 1f);
                            }
                            else
                            {
                                res = new Color(65f / 255f, 241f / 255f, 74f / 255f, 1f);
                            }
                        }
                        else
                        {
                            if (isCritical)
                            {
                                res = new Color(255f / 255f, 56f / 255f, 0f / 255f, 1f);
                            }
                            else
                            {
                                res = new Color(255f / 255f, 100f / 255f, 59f / 255f, 1f);
                            }
                        }
                    }
                    break;
                case FloatTipEnum.Magic:
                    {
                        if (moreThanZero)
                        {
                            if (isCritical)
                            {
                                res = new Color(0f / 255f, 140f / 255f, 13f / 255f, 1f);
                            }
                            else
                            {
                                res = new Color(75f / 255f, 170f / 255f, 250f / 255f, 1f);
                            }
                        }
                        else
                        {
                            if (isCritical)
                            {
                                res = new Color(156f / 255f, 26f / 255f, 255f / 255f, 1f);
                            }
                            else
                            {
                                res = new Color(170f / 255f, 61f / 255f, 253f / 255f, 1f);
                            }
                        }
                    }
                    break;
                case FloatTipEnum.Anger:
                    {
                        if (isCritical)
                        {
                            res = new Color(255f / 255f, 111f / 255f, 23f / 255f, 1f);
                        }
                        else
                        {
                            res = new Color(250f / 255f, 144f / 255f, 80f / 255f, 1f);
                        }
                    }
                    break;
                case FloatTipEnum.Power:
                    {
                        if (isCritical)
                        {
                            res = new Color(255f / 255f, 168f / 255f, 0f / 255f, 1f);
                        }
                        else
                        {
                            res = new Color(255f / 255f, 188f / 255f, 57f / 255f, 1f);
                        }
                    }
                    break;
                case FloatTipEnum.Money:
                    {
                        if (isCritical)
                        {
                            res = new Color(255f / 255f, 248f / 255f, 0f / 255f, 1f);
                        }
                        else
                        {
                            res = new Color(246f / 255f, 241f / 255f, 112f / 255f, 1f);
                        }
                    }
                    break;
                case FloatTipEnum.Info:
                    {

                    }
                    break;
            }

            return res;
        }

        static Dictionary<QualityEnum, string> ColorStrings = new Dictionary<QualityEnum, string>();

        /// <summary>
        /// 获得品质描述
        /// </summary>
        /// <param name="qualityEnum"></param>
        /// <returns></returns>
        public static string GetColorString(QualityEnum qualityEnum)
        {
            if (ColorStrings.ContainsKey(qualityEnum))
            {
                return ColorStrings[qualityEnum];
            }
            StringBuilder sb = new StringBuilder();
            Color c = GetColorQuality(qualityEnum);
            sb.Append("<color=#");
            sb.Append(c.GetString16FromColor());
            sb.Append(">");
            switch (qualityEnum)
            {
                case QualityEnum.Gray:
                    {
                        sb.Append("灰色");
                    }
                    break;
                case QualityEnum.White:
                    {
                        sb.Append("白色");
                    }
                    break;
                case QualityEnum.Green:
                    {
                        sb.Append("绿色");
                    }
                    break;
                case QualityEnum.Blue:
                    {
                        sb.Append("蓝色");
                    }
                    break;
                case QualityEnum.Purple:
                    {
                        sb.Append("紫色");
                    }
                    break;
                case QualityEnum.Orange:
                    {
                        sb.Append("橙色");
                    }
                    break;
                case QualityEnum.Red:
                    {
                        sb.Append("红色");
                    }
                    break;
                case QualityEnum.Black:
                    {
                        sb.Append("黑色");
                    }
                    break;
            }
            sb.Append("</color>");
            ColorStrings.Add(qualityEnum, sb.ToString());
            return ColorStrings[qualityEnum];
        }

        /// <summary>
        /// 获得品质描述
        /// </summary>
        /// <param name="qualityEnum"></param>
        /// <returns></returns>
        public static string GetColorString(QualityEnum qualityEnum, string res)
        {
            StringBuilder sb = new StringBuilder();
            Color c = GetColorQuality(qualityEnum);
            sb.Append("<color=#");
            sb.Append(c.GetString16FromColor());
            sb.Append(">");
            sb.Append(res);
            sb.Append("</color>");
            return sb.ToString();
        }

    }

    /// <summary>
    /// 品质分类
    /// </summary>
    public enum QualityEnum
    {
        Gray,
        White,
        Green,
        Blue,
        Purple,
        Orange,
        Red,
        Black,
    }

    /// <summary>
    /// 飘字类型
    /// </summary>
    public enum FloatTipEnum
    {
        /// <summary>
        /// 血量
        /// </summary>
        Hp,
        /// <summary>
        /// 魔法
        /// </summary>
        Magic,
        /// <summary>
        /// 怒气
        /// </summary>
        Anger,
        /// <summary>
        /// 能量
        /// </summary>
        Power,
        /// <summary>
        /// 金钱
        /// </summary>
        Money,
        /// <summary>
        /// 文字
        /// </summary>
        Info,
    }
}


