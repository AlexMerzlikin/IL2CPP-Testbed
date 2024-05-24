/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
#endif
using UnityEditor.Callbacks;

namespace Facebook.Unity.PostProcess
{
    /// <summary>
    /// Automatically disables Bitcode on iOS builds
    /// </summary>
    public static class DisableBitcode
    {
        [PostProcessBuildAttribute(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject)
        {
#if UNITY_IOS
            if (buildTarget != BuildTarget.iOS) return;
            string projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            //Disabling Bitcode on all targets
            //Main
            string target = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            //Unity Tests
            target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            //Unity Framework
            target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            pbxProject.WriteToFile(projectPath);
#endif
        }
    }
}

namespace Facebook.Unity.PostProcess
{
    /// <summary>
    /// Automatically disables Bitcode on iOS builds
    /// </summary>
    public static class FixDeeplinks
    {
        [PostProcessBuild(10000)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            FixUniversalLinksColdStartBugInFacebookSDK(pathToBuiltProject);
        }

        private static void FixUniversalLinksColdStartBugInFacebookSDK(string path)
        {
            string isBackgroundLaunchOptions3 = @"(?x)(isBackgroundLaunchOptions.+(?:.*\n)+?\s*return\ )YES(\;\n\})# }";;
            string isBackgroundLaunchOptions = @"(?x)(\(BOOL\)isBackgroundLaunchOptions.+(?:.*\n)+?\s*return\ )YES(\;\n\})";
            string isBackgroundLaunchOptions2 = @"(?x)(isBackgroundLaunchOptions:\(NSDictionary\*\)launchOptions(?:.*\n)+?\s*return\ )YES(\;\n\})# }";
            string fullPath = Path.Combine(path, Path.Combine("Classes", "UnityAppController.mm"));
            string data = Load(fullPath);
            var data1 = Regex.Replace(
                data,
                isBackgroundLaunchOptions,
                "$1NO$2");
            var data2 = Regex.Replace(
                data,
                isBackgroundLaunchOptions2,
                "$1NO$2");
            var data3 = Regex.Replace(
                data,
                isBackgroundLaunchOptions3,
                "$1NO$2");
            Save(fullPath, data1);

            static string Load(string fullPath)
            {
                string data;
                FileInfo projectFileInfo = new FileInfo(fullPath);
                StreamReader fs = projectFileInfo.OpenText();
                data = fs.ReadToEnd();
                fs.Close();

                return data;
            }

            static void Save(string fullPath, string data)
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(fullPath, false);
                writer.Write(data);
                writer.Close();
            }
        }
    }
}