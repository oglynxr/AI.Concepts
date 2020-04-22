using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.Concepts.Algorithms
{
    public class GaussianNaiveBayes
    {
        private double[][] means;

        private double[][] variances;

        private double[] classes;

        private int features;

        public void Fit( double[][] trainingData, double[] trainingExpectedValues )
        {
            classes = trainingExpectedValues.Distinct().ToArray();
            Array.Sort( classes );

            means = new double[ classes.Length ][];
            variances = new double[ classes.Length ][];

            List<double[]>[] classGroups = new List<double[]>[ classes.Length ];

            for ( int i = 0; i < classGroups.Length; i++ ) classGroups[ i ] = new List<double[]>();

            for ( int i = 0; i < trainingExpectedValues.Length; i++ )
            {
                int index = Array.BinarySearch( classes, trainingExpectedValues[ i ] );
                classGroups[ index ].Add( trainingData[ i ] );
            }

            for ( int i = 0; i < classGroups.Length; i++ )
            {
                var data = classGroups[ i ].ToArray();
                means[ i ] = CalculateMeans( data );
                variances[ i ] = CalculateVariances( data, means[ i ] );
            }

            features = means[ 0 ].Length;
        }

        public double[] Predict( double[][] values ) => values.Select( Predict ).ToArray();

        public double Predict( double[] values )
        {
            double prediction = classes[ 0 ];

            if ( values.Length == features )
            {
                int maxIndex = -1;
                double maxValue = 0;

                for ( int i = 0; i < classes.Length; i++ )
                {
                    double classProbability = 1;
                    for ( int j = 0; j < features; j++ )
                    {
                        classProbability *= Predict( values[ j ], means[ i ][ j ], variances[ i ][ j ] );
                    }

                    if ( classProbability > maxValue )
                    {
                        maxIndex = i;
                        maxValue = classProbability;
                    }
                }

                prediction = classes[ maxIndex ];
            }

            return prediction;
        }

        private double Predict( double value, double mean, double variance )
        {
            double numerator = -Math.Pow( value - mean, 2 );
            double denominator = 2 * variance;

            return Math.Pow( Math.E, numerator / denominator ) / Math.Sqrt( denominator * Math.PI );
        }

        private double[] CalculateMeans( double[][] data )
        {
            double[] means = new double[ 0 ];

            if ( data.Length > 0 )
            {
                means = new double[ data[ 0 ].Length ];
                double[] featureSums = new double[ means.Length ];

                for ( int i = 0; i < data.Length; i++ )
                {
                    for ( int j = 0; j < featureSums.Length; j++ )
                    {
                        featureSums[ j ] += data[ i ][ j ];
                    }
                }

                for ( int i = 0; i < featureSums.Length; i++ )
                {
                    means[ i ] = featureSums[ i ] / data.Length;
                }
            }

            return means;
        }

        private double[] CalculateVariances( double[][] data, double[] means )
        {
            double[] variance = new double[ 0 ];

            if ( data.Length > 0 && means.Length > 0 )
            {
                variance = new double[ means.Length ];
                double[] featureSums = new double[ variance.Length ];

                for ( int i = 0; i < data.Length; i++ )
                {
                    for ( int j = 0; j < featureSums.Length; j++ )
                    {
                        featureSums[ j ] += Math.Pow( data[ i ][ j ] - means[ j ], 2 );
                    }
                }

                for ( int i = 0; i < featureSums.Length; i++ )
                {
                    variance[ i ] = featureSums[ i ] / ( data.Length - 1 );
                }
            }

            return variance;
        }
    }
}
