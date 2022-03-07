using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AwariGameWpf.Persistence
{
    public class DataAccess : IDataAccess
    {
        public async Task<(Dictionary<int, int>, bool, bool)> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line = await reader.ReadLineAsync();
                    int bowlAmount = int.Parse(line);

                    line = await reader.ReadLineAsync();
                    bool activePlayer = bool.Parse(line);

                    line = await reader.ReadLineAsync();
                    bool canAgain = bool.Parse(line);

                    line = await reader.ReadLineAsync();
                    string[] numbers = line.Split(' ');
                    Dictionary<int, int> bowls = new Dictionary<int, int>();

                    for (int i = 0; i < bowlAmount + 2; i++)
                    {
                        bowls.Add(i, int.Parse(numbers[i]));
                    }

                    return (bowls, activePlayer, canAgain);
                }
            }
            catch
            {
                throw new AwariDataException();
            }
        }

        public async Task SaveAsync(string path, Dictionary<int, int> bowls, bool player, bool again)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(bowls.Count - 2);
                    writer.WriteLine(player);
                    writer.WriteLine(again);

                    for (int i = 0; i < bowls.Count; i++)
                    {
                        await writer.WriteAsync(bowls[i] + " ");
                    }
                }
            }
            catch
            {
                throw new AwariDataException();
            }
        }
    }
}
