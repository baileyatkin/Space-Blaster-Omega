using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyLogic
{

    public class FuzzyVariable
    {
        private Dictionary<string, FuzzySet> sets = new Dictionary<string, FuzzySet>();
        public FuzzyVariable()
        {
        }

        public void AddFuzzySet(string name, FuzzySet set)
        {
            sets[name] = set;
        }

        public string GetHighestDOM()
        {
            string highestDOM = "";
            foreach (KeyValuePair<string, FuzzySet> set in sets)
            {
                if (string.IsNullOrEmpty(highestDOM))
                {
                    highestDOM = set.Key;
                }
                else
                {
                    if (sets[highestDOM].GetDOM() < set.Value.GetDOM())
                    {
                        highestDOM = set.Key;
                    }
                }
            }
            return highestDOM;
        }

        public FuzzySet GetSet(string name)
        {
            if (sets.ContainsKey(name))
            {
                return sets[name];
            }

            return null;
        }

        public void SetDOM(float crispVal)
        {
            foreach (KeyValuePair<string, FuzzySet> set in sets)
            {
                float dom = set.Value.CalculateDOM(crispVal);
                set.Value.SetDOM(dom);
            }
        }
    }
}
