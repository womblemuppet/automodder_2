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

namespace Automodder_2
{
  internal static class Program
  {
    [STAThread]

    static void Main()
    {
      CleanUpOutputFolder();

      TestMakeKyFiles();

      CreatePak();

      const string gameDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\GUILTY GEAR STRIVE\\RED\\Content\\Paks";

      string aesKey = System.IO.File.ReadAllText("./resources/aes_key.txt");
      Debug.WriteLine(aesKey);

      return;
      var provider = new DefaultFileProvider(gameDir, SearchOption.TopDirectoryOnly, true, new VersionContainer(EGame.GAME_UE4_27));
      provider.Initialize();
      provider.SubmitKey(new FGuid(), new FAesKey(aesKey));

      var allObjects = provider.LoadAllObjects("RED/Content/Chara/ABA/Costume01/Animation/Default/body/AB_body.uasset");
      var fullJson = JsonConvert.SerializeObject(allObjects, Newtonsoft.Json.Formatting.Indented);
      Debug.WriteLine("fullJson:");
      Debug.WriteLine(fullJson);


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
      //System.IO.Directory.CreateDirectory("./output/to_pak/RED/Content/Chara/");
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
