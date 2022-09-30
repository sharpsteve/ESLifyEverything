﻿using ESLifyEverything.BSAHandlers;
using ESLifyEverything.FormData;
using ESLifyEverything.Properties.DataFileTypes;
using ESLifyEverything.XEdit;
using System.Diagnostics;
using System.Text.Json;

namespace ESLifyEverything
{
    public static partial class Program
    {

        #region xEdit Log
        public static void XEditSession()
        {
            XEditLogReader.ReadLog(Path.Combine(GF.Settings.XEditFolderPath, GF.Settings.XEditLogFileName));
            int xEditSessionsCount = XEditLogReader.xEditLog.xEditSessions?.Length ?? 0;
            if (xEditSessionsCount <= 0)
            {
                GF.WriteLine(GF.stringLoggingData.NoxEditSessions);
            }
            else if (GF.Settings.AutoReadAllxEditSeesion)
            {
                string fileName = Path.Combine(GF.Settings.XEditFolderPath, GF.Settings.XEditLogFileName);
                FileInfo fi = new FileInfo(fileName);
                if (fi.Length > 10000000)//Path.Combine(GF.Settings.XEditFilePath, GF.Settings.XEditLogFileName)
                {
                    GF.WriteLine(GF.stringLoggingData.XEditLogFileSizeWarning);
                }
                XEditSessionAutoAll();
                if (GF.Settings.DeletexEditLogAfterRun_Requires_AutoReadAllxEditSeesion)
                {
                    File.Delete(Path.Combine(GF.Settings.XEditFolderPath, GF.Settings.XEditLogFileName));
                }
            }
            else if (GF.Settings.AutoReadNewestxEditSeesion)
            {
                XEditLogReader.xEditLog.xEditSessions![xEditSessionsCount - 1].GenerateCompactedModDatas();
            }
            else
            {
                XEditSessionMenu();
            }
        }

        public static void XEditSessionMenu()
        {
            int xEditSessionsCount = XEditLogReader.xEditLog.xEditSessions?.Length ?? 0;
            Console.WriteLine("\n\n\n\n");
            GF.WriteLine(GF.stringLoggingData.SelectSession,true,false);
            for (int i = 0; i < xEditSessionsCount; i++)
            {
                GF.WriteLine($"{i}. " + XEditLogReader.xEditLog.xEditSessions![i].SessionTimeStamp);
            }
            GF.WriteLine(GF.stringLoggingData.InputSessionPromt, true, false);
            GF.WriteLine(GF.stringLoggingData.ExitCodeInput, true, false);
            int selectedMenuItem;
            while (GF.WhileMenuSelect(xEditSessionsCount - 1, out selectedMenuItem) == false) ;
            if (selectedMenuItem != -1) XEditLogReader.xEditLog.xEditSessions![selectedMenuItem].GenerateCompactedModDatas();
        }

        public static void XEditSessionAutoAll()
        {
            foreach (XEditSession session in XEditLogReader.xEditLog.xEditSessions!)
            {
                session.GenerateCompactedModDatas();
            }
        }

        #endregion xEdit Log

        public static void ImportModData(string compactedFormsLocation)
        {
            IEnumerable<string> compactedFormsModFiles = Directory.EnumerateFiles(
                compactedFormsLocation,
                "*_ESlEverything.json",
                SearchOption.AllDirectories);
            foreach (string compactedFormsModFile in compactedFormsModFiles)
            {
                CompactedModData mod = new CompactedModData();
                //mod.GetModData(Path.GetFileName(compactedFormsModFile).Replace("_ESlEverything.json", "").Replace("_ESlEverything", ""));
                mod.GetModData(compactedFormsModFile);
                mod.Write();
                CompactedModDataD.TryAdd(mod.ModName, mod);
            }
        }

