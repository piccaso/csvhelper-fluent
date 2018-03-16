using System.Globalization;
using System.Text;
using CsvCfg = CsvHelper.Configuration.Configuration;

namespace CsvHelper.Fluent
{
    public static class Configuration
    {
        public static CsvCfg Default => For(CultureInfo.CurrentCulture);
        public static CsvCfg For(CultureInfo cultureInfo)
        {
            var textInfo = cultureInfo.TextInfo;
            return new CsvCfg
            {
                Delimiter = textInfo.ListSeparator,
                Encoding = GetEncoding(textInfo.ANSICodePage),
                CultureInfo = cultureInfo,
            };
        }

        private static Encoding GetEncoding(int codepage)
        {
#if NETSTD20
            return CodePagesEncodingProvider.Instance.GetEncoding(codepage);
#endif
#if NET45
            return Encoding.GetEncoding(codepage);
#endif
        }
    }
}