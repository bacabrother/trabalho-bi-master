using System;
using System.Diagnostics;
using System.Linq;

namespace GeneticSharp
{
    /// <summary>
    /// Represents a gene of a chromosome.
    /// </summary>
    [DebuggerDisplay("{Value} || {badPosition}")]
    [Serializable]
    public struct Gene : IEquatable<Gene>
    {
        #region Fields
        private object m_value;
        public int badPosition;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Gene"/> struct.
        /// </summary>
        /// <param name="value">The gene initial value.</param>
        public Gene(object value)
        {
            m_value = value;
            badPosition = 1;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public Object Value
        {
            get
            {
                return m_value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Gene first, Gene second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Gene first, Gene second)
        {
            return !(first == second);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Gene"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Gene"/>.</returns>
        public override string ToString()
        {
            return Value != null ? Value.ToString() + " - " + badPosition.ToString() : String.Empty;

        }

        /// <summary>
        /// Determines whether the specified <see cref="GeneticSharp.Gene"/> is equal to the current <see cref="GeneticSharp.Gene"/>.
        /// </summary>
        /// <param name="other">The <see cref="GeneticSharp.Gene"/> to compare with the current <see cref="GeneticSharp.Gene"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="GeneticSharp.Gene"/> is equal to the current
        /// <see cref="GeneticSharp.Gene"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(Gene other)
        {
            if (Value == null)
                return other.Value == null;

            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GeneticSharp.Gene"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="GeneticSharp.Gene"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="GeneticSharp.Gene"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Gene other)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (Value == null)
            {
                return 0;
            }

            return Value.GetHashCode();
        }
        #endregion
    }


    public static class GeneExtensions
    {
        /// <returns>First -> gene, Second -> index</returns>
        public static (Gene, int) GetGeneWeighted(this Gene[] genes, int indexWeighted)
        {
            int lastRemaining = indexWeighted;
            int remaning = indexWeighted;
            for (int i = 0; i < genes.Length; i++)
            {
                remaning -= genes[i].badPosition;
                if (remaning < 0)
                {
                    return (genes[i], i);
                }
                lastRemaining = remaning;
            }
            return (genes.Last(), genes.Length - 1);
        }
    }
}
