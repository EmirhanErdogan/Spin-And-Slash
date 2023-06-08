using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ElephantSdkManager.Model;
using UnityEngine;

namespace ElephantSdkManager.Util
{
    public static class VersionUtils
    {
        public static int CompareVersions(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;

            var versionA = VersionStringToInts(a);
            var versionB = VersionStringToInts(b);
            for (var i = 0; i < Mathf.Max(versionA.Length, versionB.Length); i++)
            {
                if (VersionPiece(versionA, i) < VersionPiece(versionB, i))
                    return -1;
                if (VersionPiece(versionA, i) > VersionPiece(versionB, i))
                    return 1;
            }

            return 0;
        }

        public static bool IsEqualVersion(string a, string b)
        {
            return a.Equals(b);
        }


        private static int VersionPiece(IList<int> versionInts, int pieceIndex)
        {
            return pieceIndex < versionInts.Count ? versionInts[pieceIndex] : 0;
        }


        private static int[] VersionStringToInts(string version)
        {
            int piece;
            if (version.Contains("_internal"))
            {
                version = version.Replace("_internal", string.Empty);
            }
            return version.Split('.')
                .Select(v => int.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out piece) ? piece : 0)
                .ToArray();
        }

        private static string CheckMediationPackageName(string packageName)
        {
            if (packageName.ToLower().Contains("gamekit-is"))
            {
                return Application.dataPath + "/RollicGames/RollicIronSourceIDs.cs";
            }

            if (packageName.ToLower().Contains("gamekit-max"))
            {
                return Application.dataPath + "/RollicGames/RollicApplovinIDs.cs";
            }

            return null;
        }

