using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Brickficiency.App
{
    public class ApplicationInitializer
    {
        private readonly string _programDataPath;

        public ApplicationInitializer()
        {
            _programDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Constants.ApplicationName);
        }

        public void Run()
        {
            Directory.CreateDirectory(_programDataPath);
            Directory.CreateDirectory(Path.Combine(_programDataPath, "debug"));
            EnsureImageDirectoriesExist();
        }

        private void EnsureImageDirectoriesExist()
        {
            var subDirectoryPaths = new[] { "images", "images\\S", "images\\P", "images\\M", "images\\B", "images\\G", "images\\C", "images\\I", "images\\O", "images\\U" };

            foreach (string subDirectoryPath in subDirectoryPaths)
            {
                string folderPath = Path.Combine(_programDataPath, subDirectoryPath);
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}
