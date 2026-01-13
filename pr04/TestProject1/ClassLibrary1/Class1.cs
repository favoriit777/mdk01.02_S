using System;

namespace StringCalculatorLib
{
    public class Calculator
    {
        /// <summary>
        /// Сортирует массив целых чисел по возрастанию методом пузырька
        /// </summary>
        /// <param name="array">Исходный массив для сортировки</param>
        /// <returns>Отсортированный по возрастанию массив</returns>
        public int[] BubbleSort(int[] array)
        {
            // Проверка на null
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Массив не может быть null");

            // Создаем копию массива, чтобы не изменять оригинал
            int[] sortedArray = (int[])array.Clone();
            int n = sortedArray.Length;

            // Реализация алгоритма пузырьковой сортировки
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (sortedArray[j] > sortedArray[j + 1])
                    {
                        // Обмен элементов
                        int temp = sortedArray[j];
                        sortedArray[j] = sortedArray[j + 1];
                        sortedArray[j + 1] = temp;
                    }
                }
            }

            return sortedArray;
        }
    }
}