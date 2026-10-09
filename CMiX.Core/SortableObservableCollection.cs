// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;

namespace CMiX.Core
{
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        // Constructors
        public SortableObservableCollection() : base() { }
        public SortableObservableCollection(List<T> l) : base(l) { }
        public SortableObservableCollection(IEnumerable<T> l) : base(l) { }

        #region Sorting
        
        /// <summary>
        /// Sorts the items of the collection in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        public void Sort<TKey>(Func<T, TKey> keySelector)
        {
            InternalSort(Items.OrderBy(keySelector));
        }

        /// <summary>
        /// Sorts the items of the collection in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        public void SortDescending<TKey>(Func<T, TKey> keySelector)
        {
            InternalSort(Items.OrderByDescending(keySelector));
        }

        /// <summary>
        /// Sorts the items of the collection in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            InternalSort(Items.OrderBy(keySelector, comparer));
        }

        /// <summary>
        /// Inserts an item at the position that keeps an already sorted collection sorted, which
        /// costs one insert instead of the full resort a plain add followed by a sort would cost.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="item">The item to insert.</param>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        public void AddSorted<TKey>(T item, Func<T, TKey> keySelector)
        {
            var comparer = Comparer<TKey>.Default;
            var key = keySelector(item);

            int low = 0;
            int high = Count;
            while (low < high)
            {
                int middle = low + (high - low) / 2;
                // Items with an equal key keep the order they were added in, which is what a
                // stable OrderBy over the whole collection would have produced.
                if (comparer.Compare(keySelector(Items[middle]), key) <= 0)
                    low = middle + 1;
                else
                    high = middle;
            }

            Insert(low, item);
        }

        /// <summary>
        /// Moves the items of the collection so that their orders are the same as those of the items provided.
        /// </summary>
        /// <param name="sortedItems">An <see cref="IEnumerable{T}"/> to provide item orders.</param>
        private void InternalSort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();
            var comparer = EqualityComparer<T>.Default;

            for (int targetIndex = 0; targetIndex < sortedItemsList.Count; targetIndex++)
            {
                var item = sortedItemsList[targetIndex];

                // Items already in place are skipped, so a collection that is still sorted costs
                // one comparison per item and raises no collection changed event at all.
                if (comparer.Equals(Items[targetIndex], item))
                    continue;

                var currentIndex = IndexOf(item);
                if (currentIndex >= 0)
                    Move(currentIndex, targetIndex);
            }
        }

        #endregion // Sorting
    }
}
