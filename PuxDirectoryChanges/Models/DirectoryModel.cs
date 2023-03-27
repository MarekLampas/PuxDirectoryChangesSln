using System.ComponentModel.DataAnnotations;

namespace PuxDirectoryChanges.Models
{
    /// <summary>
    /// Model used to get directory path entered by user in controller and use it to set up states holder
    /// </summary>
    public class DirectoryModel
    {
        [Required(ErrorMessage = "Direcory path must be defined")]
        public string DirectoryPath { get; set; }
    }
}
