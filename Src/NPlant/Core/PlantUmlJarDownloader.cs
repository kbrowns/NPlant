using System;
using System.IO;
using System.Net;

namespace NPlant.Core
{
    public static class PlantUmlJarDownloader
    {
        public static string Download(string dir)
        {
            string jarPath = Path.Combine(dir, "plantuml.jar");

            if (!File.Exists(jarPath))
            {
                try
                {
                    WebClient client = new WebClient();
                    client.DownloadFile("http://sourceforge.net/projects/plantuml/files/plantuml.jar/download", jarPath);
                }
                catch (WebException ex)
                {
                    throw new ApplicationException("Failed to download plantuml.jar.  If the internet is not available, please place a copy of this file in the execution directory.", ex);
                }
            }

            return jarPath;
        }
    }
}
