using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Diagnostics;

namespace Mismagius // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static string InteractiveHelpMessage = "Usage:\n" +
            string.Format("{0, -17} - {1}\n", " add <format>", "Adds the format in <format>.json to pvpoke") +
            string.Format("{0, -17} - {1}\n", " remove <format>", "Removes the format in <format>.json from pvpoke") +
            string.Format("{0, -17} - {1}\n", " reset", "Resets pvpoke gamemaster to a fresh install") +
            string.Format("{0, -17} - {1}\n", " restart", "Restarts the local server / applies changes") +
            string.Format("{0, -17} - {1}\n", " exit", "Exits the application") +
            string.Format("{0, -17} - {1}", " help", "Displays this message");

        static string CommandHelpMessage = "Usage:\n" +
            string.Format("{0, -27} - {1}\n", " mismagius add <format>", "Adds the format in <format>.json to pvpoke") +
            string.Format("{0, -27} - {1}\n", " mismagius remove <format>", "Removes the format in <format>.json from pvpoke") +
            string.Format("{0, -27} - {1}\n", " mismagius reset", "Resets pvpoke gamemaster to a fresh install") +
            string.Format("{0, -27} - {1}\n", " mismagius restart", "Restarts the local server / applies changes") +
            string.Format("{0, -27} - {1}\n", " mismagius help", "Displays this message") +
            string.Format("{0, -27} - {1}", " mismagius", "Launches interactive mode");


        static void Main(string[] args)
        {
            if (!Functionality.EnsurePureGamemaster()) { return; };

            if (args.Length == 0)
            {
                Console.WriteLine("Mismagius Interactive Mode:\n");
                Console.WriteLine(InteractiveHelpMessage);
                string? command = "";
                bool goodbye = false;

                while (true)
                {
                    Console.Write("> ");
                    command = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(command))
                    {
                        Console.WriteLine(InteractiveHelpMessage);
                        continue;
                    }
                    else
                    {
                        string[] inputs = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        switch (inputs[0])
                        {
                            case "add":
                                if (inputs.Length == 1)
                                {
                                    Console.WriteLine("Error: Must specify a format to add with \"add\"");
                                }
                                else
                                {
                                    Functionality.AddFormat(inputs[1], false);
                                }
                                
                                break;
                            case "remove":
                                if (inputs.Length == 1)
                                {
                                    Console.WriteLine("Error: Must specify a format to remove with \"remove\"");
                                }
                                else
                                {
                                    Functionality.RemoveFormat(inputs[1], false);
                                }
                                
                                break;
                            case "reset":
                                Functionality.Reset(false);
                                break;
                            case "restart":
                                Functionality.RestartServer(false);
                                break;
                            case "exit":
                                goodbye = true;
                                break;
                            default:
                                Console.WriteLine(InteractiveHelpMessage);
                                break;
                        }
                    }
                    if (goodbye) { break; }

                }
            }
            else
            {              
                switch (args[0])
                {
                    case "add":
                        if (args.Length == 1)
                        {
                            Console.WriteLine("Error: Must specify a format to add with \"add\"");
                        }
                        else
                        {
                            Functionality.AddFormat(args[1]);
                        }

                        break;
                    case "remove":
                        if (args.Length == 1)
                        {
                            Console.WriteLine("Error: Must specify a format to remove with \"remove\"");
                        }
                        else
                        {
                            Functionality.RemoveFormat(args[1]);
                        }

                        break;
                    case "reset":
                        Functionality.Reset();
                        break;
                    case "restart":
                        Functionality.RestartServer();
                        break;
                    default:
                        Console.WriteLine(CommandHelpMessage);
                        break;
                }
            }
        }
    }


    public static class Functionality
    {
        const string GameMasterPath = "./src/data/gamemaster.json";
        const string PureGameMasterPath = "./src/data/gamemaster_pure.json";
        const string OverridesPath = "./src/data/overrides";
        const string GroupsPath = "./src/data/groups";
        const string RankingsPath = "./src/data/rankings";

        public static void AddFormat(string format_name, bool silent = true)
        {
            string filepath = "./" + format_name + ".json";
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Error: File " + filepath + " does not exist.");
                return;
            }

            JObject format_json;

            string name = format_name;
            string title = format_name.ToUpper()[0] + format_name.Substring(1) + " Cup";

            try
            {
                format_json = JObject.Parse(File.ReadAllText(filepath));
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("Error: Invalid JSON in " + filepath + ".");
                return;
            }

            if (format_json.Type != JTokenType.Object)
            {
                Console.WriteLine("Error: Invalid format JSON in " + filepath + ".");
                return;
            }
            else
            {
                // GAMEMASTER

                //string? name = (string?)format_json["name"];
                //string? title = (string?)format_json["title"];
                int? league = (int?)format_json["league"];
                JArray? include = (JArray?)format_json["include"];
                JArray? exclude = (JArray?)format_json["exclude"];
                JArray? overrides = (JArray?)format_json["overrides"];

                if (name == null || title == null || league == null || include == null || exclude == null || overrides == null)
                {
                    Console.WriteLine("Error: Invalid format JSON in " + filepath + ".");
                    return;
                }

                JObject GamemasterJSON = JObject.Parse(File.ReadAllText(GameMasterPath));

                JArray? cups = (JArray?)GamemasterJSON["cups"];
                JArray? formats = (JArray?)GamemasterJSON["formats"];

                if (cups == null || formats == null)
                {
                    Console.WriteLine("Error: Invalid gamemaster file.");
                    return;
                }

                for (int i = 0; i < cups.Count; i++)
                {
                    string? cup_name = (string?)cups[i]["name"];

                    if (cup_name != null && cup_name == name)
                    {
                        cups[i].Remove();
                    }
                }

                for (int i = 0; i < formats.Count; i++)
                {
                    string? cup_name = (string?)formats[i]["cup"];

                    if (cup_name != null && cup_name == name)
                    {
                        formats[i].Remove();
                    }
                }

                JObject new_cup = new JObject();
                new_cup["name"] = name;
                new_cup["title"] = title;
                new_cup["include"] = include;
                new_cup["exclude"] = exclude;
                new_cup["allowSameSpecies"] = true;

                JObject new_format = new JObject();
                new_format["title"] = title;
                new_format["cup"] = name;
                new_format["meta"] = name;
                new_format["cp"] = league;
                new_format["showCup"] = true;
                new_format["showFormat"] = true;
                new_format["showMeta"] = true;

                cups.Add(new_cup);
                formats.Add(new_format);

                File.WriteAllText(GameMasterPath, GamemasterJSON.ToString());


                // OVERRIDES
                string override_dir = OverridesPath + "/" + name;

                Directory.CreateDirectory(override_dir);
                foreach (FileInfo file in new DirectoryInfo(OverridesPath + "/all").GetFiles())
                {
                    string targetFilePath = Path.Combine(override_dir, file.Name);
                    if (!File.Exists(targetFilePath)) {file.CopyTo(targetFilePath, true);}
                }

                string override_filename = override_dir + "/" + league.ToString() + ".json";
                JArray OverridesJSON = JArray.Parse(File.ReadAllText(override_filename));

                foreach (JObject over in overrides)
                {
                    bool found = false;
                    string? over_name = (string?)over["speciesId"];
                    if (over_name == null) { continue; }

                    foreach (JObject present_over in OverridesJSON)
                    {
                        string? present_name = (string?)present_over["speciesId"];
                        if (present_name == null || present_name != over_name) { continue; }
                        found = true;
                        present_over.Remove("fastMove");
                        present_over.Remove("chargedMoves");
                        present_over.Remove("weight");

                        if (over.ContainsKey("fastMove"))
                        {
                            present_over["fastMove"] = over["fastMove"];
                        }

                        if (over.ContainsKey("chargedMoves"))
                        {
                            present_over["chargedMoves"] = over["chargedMoves"];
                        }

                        if (over.ContainsKey("weight"))
                        {
                            present_over["weight"] = over["weight"];
                        }
                    }

                    if (!found)
                    {
                        OverridesJSON.Add(over);
                    }
                }

                File.WriteAllText(override_filename, OverridesJSON.ToString());


                // GROUPS
                string group_file = GroupsPath + "/" + name + ".json";
                File.WriteAllText(group_file, "[]");


                // RANKINGS
                string rankings_dir = RankingsPath + "/" + name;

                if (!Directory.Exists(rankings_dir)) { Directory.CreateDirectory(rankings_dir); }
                foreach (DirectoryInfo dir in new DirectoryInfo(RankingsPath + "/all").GetDirectories())
                {
                    string targetdirpath = Path.Combine(rankings_dir, dir.Name);
                    if (!Directory.Exists(targetdirpath)) {Directory.CreateDirectory(targetdirpath);}

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        string targetfilepath = Path.Combine(targetdirpath, file.Name);
                        if (!File.Exists(targetfilepath)) { file.CopyTo(targetfilepath, true); }
                    }
                }

                if (!silent) { Console.WriteLine("Added format " + title + " to the gamemaster."); }
            }
        }

        public static void RemoveFormat(string format_name, bool silent = true)
        {
            string filepath = "./" + format_name + ".json";
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Error: File " + filepath + " does not exist.");
                return;
            }

            JObject format_json;

            string name = format_name;

            try
            {
                format_json = JObject.Parse(File.ReadAllText(filepath));
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("Error: Invalid JSON in " + filepath + ".");
                return;
            }

            if (format_json.Type != JTokenType.Object)
            {
                Console.WriteLine("Error: Invalid format JSON in " + filepath + ".");
                return;
            }
            else
            {
                // GAMEMASTER
                JObject GamemasterJSON = JObject.Parse(File.ReadAllText(GameMasterPath));

                JArray? cups = (JArray?)GamemasterJSON["cups"];
                JArray? formats = (JArray?)GamemasterJSON["formats"];

                if (cups == null || formats == null)
                {
                    Console.WriteLine("Error: Invalid gamemaster file.");
                    return;
                }

                for (int i = 0; i < cups.Count; i++) 
                {
                    string? cup_name = (string?)cups[i]["name"];

                    if (cup_name != null && cup_name == name)
                    {
                        cups[i].Remove();
                    }
                }

                for (int i = 0; i < formats.Count; i++)
                {
                    string? cup_name = (string?)formats[i]["cup"];

                    if (cup_name != null && cup_name == name)
                    {
                        formats[i].Remove();
                    }
                }

                File.WriteAllText(GameMasterPath, GamemasterJSON.ToString());

                // OVERRIDES
                string override_dir = OverridesPath + "/" + name;
                if (Directory.Exists(override_dir)) { Directory.Delete(override_dir, true); };

                // GROUPS
                string group_file = GroupsPath + "/" + name + ".json";
                if (File.Exists(group_file)) { File.Delete(group_file); }

                // RANKINGS
                string rankings_dir = RankingsPath + "/" + name;
                if (Directory.Exists(rankings_dir)) { Directory.Delete(rankings_dir, true); }

                if (!silent) { Console.WriteLine("Removed format " + name + " from the gamemaster."); }
            }
        }

        public static void Reset(bool silent = true)
        {
            File.Copy(PureGameMasterPath, GameMasterPath, true);
            if (!silent) { Console.WriteLine("Reset gamemaster to default."); }
        }
        public static bool EnsurePureGamemaster()
        {
            if (!File.Exists(PureGameMasterPath))
            {
                if (!File.Exists(GameMasterPath))
                {
                    Console.WriteLine("Couldn't find gamemaster.json, make sure this program is placed in htdocs/pvpoke");
                    return false;
                }
                else
                {
                    File.Copy(GameMasterPath, PureGameMasterPath);
                    return true;
                }
            }
            else return true;
        }
        public static void RestartServer(bool silent = true)
        {
            Process apache_stop = new Process();
            Process apache_start = new Process();

            if (File.Exists("../../apache_stop.bat") || File.Exists("../../apache_start.bat"))
            {             
                apache_stop.StartInfo.FileName = "../../apache_stop.bat";
                
                apache_start.StartInfo.FileName = "../../apache_start.bat";

                if (!silent) { Console.WriteLine("Restarting the local server..."); }
                apache_stop.Start();

                Thread.Sleep(2000); // Giving the Apache Server time to shut down

                apache_start.Start();

                Thread.Sleep(2000);
                if (!silent) { Console.WriteLine("Done."); }
            }
            else
            {
                if (!silent) { Console.WriteLine("Restart is only available on Windows."); }
            }  
        }
    }
}