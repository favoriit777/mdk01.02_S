# Практическая работа 5. Использование различных методов Assert в xUnit

**Вариант: 15**

**Задание:**
Вариант 15: Класс ArrayProcessor с методами:

int[] SortArray(int[] array)
int[] MergeArrays(int[] arr1, int[] arr2)
int FindMedian(int[] array)
## Структура проекта

- `StringCalculatorLib/`: Реализация метода `ReverseString`.
- `StringCalculatorLib.Tests/`: Юнит-тесты на данный метод.
- `images/`: Скриншот результатов тестирования.
- `README.md`: Инструкция и описание.

## Класс `StringUtils`

```csharp
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
```

```csharp
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
```
<img src="Снимок.JPG">