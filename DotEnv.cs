using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace farm2plate
{
    public class DotEnv
    {
       public static void LoadDotEnv()
       {
            var rootPath = Directory.GetCurrentDirectory();
            string path = Path.Combine(rootPath, ".env");
            if (!File.Exists(path))
            {
                return;
            }
            
            // Iterate over each line in .env
            foreach (var Env in File.ReadAllLines(path))
            {
                // Splitting NAME=VALUE into [name, value]
                var envParts = Env.Split("=", StringSplitOptions.RemoveEmptyEntries);

                // Shouldn't be possible
                if (envParts.Length != 2)
                {
                    continue;
                }

                // Sets NAME to VALUE
                Environment.SetEnvironmentVariable(envParts[0], envParts[1]);
            }
       } 

    }
}
