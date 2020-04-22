using AI.Concepts.Algorithms;
using AI.Concepts.DataGenerators;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace AI.Concepts
{
    class Program
    {
        static void Main( string[] args )
        {
            FarOutGenerator generator = new FarOutGenerator();
            generator.Generate( "FarOutData.json" );

            var data = JsonConvert.DeserializeObject<double[][]>( File.ReadAllText( "FarOutData.json" ) );

            // split data
            int takeAmount = ( int )Math.Floor( data.Length * 0.9 );
            var training = data.Take( takeAmount );
            var trainingData = training.Select( dataPoint => dataPoint.Take( dataPoint.Length - 1 ).ToArray() ).ToArray();
            var trainingClasses = training.Select( dataPoint => dataPoint.Last() ).ToArray();

            var testing = data.Skip( takeAmount );
            var testingData = testing.Select( dataPoint => dataPoint.Take( dataPoint.Length - 1 ).ToArray() ).ToArray();
            var testingClasses = testing.Select( dataPoint => dataPoint.Last() ).ToArray();

            GaussianNaiveBayes model = new GaussianNaiveBayes();
            model.Fit( trainingData, trainingClasses );
            var predictions = model.Predict( testingData );

            var results = predictions.Select( ( prediction, index ) => testingData[ index ].Concat( new double[] { testingClasses[ index ], prediction } ).ToArray() ).ToArray();

            var accuracy = results.Count( result => result[ 2 ] == result[ 3 ] ) * 100.0 / results.Length;

            File.WriteAllText( "FarOutData-Results.json", JsonConvert.SerializeObject( results ) );
        }
    }
}
