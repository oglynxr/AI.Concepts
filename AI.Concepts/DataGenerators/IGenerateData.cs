using System.IO;

namespace AI.Concepts.DataGenerators
{
    /// <summary>
    /// Generates data for use in training and testing.
    /// </summary>
    public interface IGenerateData
    {
        /// <summary>
        /// Writes the data to the stream, this is intended for writing data.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        public void Generate( Stream stream );

        /// <summary>
        /// Writes teh data to a file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void Generate( string fileName );
    }
}
