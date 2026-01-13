using System;

namespace ParametrizedTestsDemo
{
    public class ShapeCalculator
    {
        /// <summary>
        /// Вычисляет площадь прямоугольника
        /// </summary>
        /// <param name="width">Ширина прямоугольника</param>
        /// <param name="height">Высота прямоугольника</param>
        /// <returns>Площадь прямоугольника</returns>
        public double CalculateRectangleArea(double width, double height)
        {
            if (width <= 0)
                throw new ArgumentException("Ширина должна быть положительным числом", nameof(width));

            if (height <= 0)
                throw new ArgumentException("Высота должна быть положительным числом", nameof(height));

            return width * height;
        }

        /// <summary>
        /// Вычисляет объем куба
        /// </summary>
        /// <param name="side">Длина стороны куба</param>
        /// <returns>Объем куба</returns>
        public double CalculateCubeVolume(double side)
        {
            if (side <= 0)
                throw new ArgumentException("Длина стороны должна быть положительным числом", nameof(side));

            return Math.Pow(side, 3);
        }

        /// <summary>
        /// Вычисляет объем сферы
        /// </summary>
        /// <param name="radius">Радиус сферы</param>
        /// <returns>Объем сферы</returns>
        public double CalculateSphereVolume(double radius)
        {
            if (radius <= 0)
                throw new ArgumentException("Радиус должен быть положительным числом", nameof(radius));

            return (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);
        }
    }
}