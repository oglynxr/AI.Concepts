using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AI.Concepts.DataGenerators
{
    public class FarOutGenerator : IGenerateData
    {
        public const int MinimumDataPoints = 1_000;
        private const int Dimensions = 2;
        private const int Segments = 2;

        private double lower;
        private double lowerFromOrigin;
        private double upper;
        private double upperFromOrigin;
        private double segmentDistance;

        private int dataPoints;
        private Random random;

        public FarOutGenerator() : this( 0, 10 ) { }

        public FarOutGenerator( double lower, double upper ) : this( lower, upper, 10_000 ) { }

        public FarOutGenerator( double lower, double upper, int dataPoints ) : this( lower, upper, dataPoints, null ) { }

        public FarOutGenerator( double lower, double upper, int dataPoints, int? seed )
        {
            if ( lower >= upper ) throw new ArgumentException( $"{ nameof( lower ) } must be less than { nameof( upper ) }." );
            if ( dataPoints < MinimumDataPoints ) throw new ArgumentException( $"{ nameof( dataPoints ) } must be greater than or equal to { MinimumDataPoints }." );

            this.lower = lower;
            this.upper = upper;

            lowerFromOrigin = Distance( Enumerable.Range( 0, Dimensions ).Select( _ => lower ) );
            upperFromOrigin = Distance( Enumerable.Range( 0, Dimensions ).Select( _ => upper ) );
            segmentDistance = ( upperFromOrigin - lowerFromOrigin ) / Segments;

            this.dataPoints = dataPoints;
            random = seed == null ? new Random() : new Random( seed.Value );
        }

        public void Generate( Stream stream )
        {
            if ( stream == null ) throw new ArgumentNullException( nameof( stream ) );

            using StreamWriter writer = new StreamWriter( stream );
            writer.Write( JsonConvert.SerializeObject( Generate() ) );
        }

        public void Generate( string fileName )
        {
            if ( string.IsNullOrWhiteSpace( fileName ) ) throw new ArgumentNullException( nameof( fileName ) );

            using FileStream fileStream = new FileStream( fileName, FileMode.Create );
            Generate( fileStream );
        }

        private double[][] Generate() =>
            Enumerable
                .Range( 0, dataPoints )
                .Select( _ => CreateDataPoint().ToArray() )
                .ToArray();

        private IEnumerable<double> CreateDataPoint()
        {
            var dimensions = CreateRandomDimensions().ToArray();

            return dimensions.Concat( new double[] { DetermineClass( dimensions ) } );
        }

        private IEnumerable<double> CreateRandomDimensions() =>
            Enumerable
                .Range( 0, Dimensions )
                .Select( _ => NextDouble() );

        private double DetermineClass( IEnumerable<double> dimensions ) =>
            Math.Floor( ( Distance( dimensions ) - lowerFromOrigin ) / segmentDistance );

        private double Distance( IEnumerable<double> dimensions ) => Math.Sqrt( DistanceSquared( dimensions ) );

        private double DistanceSquared( IEnumerable<double> dimensions ) =>
            dimensions
                .Select( dimension => Math.Pow( dimension, 2 ) )
                .Sum();

        private double NextDouble() => random.NextDouble() * ( upper - lower ) + lower;
    }
}
