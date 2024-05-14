using System.IO;

namespace UnityProjectCreator.Util
{
    public static class FileUtil
    {
        public static void DeleteFiles(string searchPattern, string sourceDirectory, bool recursive)
        {
            DirectoryInfo directory = new DirectoryInfo(sourceDirectory);

            if (!directory.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {directory.FullName}");

            DirectoryInfo[] directories = directory.GetDirectories();

            foreach (FileInfo file in directory.GetFiles(searchPattern))
                 file.Delete();

            if (recursive)
            {
                foreach (DirectoryInfo subDir in directories)
                {
                    DeleteFiles(searchPattern, subDir.FullName, true);
                }
            }
        }

        public static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool recursive)
        {
            DirectoryInfo directory = new DirectoryInfo(sourceDirectory);

            if (!directory.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {directory.FullName}");

            DirectoryInfo[] directories = directory.GetDirectories();

            Directory.CreateDirectory(destinationDirectory);

            foreach (FileInfo file in directory.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in directories)
                {
                    string newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
