using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyLogic
{
    public abstract class FuzzySet
    {
        //Set up the parent class FuzzySet for Triangle and Trapezoid Fuzzy Sets
        public string name;
        private float dom;

        public FuzzySet(string setName)
        {
            name = setName;
        }

        public void SetDOM(float setDom) { dom = setDom; }
        public float GetDOM() { return dom; }
        public void ClearDOM() { dom = 0; }
        public abstract float CalculateDOM(float value);
    }

    //Class for Fuzzy Sets that are best represented by a Triangle shape on a graph, best used to represent the distances between the player, enemies and projectiles.
    public class FuzzySetTriangle : FuzzySet
    {
        //This class calculates the degree of membership within a set that is best represented by a triangle, 
        public float peak;
        public float left;
        public float right;

        public FuzzySetTriangle(string setName, float leftVal, float peakVal, float rightVal) : base(setName)
        {
            peak = peakVal;
            left = leftVal;
            right = rightVal;
        }

        public override float CalculateDOM(float value)
        {
            //If the value is the peak of the set, then return a DOM of 1
            if (value == peak)
            {
                return 1.0f;
            }
            //Use linear interpolation between the left/right slopes of the set triangle to calculate and return the degree of membership
            else if (value < peak)
            {
                return ((value - left) / (peak - left));
            }
            else if (value > peak)
            {
                return ((right - value) / (right - peak));
            }
            //If none of the other conditions are met, the value must not fall within the set
            else
            {
                return 0.0f;
            }
        }
    }

    //Class for Fuzzy Sets that are represented by a Trapezium shape on a graph, best used to represent the speed of the player or how much they are covered by the bunkers
    public class FuzzySetTrapezoid : FuzzySet
    {
        //Declare the points of the Trapzeium set where the DOM is at 0 and 1.
        public float leftMax;
        public float rightMax;
        public float leftMin;
        public float rightMin;

        public FuzzySetTrapezoid(string setName, float leftMinVal, float leftMaxVal, float rightMaxVal, float rightMinVal) : base(setName)
        {
            leftMax = leftMaxVal;
            rightMax = rightMaxVal;
            leftMin = leftMinVal;
            rightMin = rightMinVal;
        }

        public override float CalculateDOM(float value)
        {
            //Use linear interpolation to calculate where the value lies on the left/right slopes of the trapezium set if it valls on either of those slopes
            if (value >= leftMin && value <= leftMax)
            {
                return (value - leftMin) / (leftMax - leftMin);
            }
            else if (value > rightMax && value <= rightMin)
            {
                return (rightMin - value) / (rightMax - rightMin);
            }
            //If the value falls between both the left and right maximum points of the trapezium, the value has full membership within this set so return 1 to show that
            else if (value >= leftMax && value <= rightMax)
            {
                return 1.0f;
            }
            //If previous conditions haven't been met, then the value must not fall within the fuzzy set, so return 0 to show that it has no DOM within the set
            else
            {
                return 0.0f;
            }
        }
    }
}
