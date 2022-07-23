namespace TG.JSON
{
#if NETSTANDARD1_0 || NETSTANDARD1_3
    /// <summary>
    /// Specify the sort direction. Implemented for NetStandard 1.0 and 1.3
    /// </summary>
    public enum ListSortDirection
    {
        /// <summary>
        /// Sort Ascending
        /// </summary>
        Ascending,
        /// <summary>
        /// Sort Descending
        /// </summary>
        Descending
    } 
#endif
}