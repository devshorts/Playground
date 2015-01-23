using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace GradientAscent
{
    public static class GradientRunner
    {
        private static double Sigmoid(double input)
        {
            return 1.0/(1 + Math.Pow(Math.E, (input * -1)));
        }

        public static Vector<double> GetWeights(Matrix<double> data, Vector<double> targetClassification)
        {            
            var features = data.ColumnCount;

            // these are things we are trying to solve for
            Vector<double> weights = DenseVector.Create(features, i => 1.0);

            var alpha = 0.001;

            foreach (var cycle in Enumerable.Range(0, 500))
            {
                #region Sigmoid Explanation

                /*
                 * multiply all the data by the weights, this gives you the estimation of the current function
                 * given the weights. multiplying by the sigmoid moves a point into one class vs the other
                 * if its larger than 0.5 it'll be in class 1, if its smaller than it'll be in class 0.  The closer it is
                 * to 1 means that with the given weights that value is highly probably to be in class 1.
                 * 
                 * it doesn't matter if the instance is the class the sigmoid says it is,
                 * the error will shift the weights gradient so over the iterations of the cycles 
                 * the weights will move the final data point towards its actual expected class
                 * 
                 * for example, if there is a data point with values    
                 * 
                 * [1.0, -0.017612, 14.053064]
                 * 
                 * where value 1 is the initial weight factor, and values 2 and 3 are the x y coordinates
                 *  
                 * and lets say the point is categorized at class 0.
                 * 
                 * Calculating the initial sigma gives you something like 0.9999998
                 * 
                 * which says its class 1, but thats obviously wrong.  However, the error rate here is large
                 * 
                 * meaning that the gradient wants to move towards the expected data.  
                 * 
                 * As you run the ascent this data point will get smaller and smaller and eventually
                 * 
                 * the sigmoid will classify it properly
                 */

                #endregion

                var currentData = DenseVector.OfEnumerable(data.Multiply(weights).Select(Sigmoid));
               
                #region Error Explanation

                // find out how far off we are from the actual expectation. this is
                // like the x2 - x1 part of a derivative

                #endregion

                var error = targetClassification.Subtract(currentData);

                #region Gradient Explanation

                // this gives you the direction of change from the current 
                // set of data.  At this point every point is moving in the direction
                // of the error rate.  A large error means we are far off and trying to move
                // towards the actual data, a low error rate means we are really close
                // to the target data (so the gradient will be smaller, meaning less delta)

                #endregion

                var gradient = data.Transpose() * error;

                #region Weights Update Explanation

                // multiplying by alpha means we'll take a small step in the direction
                // of the gradient and add it to the current weights. An initial weights of 1.0
                // means we're going to start at the current location of where we are.

                #endregion

                weights = weights + alpha * gradient;
            }

            return weights;
        }
    }
}
