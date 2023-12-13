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
        static string leaderboardFileName = "leaderboard.txt"; // Name of the leaderboard file
        string directoryPath = Path.Combine(defaultFolderPath, directoryName); // Combine the default folder path with the directory name
        public void SaveFile()
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string filePath = Path.Combine(directoryPath, saveFileName); 

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
            string filePath = Path.Combine(directoryPath, saveFileName);

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

        public void SaveLeaderboardFile(string data)
        {
            try
            {

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string filePath = Path.Combine(directoryPath, leaderboardFileName); // Ensure C:\temp exists

                File.WriteAllText(filePath, data);
                Console.WriteLine("Leaderboard saved successfully to " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving the leaderboard file: " + ex.Message);
            }
        }

        public List<Dictionary<string, float>> LoadLeaderboardFile()
        {
            List<Dictionary<string, float>> collection = new List<Dictionary<string, float>>();
            string filePath = Path.Combine(directoryPath, leaderboardFileName);

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read all lines from the file 
                    string fileContent = File.ReadAllText(filePath);

                    // Split the content by \newline
                    string[] parts = fileContent.Split('\n');
                    foreach (string user in parts)
                    {
                        string[] userParts = user.Split(',');
                        if (userParts.Length == 2)
                        {
                            Dictionary<string, float> userScore = new Dictionary<string, float>();
                            userScore.Add(userParts[0], float.Parse(userParts[1]));
                            collection.Add(userScore);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Saved file not found.");
                }
                return collection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading the leaderboard file: " + ex.Message);
            }
            return null;
        }
    }
}
