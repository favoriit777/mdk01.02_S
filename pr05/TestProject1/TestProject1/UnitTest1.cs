using Xunit;
using AssertMethodsDemo;

namespace AssertMethodsDemo.Tests
{
    public class ArrayProcessorTests
    {
        private readonly ArrayProcessor _processor;

        public ArrayProcessorTests()
        {
            _processor = new ArrayProcessor();
        }

        [Fact]
        public void SortArray_ValidArray_ReturnsSortedArray()
        {
            // Arrange
            int[] input = { 5, 3, 8, 1, 2 };

            // Act
            var result = _processor.SortArray(input);

            // Assert - проверка на сортировку
            Assert.NotNull(result);
            Assert.Equal(new int[] { 1, 2, 3, 5, 8 }, result);
            Assert.All(result, item => Assert.InRange(item, 1, 8));
        }

        [Fact]
        public void SortArray_NullArray_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _processor.SortArray(null));
        }

        [Fact]
        public void MergeArrays_ValidInput_ReturnsCombinedArray()
        {
            // Arrange
            int[] arr1 = { 1, 2 };
            int[] arr2 = { 3, 4 };

            // Act
            var result = _processor.MergeArrays(arr1, arr2);

            // Assert - проверка коллекций
            Assert.NotNull(result);
            Assert.Equal(4, result.Length);
            Assert.Contains(3, result);
            Assert.DoesNotContain(0, result);
            Assert.All(result, item => Assert.InRange(item, 1, 4));
            Assert.IsAssignableFrom<int[]>(result);
        }

        [Fact]
        public void FindMedian_OddLengthArray_ReturnsMiddleElement()
        {
            // Arrange
            int[] array = { 5, 1, 9 };
            // Act
            var median = _processor.FindMedian(array);
            // Assert
            Assert.Equal(5, median);
            Assert.IsType<double>(median);
            Assert.True(median > 0);
        }

        [Fact]
        public void FindMedian_EvenLengthArray_ReturnsAverageOfMiddle()
        {
            // Arrange
            int[] array = { 1, 3, 2, 4 };
            // Act
            var median = _processor.FindMedian(array);
            // Assert
            Assert.Equal(2.5, median);
        }

        [Fact]
        public void FindMedian_EmptyArray_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _processor.FindMedian(new int[] { }));
        }
    }
}