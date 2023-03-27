using PuxDirectoryChanges.Models;
using System.IO;
using System.Reflection;
using static PuxDirectoryChanges.Helpers.Enums;

namespace PuxDirectoryChanges.Helpers
{
    /// <summary>
    /// Holder to keep states of files in defined derectory
    /// Contains app logic and returns data that are displayed in changes page
    /// </summary>
    public class StatesHolder
    {
        private readonly ILogger<StatesHolder> _logger;

        public StatesHolder(ILogger<StatesHolder> logger)
        {
            _logger = logger;
        }

        public string DirectoryPath { get; set; }
        private List<FileState> FileStates { get; set; }

        /// <summary>
        /// Helper method to check if directory was inserted
        /// </summary>
        /// <returns></returns>
        public bool IsDefinedDirectoryPath()
        {
            return !string.IsNullOrEmpty(DirectoryPath);
        }

        /// <summary>
        /// Initial set up of states holder
        /// Scans defined directory and saves states of all files contained
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public bool SetupDirectoryState(string directoryPath)
        {
            if (!string.IsNullOrEmpty(DirectoryPath) && (DirectoryPath == directoryPath))
            {
                return true;
            }

            try
            {
                DirectoryPath = directoryPath;

                FileStates = new List<FileState>();

                foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
                {
                    FileStates.Add(new FileState()
                    {
                        FilePath = filePath,
                        State = Enums.EState.Unchunaged,
                        Version = 1,
                        Modified = File.GetLastWriteTime(filePath)
                    });
                }

                _logger.LogInformation($"State holder setted up for directory '{directoryPath}'");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Setting up state holder for directory '{directoryPath}' failed");

                return false;
            }
        }

        /// <summary>
        /// Conpares saved states of files with actual state
        /// Returns changed files grouped by 3 categories (added, modified and deleted files)
        /// </summary>
        /// <returns></returns>
        public Dictionary<EState, FileState[]> GetFileStatesToDisplay()
        {
            //We dont want to track deleted files anymore
            var fileStates = FileStates.Where(s => s.State != Enums.EState.Deleted).Select(s => new StateModified() { State = s, Checked = false }).ToList();

            foreach (var filePath in Directory.EnumerateFiles(DirectoryPath, "*", SearchOption.AllDirectories))
            {
                var fileState = fileStates.FirstOrDefault(s => s.State.FilePath == filePath);

                var fileModified = File.GetLastWriteTime(filePath);

                if (fileState == null)
                {
                    fileStates.Add(new StateModified(){
                        State = new FileState()
                        {
                            FilePath = filePath,
                            State = Enums.EState.Added,
                            Version = 1,
                            Modified = fileModified
                        },
                        Checked = true });
                }
                else if (fileState.State.Modified != fileModified)
                {
                    fileState.State.State = Enums.EState.Modified;
                    fileState.State.Version += 1;
                    fileState.State.Modified = fileModified;
                    fileState.Checked = true;
                }
                else
                {
                    fileState.State.State = Enums.EState.Unchunaged;
                    fileState.Checked = true;
                }
            }

            //if some file is tracked by states holder but it has not been checked, it was deleted since last run
            foreach(var fileState in fileStates.Where(s => !s.Checked))
            {
                fileState.State.State = Enums.EState.Deleted;
            }

            FileStates = fileStates.Select(s => s.State).ToList();

            _logger.LogInformation($"{fileStates.Count} have changed since last run");

            return new Dictionary<EState, FileState[]>()
            {
                { EState.Added, FileStates.Where(s => s.State == EState.Added).ToArray() },
                { EState.Modified, FileStates.Where(s => s.State == EState.Modified).ToArray() },
                { EState.Deleted, FileStates.Where(s => s.State == EState.Deleted).ToArray() }
            };
        }

        /// <summary>
        /// Cleans up states holder before defining new directory to track
        /// </summary>
        public void CleanUpStatesHolder()
        {
            DirectoryPath = string.Empty;

            FileStates.Clear();

            _logger.LogInformation($"State holder cleaned up");
        }

        /// <summary>
        /// Helper class to track which files still exists and which have been deleted
        /// </summary>
        public class StateModified
        {
            public FileState State { get; set; }
            public bool Checked { get; set; }
        }
    }
}
