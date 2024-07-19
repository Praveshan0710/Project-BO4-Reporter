using System.IO.Compression;
using static System.Net.Mime.MediaTypeNames;

namespace Project_BO4_Report
{
    internal class Program
    {
        static void Main()
        {
            if (!File.Exists("BlackOps4.exe"))
            {
                Console.WriteLine("Please run this application in your Black Ops 4 directory");
                Console.ReadKey(true);
                return;
            }

            const string tempReportDir = "ReporterTemp";
            string shieldInfoFileName = $@"ShieldInfo{ DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.txt";
            string shieldInfoFilePath = Path.Combine(tempReportDir, shieldInfoFileName);

            Directory.CreateDirectory(tempReportDir);

            using (var sw = new StreamWriter(shieldInfoFilePath, true))
            {
                //Get project-bo4.json data
                sw.WriteLine("-----------project-bo4.json-----------\n");
                try
                {
                    sw.WriteLine(File.ReadAllText("project-bo4.json"));
                }
                catch (FileNotFoundException)
                {
                    sw.WriteLine("Couldn't find project-bo4.json.");
                }

                //Get project-bo4.log data
                sw.WriteLine("\n-----------project-bo4.log-----------\n");
                try
                {
                    sw.WriteLine(File.ReadAllText("project-bo4.log"));
                }
                catch (FileNotFoundException)
                {
                    sw.WriteLine("Couldn't find project-bo4.log.");
                }

                //Get launcher version if used
                sw.WriteLine("\n-----------project-bo4 launcher settings-----------\n");
                try
                {
                    sw.WriteLine(File.ReadAllText(@"project-bo4\files\settings.ini"));
                }
                catch (FileNotFoundException)
                {
                    sw.WriteLine("Couldn't find launcher settings.");
                }
                catch (Exception ex)
                {
                    sw.WriteLine(ex.Message);
                }

                //Get minidumps if any
                 sw.WriteLine("\n-----------minidumps-----------\n");
                if (Directory.Exists("minidumps"))
                {
                    if (Directory.GetFiles("minidumps").Length > 1)
                    {
                        foreach (var file in Directory.EnumerateFiles("minidumps"))
                        {
                            File.Copy(file, Path.Combine(@"ReporterTemp", Path.GetFileName(file)));
                        }
                        sw.WriteLine($"Found {Directory.GetFiles("minidumps").Length} minidumps.");
                    }
                }
                else
                {
                    sw.WriteLine("No minidumps were found.");
                }

                // Get Project-Bo4 dir files
                sw.WriteLine("\n-----------project-bo4 directory-----------\n");
                try
                {
                    foreach (var file in Directory.EnumerateFiles("project-bo4", "*", SearchOption.AllDirectories))
                    {
                        sw.WriteLine(file);
                    }
                }
                catch (Exception e) { sw.WriteLine(e.Message); }
            }

            // Prepere for sending
            if (Directory.GetFiles("ReporterTemp").Length > 1)
            {
                ZipFile.CreateFromDirectory(tempReportDir, $"SEND-ME-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.zip");
            }
            else
            {
                File.Move(shieldInfoFilePath, shieldInfoFileName); 
            }
            Directory.Delete(tempReportDir, true); // Always delete this, the zip or txt file will remain afterwards
        }
    }
}
