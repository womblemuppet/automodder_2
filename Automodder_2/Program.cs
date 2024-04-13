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

namespace Automodder_2
{
  internal static class Program
  {
    [STAThread]

    static void Main()
    {
      CleanUpOutputFolder();

      CreatePak();


      return;

      const string gameDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\GUILTY GEAR STRIVE\\RED\\Content\\Paks";
      const string aesKey = "0x3D96F3E41ED4B90B6C96CA3B2393F8911A5F6A48FE71F54B495E8F1AFD94CD73";

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
      System.IO.Directory.CreateDirectory("./output");
      System.IO.DirectoryInfo outputFolder = new DirectoryInfo("./output");

      foreach (FileInfo file in outputFolder.GetFiles()) { file.Delete(); }

      System.IO.Directory.CreateDirectory("./output/to_pak/RED/Content/Chara");
    }

    static void CreatePak()
    {
      string unrealPakAllFilesLine = "\"" + Directory.GetCurrentDirectory() + "/output/to_pak/**.*\" \"../../../*.*\"";
      File.WriteAllText("./output/files_to_pack.txt", unrealPakAllFilesLine);

      List<string> unrealPakArgs = new List<string> {
        "",
        ""
      };

      RunExecutable("external_apps/packer/UnrealPak.exe", unrealPakArgs);
    }

    static void RunExecutable(string exePath, IEnumerable<string> args)
    {
      try
      {
        Process process = new Process();
        process.StartInfo.FileName = exePath;
        process.StartInfo.ArgumentList.Add = args;
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
