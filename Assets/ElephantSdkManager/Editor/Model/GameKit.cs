using System;

namespace ElephantSdkManager.Model
{
    [Serializable]
    public class GameKitManifest
    {
        public GameKitSdk data;
    }
    
    [Serializable]
    public class GameKitSdk
    {
        public string version;
        public string appKey;
        public string bannerAdUnitIos;
        public string interstitialAdUnitIos;
        public string rewardedAdUnitIos;
        public string bannerAdUnitAndroid;
        public string interstitialAdUnitAndroid;
        public string rewardedAdUnitAndroid;
        public string googleAppIdIos;
        public string googleAppIdAndroid;
    }
}