        public static async Task<int> LoadOrderBSAData()
        {
            string loadorderFilePath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData")!, "Skyrim Special Edition", "loadorder.txt");
            if (!File.Exists(loadorderFilePath))
            {
                loadorderFilePath = "loadorder.txt";
            }

            if (File.Exists(loadorderFilePath))
            {
                string[] loadorder = File.ReadAllLines(loadorderFilePath);

                foreach (string plugin in loadorder)
                {
                    string pluginNoExtension = Path.ChangeExtension(plugin, null);
                    if (File.Exists(Path.Combine(GF.Settings.DataFolderPath, pluginNoExtension + ".bsa")))
                    {
                        LoadOrderNoExtensions.Add(pluginNoExtension);
                    }
                }

                int loadorderCount = LoadOrderNoExtensions.Count;
                for (int i = 0; i < loadorderCount; i++)
                {
                    if (File.Exists(Path.Combine(GF.Settings.DataFolderPath, LoadOrderNoExtensions[i] + ".bsa")))
                    {
                        GF.WriteLine(String.Format(GF.stringLoggingData.BSACheckMod, LoadOrderNoExtensions[i]));
                        BSAData.AddNew(LoadOrderNoExtensions[i]);
                    }
                    Console.WriteLine();
                    GF.WriteLine(String.Format(GF.stringLoggingData.ProcessedBSAsLogCount, i + 1, loadorderCount));
                    Console.WriteLine();
                }
                BSAData.Output();
            }
            else
            {
                GF.WriteLine(GF.stringLoggingData.LoadOrderNotDetectedError);
                GF.WriteLine(GF.stringLoggingData.RunOrReport);
                BSAExtracted = false;
            }
            return await Task.FromResult(0);
        }

        #region Voice Eslify

        public static void VoiceESlIfyMenu()
        {
            GF.WriteLine(GF.stringLoggingData.VoiceESLMenuHeader);
            GF.WriteLine($"1. {GF.stringLoggingData.ESLEveryMod}{GF.stringLoggingData.WithCompactedForms}", true, false);//with a _ESlEverything.json attached to it inside of the CompactedForms folders.
            GF.WriteLine($"2. {GF.stringLoggingData.SingleInputMod}{GF.stringLoggingData.WithCompactedForms}", true, false);
            GF.WriteLine(GF.stringLoggingData.CanLoop, true, false);
            GF.WriteLine(GF.stringLoggingData.ExitCodeInput, true, false);
            bool whileContinue = true;
            do
            {
                GF.WhileMenuSelect(2, out int selectedMenuItem, 1);
                switch (selectedMenuItem)
                {
                    case -1:
                        GF.WriteLine(GF.stringLoggingData.ExitCodeInputOutput);
                        whileContinue = false;
                        break;
                    case 1:
                        GF.WriteLine(GF.stringLoggingData.EslifingEverything);
                        whileContinue = VoiceESLifyEverything();
                        break;
                    case 2:
                        GF.WriteLine(GF.stringLoggingData.EslifingSingleMod);
                        VoiceESLifySingleMod();
                        break;
                }
            } while (whileContinue == true);
        }

        public static bool VoiceESLifyEverything()
        {
            foreach (CompactedModData modData in CompactedModDataD.Values)
            {
                VoiceESLifyMod(modData);
            }
            return false;
        }

        public static void VoiceESLifySingleMod()
        {
            bool whileContinue = true;
            string input;
            CompactedModData? modData;
            do
            {
                GF.WriteLine($"{GF.stringLoggingData.SingleModInputHeader}{GF.stringLoggingData.ExamplePlugin}");
                GF.WriteLine(GF.stringLoggingData.ExitCodeInput, true, false);
                input = Console.ReadLine() ?? "";
                if (CompactedModDataD.TryGetValue(input, out modData))
                {
                    modData.Write();
                    VoiceESLifyMod(modData);
                }
                else if (input.Equals("XXX", StringComparison.OrdinalIgnoreCase))
                {
                    whileContinue = false;
                }
            } while (whileContinue == true);
        }

        public static void VoiceESLifyMod(CompactedModData modData)
        {
            Task v = ExtractBSAVoiceData(modData.ModName);
            v.Wait();
            v.Dispose();
            VoiceESLifyModData(modData, GF.ExtractedBSAModDataPath);
            VoiceESLifyModData(modData, GF.Settings.DataFolderPath);
        }

