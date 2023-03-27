using static PuxDirectoryChanges.Helpers.Enums;

namespace PuxDirectoryChanges.Models
{
    /// <summary>
    /// Represents state of file
    /// Used to detect changes between app runs
    /// </summary>
    public class FileState
    {
        public string FilePath { get; set; }
        public EState State { get; set; }
        public int Version { get; set; }
        public DateTime Modified { get; set; }
    }
}
