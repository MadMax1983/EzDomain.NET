using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EzDomain.EventSourcing.EventStores.Sql.Data
{
    public sealed class SqlStatementsLoader
        : ISqlStatementsLoader
    {
        private readonly Dictionary<string, string> _sqlStatements = new();

        public void LoadScripts(string filesPath = "Data\\SqlScripts")
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            var executingAssemblyFileInfo = new FileInfo(executingAssembly.Location);
            
            var currentDirPath = executingAssemblyFileInfo.DirectoryName!;

            var sqlFilesPath = Path.Combine(currentDirPath, filesPath);

            var sqlFilesDir = new DirectoryInfo(sqlFilesPath);
            if (!sqlFilesDir.Exists)
            {
                return;
            }

            sqlFilesDir
                .GetFiles()
                .Where(file => file.Extension.Equals(".sql"))
                .ToList()
                .ForEach(file => _sqlStatements.Add(file.Name.Split('.')[0], File.ReadAllText(file.FullName)));
        }

        public string this[string key] => key.ToLower().Contains("async")
            ? _sqlStatements[key.Substring(0, key.Length - 5)]
            : _sqlStatements[key];
    }
}