        public static async Task<int> ExtractBSAVoiceData(string pluginName)
        {
            foreach (string plugin in LoadOrderNoExtensions)
            {
                BSA? bsa;
                if (BSAData.BSAs.TryGetValue(plugin, out bsa))
                {
                    foreach (string connectedVoice in bsa.VoiceModConnections)
                    {
                        if (connectedVoice.Equals(pluginName, StringComparison.OrdinalIgnoreCase))
                        {
                            GF.WriteLine(String.Format(GF.stringLoggingData.BSAContainsData, bsa.BSAName_NoExtention, pluginName));
                            Process m = new Process();
                            m.StartInfo.FileName = ".\\BSABrowser\\bsab.exe";
                            m.StartInfo.Arguments = $"\"{Path.GetFullPath(Path.Combine(GF.Settings.DataFolderPath, bsa.BSAName_NoExtention + ".bsa"))}\" -f \"{pluginName}\"  -e -o \"{Path.GetFullPath(GF.ExtractedBSAModDataPath)}\"";
                            m.Start();
                            m.WaitForExit();
                            m.Dispose();
                        }
                    }
                }
            }
            return await Task.FromResult(0);
        }

        public static void VoiceESLifyModData(CompactedModData modData, string dataStartPath)
        {
            if (Directory.Exists(Path.Combine(dataStartPath, "sound\\voice", modData.ModName)))
            {
                foreach (FormHandler form in modData.CompactedModFormList)
                {
                    IEnumerable<string> voiceFilePaths = Directory.EnumerateFiles(
                        Path.Combine(dataStartPath, "sound\\voice", modData.ModName),
                        "*" + form.OrigonalFormID + "*",
                        SearchOption.AllDirectories);
                    foreach (string voiceFilePath in voiceFilePaths)
                    {
                        CopyFormFile(form, dataStartPath, voiceFilePath, out string filePath);
                    }
                }
            }
        }
        #endregion Voice Eslify

        #region FaceGen Eslify
        public static void FaceGenESlIfyMenu()
        {
            GF.WriteLine(GF.stringLoggingData.FaceGenESLMenuHeader);
            GF.WriteLine($"1. {GF.stringLoggingData.ESLEveryMod}{GF.stringLoggingData.WithCompactedForms}",true, false);//with a _ESlEverything.json attached to it inside of the CompactedForms folders.
            GF.WriteLine($"2. {GF.stringLoggingData.SingleInputMod}{GF.stringLoggingData.WithCompactedForms}", true, false);
            GF.WriteLine(GF.stringLoggingData.CanLoop, true, false);
            GF.WriteLine(GF.stringLoggingData.ExitCodeInput, true, false);
            bool whileContinue = true;
            do
            {
                GF.WhileMenuSelect(2, out int selectedMenuItem, 1);
                switch (selectedMenuItem)
                {
                    case -1:
                        GF.WriteLine(GF.stringLoggingData.ExitCodeInputOutput);
                        whileContinue = false;
                        break;
                    case 1:
                        GF.WriteLine(GF.stringLoggingData.EslifingEverything);
                        whileContinue = FaceGenESLifyEverything();
                        break;
                    case 2:
                        GF.WriteLine(GF.stringLoggingData.EslifingSingleMod);
                        FaceGenESLifySingleMod();
                        break;
                }
            } while (whileContinue == true);
        }

        public static bool FaceGenESLifyEverything()
        {
            foreach (CompactedModData modData in CompactedModDataD.Values)
            {
                FaceGenESLifyMod(modData);
            }
            return false;
        }

        public static void FaceGenESLifySingleMod()
        {
            bool whileContinue = true;
            string input;
            CompactedModData modData;
            do
            {
                GF.WriteLine(GF.stringLoggingData.SingleModInputHeader + GF.stringLoggingData.ExamplePlugin, GF.Settings.VerboseConsoleLoging, GF.Settings.VerboseFileLoging);
                GF.WriteLine(GF.stringLoggingData.ExitCodeInput, true, false);
                input = Console.ReadLine() ?? "";
                if (CompactedModDataD.TryGetValue(input, out modData!))
                {
                    modData.Write();
                    FaceGenESLifyMod(modData);
                }
                else if (input.Equals("XXX", StringComparison.OrdinalIgnoreCase))
                {
                    whileContinue = false;
                }
            } while (whileContinue == true);
        }
        
        public static void FaceGenESLifyMod(CompactedModData modData)
        {
            Task f = ExtractBSAFaceGenData(modData.ModName);
            f.Wait();
            f.Dispose();
            FaceGenEslifyModData(modData, GF.ExtractedBSAModDataPath);
            FaceGenEslifyModData(modData, GF.Settings.DataFolderPath);
            
        }

