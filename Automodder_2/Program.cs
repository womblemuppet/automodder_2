using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using System;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using Newtonsoft.Json;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CUE4Parse.MappingsProvider;
using System.Reflection.Metadata;
using System.Windows.Forms.VisualStyles;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects.Properties;
using CUE4Parse.UE4.Assets.Exports.Animation;
using System.Linq.Expressions;

namespace Automodder_2
{
  internal static class Program
  {
    [STAThread]

    static void Main()
    {
      /*
      string uassetPath = "RED/Content/Chara/KYK/Costume01/Animation/Default/AnimArray.uasset";
      var bytes = provider.SaveAsset(uassetPath);
      System.IO.Directory.CreateDirectory("./output/to_pak/RED/Content/Chara/KYK/Costume01/Animation/Default/");
      System.IO.File.WriteAllBytes("./output/to_pak/RED/Content/Chara/KYK/Costume01/Animation/Default/AnimArray.uasset", bytes);
      */


      /*
      string uassetPath = "RED/Content/Chara/KYK/Costume01/Animation/Default/AnimArray.uasset";
      var allObjects = provider.LoadAllObjects(uassetPath);
      var fullJson = JsonConvert.SerializeObject(allObjects, Newtonsoft.Json.Formatting.Indented);

      foreach (var obj in allObjects)
      {
        Debug.WriteLine(obj.GetType());
        Debug.WriteLine("props follow:");


        foreach (var animDataArrayProperty in obj.Properties)
        {
          var animDataArray = animDataArrayProperty.Tag.GetValue<FStructFallback[]>();

          foreach (var animData in animDataArray)
          {
            Debug.WriteLine(animData);

            foreach (var animDataProperty in animData.Properties)
            {
              Debug.WriteLine(animDataProperty.Name);

              var prop = animDataProperty.Tag.GenericValue;
              Debug.WriteLine(prop);
            }

          }
        }

        Debug.WriteLine("-------------");
      }
      */



      CleanUpOutputFolder();

      const string gameDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\GUILTY GEAR STRIVE\\RED\\Content\\Paks";
      var provider = new DefaultFileProvider(gameDir, SearchOption.TopDirectoryOnly, true, new VersionContainer(EGame.GAME_UE4_27));
      provider.Initialize();

      string aesKey = System.IO.File.ReadAllText("./resources/aes_key.txt");
      provider.SubmitKey(new FGuid(), new FAesKey(aesKey));


      /*
      System.IO.Directory.CreateDirectory("./output/to_pak/RED/Content/Chara/KYK/Costume01/Animation/Default/");
      if (provider.TrySavePackage("RED/Content/Chara/KYK/Costume01/Animation/Default/AnimArray.uasset", out var assets))
      {
        foreach (var kvp in assets)
        {
          Debug.WriteLine("key: " + kvp.Key);
          Debug.WriteLine("value: " + kvp.Value);
          File.WriteAllBytes(Path.Combine("./output/to_pak/", kvp.Key), kvp.Value);
        }
      }
      */

      // TODO: put these 3x into separate functions that explain what they do and then call Dad.







      CreatePak();

      // ApplicationConfiguration.Initialize();
      // Application.Run(new Form1());
    }

    static void CleanUpOutputFolder()
    {
      System.IO.Directory.Delete("./output", true);
      System.IO.Directory.CreateDirectory("./output");
    }

    static void TestMakeKyFiles()
    {
      System.IO.Directory.CreateDirectory("./output/to_pak/RED/Content/Chara/KYK/Common/Data");

      string[] filenames = Directory.GetFiles("./test_files");
      foreach (string filename in filenames) {
        System.IO.File.Copy(
          filename,
          "./output/to_pak/RED/Content/Chara/KYK/Common/Data/" + Path.GetFileName(filename)
        );
      }

    }

    static void CreatePak()
    {
      string unrealPakAllFilesLine = "\"" + Directory.GetCurrentDirectory() + "/output/to_pak/**.*\" \"../../../*.*\"";
      File.WriteAllText("./output/files_to_pack.txt", unrealPakAllFilesLine);

      List<string> unrealPakArgs = new List<string> {
        Directory.GetCurrentDirectory() + "/output/mod.pak",  
        "-Create=" + Directory.GetCurrentDirectory() + "/output/files_to_pack.txt",
        "-compress"
      };

      RunExecutable("external_apps/packer/UnrealPak.exe", unrealPakArgs);
      System.IO.File.Copy("./resources/mod.sig", "./output/mod.sig");
    }

    static void RunExecutable(string exePath, IEnumerable<string> args)
    {
      try
      {
        Process process = new Process();
        process.StartInfo.FileName = exePath;

        foreach (string arg in args) { process.StartInfo.ArgumentList.Add(arg); }

        process.StartInfo.RedirectStandardOutput = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        Debug.WriteLine(output);

        process.WaitForExit();
      }

      catch (Exception ex) { Debug.WriteLine("Error: " + ex.Message); throw; }
    }



  }
}
