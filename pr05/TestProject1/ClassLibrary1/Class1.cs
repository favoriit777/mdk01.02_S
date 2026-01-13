namespace AssertMethodsDemo
{
    public class ArrayProcessor
    {
        public int[] SortArray(int[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            var sorted = (int[])array.Clone();
            Array.Sort(sorted);
            return sorted;
        }

        public int[] MergeArrays(int[] arr1, int[] arr2)
        {
            if (arr1 == null || arr2 == null)
                throw new ArgumentNullException();
            return arr1.Concat(arr2).ToArray();
        }

        public double FindMedian(int[] array)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Array cannot be null or empty");
            var sorted = (int[])array.Clone();
            Array.Sort(sorted);
            int middle = sorted.Length / 2;
            if (sorted.Length % 2 == 0)
            {
                return (sorted[middle - 1] + sorted[middle]) / 2.0;
            }
            else
            {
                return sorted[middle];
            }
        }
    }
}