        public static async Task<int> ExtractBSAFaceGenData(string pluginName)
        {
            foreach (string plugin in LoadOrderNoExtensions)
            {
                BSA? bsa;
                if (BSAData.BSAs.TryGetValue(plugin, out bsa))
                {
                    foreach (string connectedFaceGen in bsa.FaceGenModConnections)
                    {

                        if (connectedFaceGen.Equals(pluginName, StringComparison.OrdinalIgnoreCase))
                        {
                            GF.WriteLine(String.Format(GF.stringLoggingData.BSAContainsData, bsa.BSAName_NoExtention, pluginName));
                            Process m = new Process();
                            m.StartInfo.FileName = ".\\BSABrowser\\bsab.exe";
                            m.StartInfo.Arguments = $"\"{Path.GetFullPath(Path.Combine(GF.Settings.DataFolderPath, bsa.BSAName_NoExtention + ".bsa"))}\" -f \"{pluginName}\"  -e -o \"{Path.GetFullPath(GF.ExtractedBSAModDataPath)}\"";
                            m.Start();
                            m.WaitForExit();
                            m.Dispose();
                            if (bsa.HasTextureBSA)
                            {
                                Process t = new Process();
                                t.StartInfo.FileName = ".\\BSABrowser\\bsab.exe";
                                t.StartInfo.Arguments = $"\"{Path.GetFullPath(Path.Combine(GF.Settings.DataFolderPath, bsa.BSAName_NoExtention + " - Textures.bsa"))}\" -f \"{pluginName}\"  -e -o \"{Path.GetFullPath(GF.ExtractedBSAModDataPath)}\"";
                                t.Start();
                                t.WaitForExit();
                                t.Dispose();
                            }
                        }
                    }
                }
            }
            return await Task.FromResult(0);
        }

        public static void FaceGenEslifyModData(CompactedModData modData, string dataStartPath)
        {
            if (Directory.Exists(Path.Combine(dataStartPath, "Meshes\\Actors\\Character\\FaceGenData\\FaceGeom\\", modData.ModName)))
            {
                foreach (FormHandler form in modData.CompactedModFormList)
                {
                    IEnumerable<string> FaceGenTexFilePaths = Directory.EnumerateFiles(
                        Path.Combine(dataStartPath, "Textures\\Actors\\Character\\FaceGenData\\FaceTint\\", modData.ModName),
                        "*" + form.OrigonalFormID + "*",
                        SearchOption.AllDirectories);
                    foreach (string FaceGenFilePath in FaceGenTexFilePaths)
                    {
                        CopyFormFile(form, dataStartPath, FaceGenFilePath, out string filePath);
                    }

                    IEnumerable<string> FaceGenFilePaths = Directory.EnumerateFiles(
                        Path.Combine(dataStartPath, "Meshes\\Actors\\Character\\FaceGenData\\FaceGeom\\", modData.ModName),
                        "*" + form.OrigonalFormID + "*",
                        SearchOption.AllDirectories);

                    foreach (string FaceGenFilePath in FaceGenFilePaths)
                    {
                        CopyFormFile(form, dataStartPath, FaceGenFilePath, out string filePath);
                        EditedFaceGen = true;
                        using (StreamWriter stream = File.AppendText(GF.FaceGenFileFixPath))
                        {
                            stream.WriteLine(Path.GetFullPath(filePath) + ";" + form.OrigonalFormID + ";" + form.CompactedFormID);
                        }
                    }

                }
            }
        }
        #endregion FaceGen Eslify

        #region ESLify Data Files
        public static void ESLifyDataFilesMainMenu()
        {
            GetESLifyModConfigurationFiles();

            Console.WriteLine(GF.stringLoggingData.InputDataFileExecutionPromt);
            Console.WriteLine($"1. {GF.stringLoggingData.ESLEveryModConfig}", true, false);
            Console.WriteLine($"2. {GF.stringLoggingData.SelectESLModConfig}", true, false);
            GF.WriteLine(GF.stringLoggingData.ExitCodeInput, true, false);
            int selectedMenuItem;
            while (GF.WhileMenuSelect(2, out selectedMenuItem, 1) == false) ;
            switch (selectedMenuItem)
            {
                case -1:
                    GF.WriteLine(GF.stringLoggingData.ExitCodeInputOutput);
                    break;
                case 1:
                    ESLifyAllDataFiles();
                    break;
                case 2:
                    ESLifySelectedDataFilesMenu();
                    break;
                default:
                    break;
            }
        }

