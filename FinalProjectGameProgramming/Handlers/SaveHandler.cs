using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinalProjectGameProgramming.Handlers
{
    public class SaveHandler
    {
        private int score { get; set; }
        private int level { get; set; }
        public SaveHandler() { }
        public SaveHandler(int score, int level)
        {
            this.score = score;
            this.level = level;
            SaveFile();
        }
        // Get Default Folder Path from Settings: Project --> Properties --> Settings
        static string defaultFolderPath = Properties.Settings.Default.FolderPath;
        static string directoryName = "Medieval Maze"; // Name of the directory to be created
        static string saveFileName = "savegame.txt"; // Name of the save file
        string directoryPath = Path.Combine(defaultFolderPath, directoryName); // Combine the default folder path with the directory name
        public void SaveFile()
        {
            try
            {
                //// Define a relative path for the save file
                //string relativePath = "SaveFiles";
                //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //string directoryPath = Path.Combine(baseDirectory, relativePath);

                //// Ensure the directory exists
                //if (!Directory.Exists(directoryPath))
                //{
                //    Directory.CreateDirectory(directoryPath);
                //}

                //// Define the file path (change 'savegame.txt' to your preferred filename)
                //string filePath = Path.Combine(directoryPath, "savegame.txt");

                //using (StreamWriter writer = new StreamWriter(filePath))
                //{
                //    writer.WriteLine($"Level: {level}");
                //    writer.WriteLine($"Score: {score}");
                //}

                //// Write the game data to the file
                ////File.WriteAllText(filePath, $"Level: {level}, Score: {score}");
                //Console.WriteLine("Game saved successfully.");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string filePath = Path.Combine(directoryPath, saveFileName); // Ensure C:\temp exists

                File.WriteAllText(filePath, $"{level},{score}");
                Console.WriteLine("Game saved successfully to " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving the game: " + ex.Message);
            }


        }

        public string[] LoadFile()
        {
            string filePath = Path.Combine(directoryPath, saveFileName); // Ensure C:\temp exists
            Console.WriteLine(filePath);
            // string filePath = @"C:\temp\savegame.txt"; // The path to your save file

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read all lines from the file 
                    string fileContent = File.ReadAllText(filePath);

                    // Split the content by comma
                    string[] parts = fileContent.Split(',');

                    if (parts.Length == 2) // Ensure there are at least two parts (level and score)
                    {
                        return parts;
                    }
                    else
                    {
                        Console.WriteLine("Invalid file format.");
                    }
                }
                else
                {
                    Console.WriteLine("Save file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading the game: " + ex.Message);
            }
            return null;
        }

        public void DeleteSave()
        {
            // The path to your save file
            string filePath = Path.Combine(directoryPath, saveFileName);
            // string filePath = @"C:\temp\savegame.txt"; 

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Delete the file
                    File.Delete(filePath);
                    Console.WriteLine("Save file deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Save file not found or already deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting the save file: " + ex.Message);
            }
        }
    }
}
