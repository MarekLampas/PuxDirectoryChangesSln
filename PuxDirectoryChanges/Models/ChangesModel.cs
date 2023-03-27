using static PuxDirectoryChanges.Helpers.Enums;

namespace PuxDirectoryChanges.Models
{
    /// <summary>
    /// Model providing data to display in changes page
    /// Provides states of files that have been changed since last run
    /// File states are gouped by 3 categories (added, modified and deleted files)
    /// </summary>
    public class ChangesModel
    {
        public Dictionary<EState, FileState[]> FileStates { get; set; }
    }
}