        public static void SetupGameKitIDs(GameKitManifest gameKitManifest, string packageName)
        {
            if (gameKitManifest is null || gameKitManifest.data is null || gameKitManifest.data.appKey is null) return;
            
            string rollicAdsPath = CheckMediationPackageName(packageName);

            if (rollicAdsPath is null) return;

            string[] lines = File.ReadAllLines(rollicAdsPath);
            File.Delete(rollicAdsPath);

            using (StreamWriter sw = File.AppendText(rollicAdsPath))
            {
                foreach (string line in lines)
                {
                    string newLine = "";
                    if (line.Contains("[TEMP_GAMEKIT_AppKey]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_AppKey]", gameKitManifest.data.appKey);
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_BannerAdUnitIos]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_BannerAdUnitIos]", gameKitManifest.data.bannerAdUnitIos);
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_InterstitialAdUnitIos]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_InterstitialAdUnitIos]",
                            gameKitManifest.data.interstitialAdUnitIos);
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_RewardedAdUnitIos]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_RewardedAdUnitIos]",
                            gameKitManifest.data.rewardedAdUnitIos);
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_BannerAdUnitAndroid]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_BannerAdUnitAndroid]",
                            gameKitManifest.data.bannerAdUnitAndroid);
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_InterstitialAdUnitAndroid]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_InterstitialAdUnitAndroid]",
                            gameKitManifest.data.interstitialAdUnitAndroid);
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_RewardedAdUnitAndroid]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_RewardedAdUnitAndroid]",
                            gameKitManifest.data.rewardedAdUnitAndroid);
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_GoogleAppIdIos]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_GoogleAppIdIos]",
                            gameKitManifest.data.googleAppIdIos);
                    }
                    else if (line.Contains("[TEMP_GAMEKIT_GoogleAppIdAndroid]"))
                    {
                        newLine = line.Replace("[TEMP_GAMEKIT_GoogleAppIdAndroid]",
                            gameKitManifest.data.googleAppIdAndroid);
                    }
                    else
                    {
                        newLine = line;
                    }

                    sw.WriteLine(newLine);
                }
            }
        }

        #region IronSource Utils

        public static string GetVersionFromXML(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string version = "";
            try
            {
                xmlDoc.LoadXml(File.ReadAllText("Assets/IronSource/Editor/" + fileName + ".xml"));
            }
            catch (Exception)
            {
                return version;
            }

            var unityVersion = xmlDoc.SelectSingleNode("dependencies/unityversion");
            if (unityVersion != null)
            {
                return (unityVersion.InnerText);
            }

            return version;
        }

        #endregion

        #region Max Utils

        public static Versions GetCurrentVersions(string dependencyPath)
        {
            XDocument dependency;
            try
            {
                dependency = XDocument.Load(dependencyPath);
            }
#pragma warning disable 0168
            catch (IOException exception)
#pragma warning restore 0168
            {
                // Couldn't find the dependencies file. The plugin is not installed.
                return new Versions();
            }

            // <dependencies>
            //  <androidPackages>
            //      <androidPackage spec="com.applovin.mediation:network_name-adapter:1.2.3.4" />
            //  </androidPackages>
            //  <iosPods>
            //      <iosPod name="AppLovinMediationNetworkNameAdapter" version="2.3.4.5" />
            //  </iosPods>
            // </dependencies>
            string androidVersion = null;
            string iosVersion = null;
            var dependenciesElement = dependency.Element("dependencies");
            if (dependenciesElement != null)
            {
                var androidPackages = dependenciesElement.Element("androidPackages");
                if (androidPackages != null)
                {
                    var adapterPackage = androidPackages.Descendants().FirstOrDefault(element =>
                        element.Name.LocalName.Equals("androidPackage")
                        && element.FirstAttribute.Name.LocalName.Equals("spec")
                        && element.FirstAttribute.Value.StartsWith("com.applovin"));
                    if (adapterPackage != null)
                    {
                        androidVersion = adapterPackage.FirstAttribute.Value.Split(':').Last();
                        // Hack alert: Some Android versions might have square brackets to force a specific version. Remove them if they are detected.
                        if (androidVersion.StartsWith("["))
                        {
                            androidVersion = androidVersion.Trim('[', ']');
                        }
                    }
                }

                var iosPods = dependenciesElement.Element("iosPods");
                if (iosPods != null)
                {
                    var adapterPod = iosPods.Descendants().FirstOrDefault(element =>
                        element.Name.LocalName.Equals("iosPod")
                        && element.FirstAttribute.Name.LocalName.Equals("name")
                        && element.FirstAttribute.Value.StartsWith("AppLovin"));
                    if (adapterPod != null)
                    {
                        iosVersion = adapterPod.Attributes()
                            .First(attribute => attribute.Name.LocalName.Equals("version")).Value;
                    }
                }
            }

            var currentVersions = new Versions();
            if (androidVersion != null && iosVersion != null)
            {
                currentVersions.Unity = string.Format("android_{0}_ios_{1}", androidVersion, iosVersion);
                currentVersions.Android = androidVersion;
                currentVersions.Ios = iosVersion;
            }
            else if (androidVersion != null)
            {
                currentVersions.Unity = string.Format("android_{0}", androidVersion);
                currentVersions.Android = androidVersion;
            }
            else if (iosVersion != null)
            {
                currentVersions.Unity = string.Format("ios_{0}", iosVersion);
                currentVersions.Ios = iosVersion;
            }

            return currentVersions;
        }

        public class Versions
        {
            public string Unity;
            public string Android;
            public string Ios;

            public override bool Equals(object value)
            {
                var versions = value as Versions;

                return versions != null
                       && Unity.Equals(versions.Unity)
                       && (Android == null || Android.Equals(versions.Android))
                       && (Ios == null || Ios.Equals(versions.Ios));
            }

            public bool HasEqualSdkVersions(Versions versions)
            {
                return versions != null
                       && AdapterSdkVersion(Android).Equals(AdapterSdkVersion(versions.Android))
                       && AdapterSdkVersion(Ios).Equals(AdapterSdkVersion(versions.Ios));
            }

            public override int GetHashCode()
            {
                return new { Unity, Android, Ios }.GetHashCode();
            }

            private static string AdapterSdkVersion(string adapterVersion)
            {
                var index = adapterVersion.LastIndexOf(".");
                return index > 0 ? adapterVersion.Substring(0, index) : adapterVersion;
            }
        }

        #endregion
    }
}