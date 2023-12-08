using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    internal class LevelHandler
    {
        public int[,] Grid { get; private set; }
        public int[] StartPoint { get; private set; }

        //could use these properties or something like them in the future to spawn monsters/relics
        public List<int[]> MonsterSpawnPoints { get; private set; }

        public ArrayList RelicSpawnPoints { get; private set; }

        public List<int[]> SpikeLocations { get; private set; }

        //in the int grid - here is what the values mean:
        //0 = nothing/floor
        //1 = wall
        //2 = Relic
        //3 = Switch/Button
        //4 = Monster Spawn Point
        //5 = Player Spawn Point
        //6 = Level Complete Point
        //8 = Power Up
        //9 = Spikes
        //10 = door

        public LevelHandler(string csvFilePath)
        {
            List<string[]> rows = new List<string[]>();
            using (var reader = new StreamReader(csvFilePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    var values = line.Split(',');
                    values = values.Where(val => !string.IsNullOrEmpty(val)).ToArray(); // Filter out empty or null values
                    rows.Add(values);
                }
            }

            Grid = new int[rows.Count, rows[0].Length]; 
            MonsterSpawnPoints = new List<int[]>();
            RelicSpawnPoints = new ArrayList();
            SpikeLocations = new List<int[]>();

            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    Grid[i, j] = int.Parse(rows[i][j]);

                    switch (Grid[i, j])
                    {
                        case 0:
                        case 1:
                            break;
                        case 2:
                            RelicSpawnPoints.Add(new int[] { j, i });
                            break;
                        case 4: // Monster Spawn Point
                            MonsterSpawnPoints.Add(new int[] { j, i });
                            break;
                        case 5: // Player Spawn Point
                            StartPoint = new int[] { j, i };
                            break;
                        case 9: // Spikes
                            SpikeLocations.Add(new int[] { j, i });
                            break;
                        default:
                            break;

                    }
                    if (Grid[i, j] == 5)
                    {
                        StartPoint = new int[2];
                        StartPoint[0] = j;
                        StartPoint[1] = i;
                    }

                }
            }
        }
    }
}
