// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Mapping
{
    public abstract class ControlPairProfile
    {
        public List<(Type, Type)> ControlModelPairs { get; set; }

        internal void CreatePair(Type type, Type type2)
        {
            if (ControlModelPairs == null)
            {
                ControlModelPairs = new List<(Type, Type)>();
            }

            ControlModelPairs.Add((type, type2));
        }

        internal void CreatePair<T, U>()
        {
            if (ControlModelPairs == null)
            {
                ControlModelPairs = new List<(Type, Type)>();
            }

            ControlModelPairs.Add((typeof(T), typeof(U)));
        }
    }
}