        public static void ESLifyAllDataFiles()
        {
            GetESLifyModConfigurationFiles();
            foreach (var modConfiguration in BasicSingleModConfigurations)
            {
                HandleConfigurationType(modConfiguration);
            }
            foreach (var modConfiguration in BasicDirectFolderModConfigurations)
            {
                HandleConfigurationType(modConfiguration);
            }
            foreach (var modConfiguration in BasicDataSubfolderModConfigurations)
            {
                HandleConfigurationType(modConfiguration);
            }
            foreach (var modConfiguration in ComplexTOMLModConfigurations)
            {
                HandleConfigurationType(modConfiguration);
            }
        }

        public static void GetESLifyModConfigurationFiles()
        {
            //                 BasicSingleFile
            IEnumerable<string> basicSingleFilesModConfigurations = Directory.EnumerateFiles(
                    ".\\Properties\\DataFileTypes",
                    "*_BasicSingleFile.json",
                    SearchOption.TopDirectoryOnly);
            foreach (string file in basicSingleFilesModConfigurations)
            {
                try
                {
                    HashSet<BasicSingleFile> basicSingleFile = JsonSerializer.Deserialize<HashSet<BasicSingleFile>>(File.ReadAllText(file))!;
                    if (basicSingleFile != null)
                    {
                        foreach (BasicSingleFile modConfiguration in basicSingleFile)
                        {
                            if (modConfiguration.Enabled)
                            {
                                BasicSingleModConfigurations.Add(modConfiguration);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    GF.WriteLine(GF.stringLoggingData.ConfiguartionFileFailed + Path.GetFileName(file));
                }
            }
            //                 BasicDirectFolder
            IEnumerable<string> basicDirectFolderModConfigurations = Directory.EnumerateFiles(
                    ".\\Properties\\DataFileTypes",
                    "*_BasicDirectFolder.json",
                    SearchOption.TopDirectoryOnly);

            foreach (var file in basicDirectFolderModConfigurations)
            {
                try
                {
                    HashSet<BasicDirectFolder> basicDirectFolderFile = JsonSerializer.Deserialize<HashSet<BasicDirectFolder>>(File.ReadAllText(file))!;
                    if (basicDirectFolderFile != null)
                    {
                        foreach (BasicDirectFolder modConfiguration in basicDirectFolderFile)
                        {
                            if (modConfiguration.Enabled)
                            {
                                BasicDirectFolderModConfigurations.Add(modConfiguration);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    GF.WriteLine(GF.stringLoggingData.ConfiguartionFileFailed + Path.GetFileName(file));
                }
            }
            //                 BasicDataSubfolder
            IEnumerable<string> basicDataSubfolderModConfigurations = Directory.EnumerateFiles(
                    ".\\Properties\\DataFileTypes",
                    "*_BasicDataSubfolder.json",
                    SearchOption.TopDirectoryOnly);

            foreach (var file in basicDataSubfolderModConfigurations)
            {
                try
                {
                    HashSet<BasicDataSubfolder> basicDirectFolderFile = JsonSerializer.Deserialize<HashSet<BasicDataSubfolder>>(File.ReadAllText(file))!;
                    if (basicDirectFolderFile != null)
                    {
                        foreach (BasicDataSubfolder modConfiguration in basicDirectFolderFile)
                        {
                            if (modConfiguration.Enabled)
                            {
                                BasicDataSubfolderModConfigurations.Add(modConfiguration);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    GF.WriteLine(GF.stringLoggingData.ConfiguartionFileFailed + Path.GetFileName(file));
                }
            }
            //                 ComplexTOML
            IEnumerable<string> complexTOMLModConfigurations = Directory.EnumerateFiles(
                    ".\\Properties\\DataFileTypes",
                    "*_ComplexTOML.json",
                    SearchOption.TopDirectoryOnly);
            foreach (var file in complexTOMLModConfigurations)
            {
                try
                {
                    HashSet<ComplexTOML> basicDirectFolderFile = JsonSerializer.Deserialize<HashSet<ComplexTOML>>(File.ReadAllText(file))!;
                    if (basicDirectFolderFile != null)
                    {
                        foreach (ComplexTOML modConfiguration in basicDirectFolderFile)
                        {
                            if (modConfiguration.Enabled)
                            {
                                ComplexTOMLModConfigurations.Add(modConfiguration);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    GF.WriteLine(GF.stringLoggingData.ConfiguartionFileFailed + Path.GetFileName(file));
                }
            }
        }

        public static void HandleConfigurationType(BasicSingleFile ModConfiguration)
        {
            Console.WriteLine("\n\n\n\n");
            GF.WriteLine(ModConfiguration.StartingLogLine);
            SingleBasicFile(Path.Combine(GF.Settings.DataFolderPath, ModConfiguration.DataPath), ModConfiguration.FileAtLogLine, ModConfiguration.FileUnchangedLogLine);
        }

        public static void HandleConfigurationType(BasicDirectFolder ModConfiguration)
        {
            Console.WriteLine("\n\n\n\n");
            GF.WriteLine(ModConfiguration.StartingLogLine);
            EnumDirectFolder(
                Path.Combine(GF.Settings.DataFolderPath, ModConfiguration.StartFolder), ModConfiguration.FileNameFilter,
                ModConfiguration.FileAtLogLine, ModConfiguration.FileUnchangedLogLine,
                ModConfiguration.SeachLevel);
        }

        public static void HandleConfigurationType(BasicDataSubfolder ModConfiguration)
        {
            Console.WriteLine("\n\n\n\n");
            GF.WriteLine(ModConfiguration.StartingLogLine);
            Task t = EnumDataSubfolder(
                        Path.Combine(GF.Settings.DataFolderPath, ModConfiguration.StartDataSubFolder), ModConfiguration.DirectoryFilter, ModConfiguration.FileFilter,
                        ModConfiguration.FileAtLogLine, ModConfiguration.FileUnchangedLogLine,
                        ModConfiguration.StartSeachLevel, ModConfiguration.SubFolderSeachLevel);
            t.Wait();
            t.Dispose();
        }

        public static void HandleConfigurationType(ComplexTOML ModConfiguration)
        {
            Console.WriteLine("\n\n\n\n");
            GF.WriteLine(ModConfiguration.StartingLogLine);
            Task t = EnumToml(
                        Path.Combine(GF.Settings.DataFolderPath, ModConfiguration.StartFolder), ModConfiguration.FileNameFilter, ModConfiguration.ArrayStartFilters,
                        ModConfiguration.FileAtLogLine, ModConfiguration.FileUnchangedLogLine,
                        ModConfiguration.SeachLevel);
            t.Wait();
            t.Dispose();
        }

        public static void ESLifySelectedDataFilesMenu()
        {
            bool endMenu = false;
            string[] modConMenuList = GetModConList();
            string[] GetModConList()
            {
                List<string> modConMenuList = new List<string>();
                foreach (var modConfiguration in BasicSingleModConfigurations)
                {
                    Console.WriteLine(modConfiguration.Name);
                    modConMenuList.Add(modConfiguration.Name);
                }
                foreach (var modConfiguration in BasicDirectFolderModConfigurations)
                {
                    modConMenuList.Add(modConfiguration.Name);
                }
                foreach (var modConfiguration in BasicDataSubfolderModConfigurations)
                {
                    modConMenuList.Add(modConfiguration.Name);
                }
                foreach (var modConfiguration in ComplexTOMLModConfigurations)
                {
                    modConMenuList.Add(modConfiguration.Name);
                }
                return modConMenuList.ToArray();
            }
            if (modConMenuList.Length <= 0)
            {
                GF.WriteLine(GF.stringLoggingData.NoModConfigurationFilesFound);
                return;
            }

            do
            {
                Console.WriteLine("\n\n");
                Console.WriteLine(GF.stringLoggingData.ModConfigInputPrompt);
                Console.WriteLine($"0. {GF.stringLoggingData.SwitchToEverythingMenuItem}");
                for (int i = 0; i < modConMenuList.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {modConMenuList[i]}");
                }
                GF.WriteLine(GF.stringLoggingData.ExitCodeInput, true, false);
                if (GF.WhileMenuSelect(modConMenuList.Length + 1, out int selectedMenuItem, 0))
                {
                    if (selectedMenuItem == 0)
                    {
                        ESLifyAllDataFiles();
                        endMenu = true;
                    }
                    else if (selectedMenuItem == -1)
                    {
                        GF.WriteLine(GF.stringLoggingData.ExitCodeInputOutput);
                        endMenu = true;
                    }
                    else
                    {
                        RunSelectedModConfig(modConMenuList, selectedMenuItem);
                    }
                }


            } while (endMenu == false);
        }

        public static void RunSelectedModConfig(string[] modConMenuList, int selectedMenuItem)
        {

            foreach (var modConfiguration in BasicSingleModConfigurations)
            {
                if (modConfiguration.Name.Equals(modConMenuList[selectedMenuItem - 1]))
                {
                    HandleConfigurationType(modConfiguration);
                }
            }
            foreach (var modConfiguration in BasicDirectFolderModConfigurations)
            {
                if (modConfiguration.Name.Equals(modConMenuList[selectedMenuItem - 1]))
                {
                    HandleConfigurationType(modConfiguration);
                }
            }
            foreach (var modConfiguration in BasicDataSubfolderModConfigurations)
            {
                if (modConfiguration.Name.Equals(modConMenuList[selectedMenuItem - 1]))
                {
                    HandleConfigurationType(modConfiguration);
                }
            }
            foreach (var modConfiguration in ComplexTOMLModConfigurations)
            {
                if (modConfiguration.Name.Equals(modConMenuList[selectedMenuItem - 1]))
                {
                    HandleConfigurationType(modConfiguration);
                }
            }

        }
        #endregion ESLify Data Files

        public static async Task<int> CustomSkillsFramework()
        {
            string startSearchPath = Path.Combine(GF.Settings.DataFolderPath, "NetScriptFramework\\Plugins");
            if (Directory.Exists(startSearchPath))
            {
                IEnumerable<string> customSkillConfigs = Directory.EnumerateFiles(
                    startSearchPath,
                    "CustomSkill*config.txt",
                    SearchOption.TopDirectoryOnly);
                foreach (string customSkillConfig in customSkillConfigs)
                {
                    GF.WriteLine(GF.stringLoggingData.CustomSkillsFileAt + customSkillConfig, GF.Settings.VerboseConsoleLoging, GF.Settings.VerboseFileLoging);
                    string[] customSkillConfigFile = File.ReadAllLines(customSkillConfig);
                    string[] newCustomSkillConfigFile = new string[customSkillConfigFile.Length];
                    string currentModName = "";
                    CompactedModData currentMod = new CompactedModData();
                    bool changed = false;
                    for (int i = 0; i < customSkillConfigFile.Length; i++)
                    {
                        string line = customSkillConfigFile[i];
                        foreach (string modName in CompactedModDataD.Keys)
                        {
                            if (customSkillConfigFile[i].Contains(modName, StringComparison.OrdinalIgnoreCase))
                            {
                                currentModName = modName;
                                CompactedModDataD.TryGetValue(modName, out currentMod!);
                                GF.WriteLine("", GF.Settings.VerboseConsoleLoging, false);
                                GF.WriteLine(GF.stringLoggingData.ModLine + line, GF.Settings.VerboseConsoleLoging, GF.Settings.VerboseFileLoging);
                            }
                        }
                        if (!currentModName.Equals(""))
                        {
                            foreach (FormHandler form in currentMod.CompactedModFormList)
                            {
                                if (line.Contains(form.GetOrigonalFormID()))
                                {
                                    GF.WriteLine(GF.stringLoggingData.OldLine + line, GF.Settings.VerboseConsoleLoging, GF.Settings.VerboseFileLoging);
                                    line = line.Replace(form.GetOrigonalFormID(), form.GetCompactedFormID());
                                    GF.WriteLine(GF.stringLoggingData.NewLine + line, GF.Settings.VerboseConsoleLoging, GF.Settings.VerboseFileLoging);
                                    currentModName = "";
                                    changed = true;
                                }
                            }
                        }
                        newCustomSkillConfigFile[i] = line;
                    }
                    OuputDataFileToOutputFolder(changed, customSkillConfig, newCustomSkillConfigFile, GF.stringLoggingData.CustomSkillsFileUnchanged);
                }

            }
            else
            {
                GF.WriteLine(GF.stringLoggingData.FolderNotFoundError + startSearchPath);
            }
            return await Task.FromResult(0);
        }
    }
}
