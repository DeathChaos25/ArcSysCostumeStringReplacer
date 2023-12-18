using Amicitia.IO.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI;

if (args.Length == 0)
{
    System.Console.WriteLine("ArcSysCostumeStringReplacer Usage:\nDrag Folder to included batch file");
    Console.ReadKey();
}
else if (args[0] == "-h" || args[0] == "-H" || args[0] == "-help" || args[0] == "-Help")
{
    System.Console.WriteLine("\nUsage:\nDrag Folder to included batch file");
}
else
{
    FileInfo arg0 = new FileInfo(args[0]);
    var filePaths = Directory.EnumerateFiles(arg0.FullName, "*.uasset", SearchOption.AllDirectories).ToList();

    int targetCostumeID = 2;
    bool replaceSkeletonStrings = false;
    int CostumeIDToReplace = 1;
    bool isAltReplaceMode = false;
    bool isReplaceColorMode = false;

    if (args.Length > 1)
    {
        targetCostumeID = int.Parse(args[1]);
    }
    if (args.Length > 2)
    {
        replaceSkeletonStrings = bool.Parse(args[2]);
        Console.WriteLine($"Replacing Skeleton Strings set to {replaceSkeletonStrings}");
    }

    if (args.Length > 3)
    {
        CostumeIDToReplace = int.Parse(args[3]);
        isAltReplaceMode = true;
        Console.WriteLine($"Changing all Costume{CostumeIDToReplace:D2} strings to Costume{targetCostumeID:D2}\n");
    }
    else Console.WriteLine($"Changing all Costume01 strings to Costume{targetCostumeID:D2}\n");

    if (args.Length > 4)
    {
        isReplaceColorMode = bool.Parse(args[4]);
        Console.WriteLine($"Replacing Color Strings set to {isReplaceColorMode}");
    }

    String CostumeIDToReplaceString = $"/Costume{CostumeIDToReplace:D2}";
    String ColorIDToReplaceString = $"/Color{CostumeIDToReplace:D2}";

    List<String> FilesToResave = new List<String>();

    foreach (var filePath in filePaths)
    {
        List<long> StringOffsets = new List<long>();
        List<long> StringOffsetsPtc = new List<long>();
        List<String> CostumeStrings = new List<String>();
        List<String> ParticlePathStrings = new List<String>();
        bool wasReplaced = false;

        if (Path.GetFileNameWithoutExtension(filePath).ToLower() == "EffectMaterialInfo".ToLower()) continue; //skip file

        using (BinaryObjectReader uasset_file = new BinaryObjectReader(filePath, Endianness.Little, Encoding.GetEncoding(0)))
        {
            uasset_file.Seek(0x29, SeekOrigin.Begin);
            var numOfStrings = uasset_file.ReadInt16();
            Console.WriteLine($"Checking {numOfStrings} strings in {filePath}");
            uasset_file.Seek(0xC1, SeekOrigin.Begin);
            for (int i = 0; i < numOfStrings; i++)
            {
                int StringSize = uasset_file.ReadInt32();
                var StringOffset = uasset_file.Position;
                string TargetString = uasset_file.ReadString(StringBinaryFormat.NullTerminated);
                var hash = uasset_file.ReadInt32();
                if (isReplaceColorMode)
                {
                    if (TargetString.Contains(ColorIDToReplaceString))
                    {
                        Console.WriteLine($"{TargetString} - {StringSize}");
                        StringOffsets.Add(StringOffset);
                        CostumeStrings.Add(TargetString);
                        wasReplaced = true;
                    }
                }
                else if (isAltReplaceMode)
                {
                    if (TargetString.Contains(CostumeIDToReplaceString))
                    {
                        if (TargetString.Contains("Skeleton") || TargetString.Contains("skeleton"))
                        {
                            if (!replaceSkeletonStrings) continue; // dont replace skeleton string
                        }

                        Console.WriteLine($"{TargetString} - {StringSize}");
                        StringOffsets.Add(StringOffset);
                        CostumeStrings.Add(TargetString);
                        wasReplaced = true;
                    }
                }
                else
                {
                    if (TargetString.Contains("/Costume01") ||
                        (targetCostumeID == 1 && (TargetString.Contains("/Costume02") || TargetString.Contains("/Costume03") ||
                        TargetString.Contains("/Costume04") || TargetString.Contains("/Costume05"))))
                    {
                        if (TargetString.Contains("Skeleton") || TargetString.Contains("skeleton"))
                        {
                            if (!replaceSkeletonStrings) continue; // dont replace skeleton string
                        }

                        Console.WriteLine($"{TargetString} - {StringSize}");
                        StringOffsets.Add(StringOffset);
                        CostumeStrings.Add(TargetString);
                        wasReplaced = true;
                    }
                }
            }
        }

        for (int i = 0; i < CostumeStrings.Count; i++)
        {
            String newString = $"/Costume{targetCostumeID:D2}";
            byte[] NewString = Encoding.ASCII.GetBytes(newString);

            String newColorString = $"/Color{targetCostumeID:D2}";
            byte[] NewColorString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace("/Costume01", newColorString));

            if (isReplaceColorMode)
            {
                if (CostumeStrings[i].Contains(ColorIDToReplaceString))
                {
                    NewString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace(ColorIDToReplaceString, newColorString));
                }
            }
            else if (isAltReplaceMode)
            {
                if (CostumeStrings[i].Contains(CostumeIDToReplaceString))
                {
                    NewString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace(CostumeIDToReplaceString, newString));
                }
            }
            else if (targetCostumeID == 1)
            {
                if (CostumeStrings[i].Contains("/Costume02")) NewString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace("/Costume02", newString));
                else if (CostumeStrings[i].Contains("/Costume03")) NewString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace("/Costume03", newString));
                else if (CostumeStrings[i].Contains("/Costume04")) NewString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace("/Costume04", newString));
                else if (CostumeStrings[i].Contains("/Costume05")) NewString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace("/Costume05", newString));
                else NewString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace("/Costume01", newString));
            }
            else NewString = Encoding.ASCII.GetBytes(CostumeStrings[i].Replace("/Costume01", newString));

            ReplaceData(filePath, (int)StringOffsets[i], NewString);
        }

        if (wasReplaced)
        {
            if (isAltReplaceMode) Console.WriteLine($"\nReplaced Costume{CostumeIDToReplace:D2} strings in {filePath}");
            else Console.WriteLine($"\nReplaced Costume{targetCostumeID:D2} strings in {filePath}");

            wasReplaced = false;
            FilesToResave.Add(filePath);
        }
    }

    foreach (var file in FilesToResave)
    {
        Save(Open(file), file);
    }
}

static void ReplaceData(string filename, int position, byte[] data)
{
    using (Stream stream = File.Open(filename, FileMode.Open))
    {
        stream.Position = position;
        stream.Write(data, 0, data.Length);
    }
}

static UAsset Open(string infile)
{
    UAsset asset = new UAsset();
    asset.SetEngineVersion(UAssetAPI.UnrealTypes.EngineVersion.VER_UE4_27);
    asset.FilePath = infile;
    asset.Read(asset.PathToReader(asset.FilePath));
    return asset;
}

static void Save(UAsset asset, string outpath)
{
    bool loop = true;
    while (loop)
    {
        loop = false;
        try
        {
            asset.Write(outpath);
            return;
        }
        catch (NameMapOutOfRangeException ex)
        {
            try
            {
                asset.AddNameReference(ex.RequiredName);
                loop = true;
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}