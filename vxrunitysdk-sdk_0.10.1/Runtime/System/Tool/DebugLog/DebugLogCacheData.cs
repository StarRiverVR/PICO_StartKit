using System;
namespace com.vivo.codelibrary
{
    class DebugLogCacheData
    {
        public string WritePath;

        public string LockKey;

        public int Y = -1;

        public int M = -1;

        public int D = -1;

        public int H = -1;

        public void Form(string root, LogTimeType logTimeType)
        {
            if (Y != DateTime.Now.Year || M != DateTime.Now.Month || D != DateTime.Now.Day ||
                H != DateTime.Now.Hour)
            {
                Y = DateTime.Now.Year;
                M = DateTime.Now.Month;
                D = DateTime.Now.Day;
                H = DateTime.Now.Hour;
                if (!string.IsNullOrEmpty(WritePath))
                {
                    FileWriteHelper.Instance.Close(WritePath);
                }
                switch (logTimeType)
                {
                    case LogTimeType.All:
                        {
                            WritePath = root +
                                        string.Format(string.Intern("{0:yyyy-M-d}/{0:yyyy-M-d-HH}-LogAll.txt"),
                                            DateTime.Now);
                        }
                        break;
                    case LogTimeType.Log:
                        {
                            WritePath = root +
                                        string.Format(string.Intern("{0:yyyy-M-d}/{0:yyyy-M-d-HH}-Log.txt"),
                                            DateTime.Now);
                        }
                        break;
                    case LogTimeType.WarningLog:
                        {
                            WritePath = root +
                                        string.Format(string.Intern("{0:yyyy-M-d}/{0:yyyy-M-d-HH}-Warning.txt"),
                                            DateTime.Now);
                        }
                        break;
                    case LogTimeType.ErrLog:
                        {
                            WritePath = root +
                                        string.Format(string.Intern("{0:yyyy-M-d}/{0:yyyy-M-d-HH}-Error.txt"),
                                            DateTime.Now);
                        }
                        break;
                }
                LockKey = WritePath.PathToLower();
            }
        }
    }
}