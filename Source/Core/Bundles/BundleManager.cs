﻿using Newtonsoft.Json.Linq;
using SIT.Core.Core;
using SIT.Core.Misc;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

/***
 * Full Credit for this patch goes to SPT-Aki team
 * Original Source is found here - https://dev.sp-tarkov.com/SPT-AKI/Modules
 * Paulov. Made changes to have better reflection and less hardcoding
 */
namespace SIT.Tarkov.Core
{
    public class BundleManager
    {
        public const string CachePath = "user/cache/bundles/";

        public const string CachedJsonPath = "user/cache/bundles.json";

        public const string CachedVersionTxtPath = "user/cache/sit.version.txt";

        public static Dictionary<string, BundleInfo> Bundles { get; private set; }

        static BundleManager()
        {
            Bundles = new Dictionary<string, BundleInfo>();

            // Ensure directories exist
            if (!Directory.Exists("user"))
                Directory.CreateDirectory("user");

            if (!Directory.Exists("user/cache"))
                Directory.CreateDirectory("user/cache");
        }

        public static void GetBundles()
        {
            var json = AkiBackendCommunication.Instance.GetJson("/singleplayer/bundles", timeout: 10000);
            PatchConstants.Logger.LogDebug($"[Bundle Manager] Bundles Json: {json}");

            bool bundlesAreSame = File.Exists(CachedJsonPath)
                && File.ReadAllText(CachedJsonPath) == json
                && VFS.Exists(CachePath + "bundles.json")
                && File.Exists(CachedVersionTxtPath)
                && File.ReadAllText(CachedVersionTxtPath) == Assembly.GetAssembly(typeof(VersionLabelPatch)).GetName().Version.ToString()
                ;
            if (bundlesAreSame)
            {
                PatchConstants.Logger.LogInfo($"[Bundle Manager] Bundles are same. Using cached Bundles");
                Bundles = Json.Deserialize<Dictionary<string, BundleInfo>>(File.ReadAllText(CachePath + "bundles.json"));

                return;
            }


            var jArray = JArray.Parse(json);

            foreach (var jObj in jArray)
            {
                var key = jObj["key"].ToString();
                if (Bundles.ContainsKey(key))
                    continue;

                var path = jObj["path"].ToString();
                var dependencyKeys = jObj["dependencyKeys"].ToObject<string[]>();
                var bundle = new BundleInfo(key, path, dependencyKeys);

                PatchConstants.Logger.LogInfo($"Adding Custom Bundle : {path}");

                if (path.Contains("http://") || path.Contains("https://"))
                {
                    var filepath = CachePath + Regex.Split(path, "bundle/", RegexOptions.IgnoreCase)[1];
                    try
                    {
                        var data = AkiBackendCommunication.Instance.GetData(path, true);
                        if (data != null && data.Length == 0)
                        {
                            PatchConstants.Logger.LogInfo("Bundle received is 0 bytes. WTF!");
                            continue;
                        }
                        VFS.WriteFile(filepath, data);
                        PatchConstants.Logger.LogInfo($"Writing Custom Bundle : {filepath}");
                        bundle.Path = filepath;
                    }
                    catch
                    {

                    }
                }
                else
                {

                }

                //PatchConstants.Logger.LogInfo($"Adding Custom Bundle : {key} : {path} : dp={dependencyKeys.Length}");
                Bundles.Add(key, bundle);
            }

            File.WriteAllText(CachedJsonPath, json);
            File.WriteAllText(CachedVersionTxtPath, Assembly.GetAssembly(typeof(VersionLabelPatch)).GetName().Version.ToString());
            VFS.WriteTextFile(CachePath + "bundles.json", Json.Serialize<Dictionary<string, BundleInfo>>(Bundles));
        }
    }
}
