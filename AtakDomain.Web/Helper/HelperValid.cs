using System.Net;

namespace AtakDomain.Web.Helper
{
    public static class HelperValid
    {
        public static bool checkUrl(string p_strValue)
        {
            if (Uri.IsWellFormedUriString(p_strValue, UriKind.RelativeOrAbsolute))
            {
                Uri l_strUri = new Uri(p_strValue);
                return (l_strUri.Scheme == Uri.UriSchemeHttp || l_strUri.Scheme == Uri.UriSchemeHttps);
            }
            else
            {
                return false;
            }
        }

        public static bool checkWebsite(string Url)
        {
            try
            {
                WebClient wc = new WebClient();
                string HTMLSource = wc.DownloadString(Url);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void starCheck(int stars)
        {
        }
    }
}