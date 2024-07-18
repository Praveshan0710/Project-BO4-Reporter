using System.IO.Compression;

namespace Project_BO4_Report
{
    internal class Program
    {
        static void Main()
        {
            //Check if bo4 in dir

            if (!IsBo4Dir())
            {
                Console.WriteLine("Please run this application in your Black Ops 4 directory");
                Console.ReadKey(true);
                return;
            }

            Directory.CreateDirectory("ReporterTemp");
            using (var sw = new StreamWriter($@"ReporterTemp\ShieldInfo{DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss")}.txt", true))
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
                //Get minidumps of any
                 sw.WriteLine("\n-----------minidumps-----------\n");
                if (Directory.Exists("minidumps"))
                {
                    if (Directory.GetFiles("minidumps").Length > 1)
                    {
                        foreach (var file in Directory.EnumerateFiles("minidumps"))
                        {
                            File.Copy(file, Path.Combine(@"ReporterTemp", Path.GetFileName(file)));
                        }
                    }
                }
                else
                {
                    sw.WriteLine("No minidumps found.");
                }
            }
            try
            {
                ZipFile.CreateFromDirectory("ReporterTemp", $"SEND-ME-{DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss")}.zip");
            }
            catch (Exception) { }
            finally
            {
                Directory.Delete("ReporterTemp", true);
            }
            //Console.WriteLine("");
        }
        private static bool IsBo4Dir()
        {
            return File.Exists("BlackOps4.exe");
        }
    }
}
