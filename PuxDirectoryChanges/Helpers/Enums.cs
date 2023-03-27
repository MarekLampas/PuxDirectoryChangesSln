namespace PuxDirectoryChanges.Helpers
{
    public class Enums
    {
        /// <summary>
        /// Used to label type of change made to file
        /// </summary>
        public enum EState
        {
            Unchunaged = 0,
            Added = 1,
            Modified,
            Deleted
        }
    }
}
