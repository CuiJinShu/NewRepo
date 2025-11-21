using System;
using System.Linq;
using System.Text;

namespace PartnerRequests
{
    public class ProductionCalculator
    {
        /// <summary>
        /// Расчет количества материала, требуемого для производства продукции
        /// </summary>
        /// <param name="productTypeId">Идентификатор типа продукции</param>
        /// <param name="materialTypeId">Идентификатор типа материала</param>
        /// <param name="requiredQuantity">Требуемое количество продукции</param>
        /// <param name="stockQuantity">Количество продукции на складе</param>
        /// <param name="productParameter1">Параметр продукции 1 (вещественный, положительный)</param>
        /// <param name="productParameter2">Параметр продукции 2 (вещественный, положительный)</param>
        /// <returns>Количество необходимого материала или -1 при ошибке</returns>
        public static int CalculateMaterialQuantity(int productTypeId, int materialTypeId,
            int requiredQuantity, int stockQuantity,
            double productParameter1, double productParameter2)
        {
            try
            {
                // Проверка входных параметров на корректность
                if (requiredQuantity <= 0 || stockQuantity < 0 ||
                    productParameter1 <= 0 || productParameter2 <= 0)
                {
                    return -1; // Некорректные параметры
                }

                using (var db = new Entities())
                {
                    // Проверка существования типа продукции
                    var productType = db.ProductType_.FirstOrDefault(pt => pt.idProductType == productTypeId);
                    if (productType == null || productType.ProductTypeCoefficient == null)
                    {
                        return -1; // Тип продукции не найден
                    }

                    // Проверка существования типа материала
                    var materialType = db.MaterialsType_.FirstOrDefault(mt => mt.idMaterialsType == materialTypeId);
                    if (materialType == null || materialType.PercentageOfLossOfRawMaterials == null)
                    {
                        return -1; // Тип материала не найден
                    }

                    // Получаем коэффициенты
                    double productCoefficient = (double)productType.ProductTypeCoefficient;
                    double lossPercentage = (double)materialType.PercentageOfLossOfRawMaterials;

                    // Расчет необходимого количества продукции для производства
                    // Учитываем наличие продукции на складе
                    int productionQuantity = Math.Max(0, requiredQuantity - stockQuantity);

                    if (productionQuantity == 0)
                    {
                        return 0; // Вся продукция уже на складе
                    }

                    // Расчет количества материала на одну единицу продукции
                    // Согласно спецификации: произведение параметров продукции, умноженное на коэффициент типа продукции
                    double materialPerUnit = productParameter1 * productParameter2 * productCoefficient;

                    // Учет потерь сырья - увеличиваем необходимое количество
                    double effectiveMaterialPerUnit = materialPerUnit * (1 + lossPercentage);

                    // Расчет общего количества материала (округляем вверх до целого)
                    double totalMaterialNeeded = productionQuantity * effectiveMaterialPerUnit;
                    int materialQuantity = (int)Math.Ceiling(totalMaterialNeeded);

                    return materialQuantity >= 0 ? materialQuantity : -1;
                }
            }
            catch (Exception)
            {
                return -1; // Любая ошибка
            }
        }

        /// <summary>
        /// Расчет необходимых материалов для заявки (улучшенная версия)
        /// </summary>
        public static string CalculateMaterialsForRequest(int requestId)
        {
            try
            {
                using (var db = new Entities())
                {
                    var request = db.PartnerRequests_.FirstOrDefault(r => r.idRequest == requestId);
                    if (request == null) return "Заявка не найдена";

                    var result = new StringBuilder();
                    result.AppendLine($"РАСЧЕТ МАТЕРИАЛОВ ДЛЯ ЗАЯВКИ #{requestId}");
                    result.AppendLine($"Партнер: {request.Partners.CompanyName}");
                    result.AppendLine($"Дата: {request.RequestDate:dd.MM.yyyy}");
                    result.AppendLine($"Статус: {request.Status}");
                    result.AppendLine(new string('=', 60));

                    decimal totalMaterialCost = 0;
                    bool hasMaterials = false;
                    int productCount = 0;

                    foreach (var requestProduct in request.RequestProducts_)
                    {
                        productCount++;
                        var product = db.Products_.FirstOrDefault(p => p.idProduct == requestProduct.idProduct);
                        if (product != null)
                        {
                            result.AppendLine();
                            result.AppendLine($"[ПРОДУКЦИЯ #{productCount}] {product.ProductName}");
                            result.AppendLine($"  Артикул: {product.Article}");
                            result.AppendLine($"  Тип продукции: {product.ProductType_.NameProductType}");
                            result.AppendLine($"  Коэффициент типа: {product.ProductType_.ProductTypeCoefficient}");
                            result.AppendLine($"  Количество в заявке: {requestProduct.Quantity} шт.");
                            result.AppendLine($"  Цена за единицу: {requestProduct.UnitPrice:N2} р.");
                            result.AppendLine($"  Общая стоимость: {requestProduct.Quantity * requestProduct.UnitPrice:N2} р.");

                            // ДЕМОНСТРАЦИЯ РАБОТЫ НОВОГО МЕТОДА
                            result.AppendLine();
                            result.AppendLine("  ДЕМОНСТРАЦИЯ РАСЧЕТА МАТЕРИАЛОВ:");

                            // Используем новый метод для демонстрации
                            int calculatedMaterial = CalculateMaterialQuantity(
                                product.idProductType,
                                1, // materialTypeId = 1 (Древесина)
                                requestProduct.Quantity,
                                0, // stockQuantity = 0 (предполагаем, что нет на складе)
                                1.5, // productParameter1
                                2.0  // productParameter2
                            );

                            if (calculatedMaterial >= 0)
                            {
                                result.AppendLine($"  Расчет материала (тестовый): {calculatedMaterial} ед.");
                            }
                            else
                            {
                                result.AppendLine($"  Расчет материала: ошибка (возвращено {calculatedMaterial})");
                            }

                            // Получаем материалы для этого продукта
                            var productMaterials = db.ProductMaterials_
                                .Where(pm => pm.idProduct == product.idProduct)
                                .ToList();

                            if (productMaterials.Any())
                            {
                                hasMaterials = true;
                                result.AppendLine("  НЕОБХОДИМЫЕ МАТЕРИАЛЫ:");

                                foreach (var pm in productMaterials)
                                {
                                    var material = db.Materials_.FirstOrDefault(m => m.idMaterials == pm.idMaterials);
                                    var materialType = material?.MaterialsType_;

                                    if (material != null && materialType != null)
                                    {
                                        // Расчет с учетом потерь
                                        double requiredQuantity = (double)(pm.MaterialQuantity * requestProduct.Quantity);
                                        double requiredWithLoss = requiredQuantity * (1 + (double)materialType.PercentageOfLossOfRawMaterials);

                                        decimal materialCost = material.UnitPriceMaterial.HasValue ?
                                            (decimal)requiredWithLoss * material.UnitPriceMaterial.Value : 0;

                                        totalMaterialCost += materialCost;

                                        result.AppendLine($"  • {material.NameMaterials} ({materialType.NameMaterialsType}):");
                                        result.AppendLine($"    Базовое количество: {requiredQuantity:N2} {material.UnitMeasurement}");
                                        result.AppendLine($"    Процент потерь: {materialType.PercentageOfLossOfRawMaterials:P2}");
                                        result.AppendLine($"    С учетом потерь: {requiredWithLoss:N2} {material.UnitMeasurement}");

                                        if (material.UnitPriceMaterial.HasValue)
                                        {
                                            result.AppendLine($"    Цена за единицу: {material.UnitPriceMaterial.Value:N2} р.");
                                            result.AppendLine($"    Стоимость материала: {materialCost:N2} р.");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                result.AppendLine("  Информация о материалах не найдена");
                            }
                        }
                    }

                    if (!hasMaterials)
                    {
                        result.AppendLine();
                        result.AppendLine("ВНИМАНИЕ: Для продукции в заявке не указаны материалы.");
                    }

                    result.AppendLine();
                    result.AppendLine(new string('=', 60));
                    result.AppendLine($"ОБЩАЯ СТОИМОСТЬ МАТЕРИАЛОВ: {totalMaterialCost:N2} р.");
                    result.AppendLine($"СТОИМОСТЬ ПРОДУКЦИИ: {request.RequestProducts_.Sum(rp => rp.Quantity * rp.UnitPrice):N2} р.");
                    result.AppendLine($"МАРЖА: {request.RequestProducts_.Sum(rp => rp.Quantity * rp.UnitPrice) - totalMaterialCost:N2} р.");

                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка расчета: {ex.Message}\n\nДетали:\n{ex.StackTrace}";
            }
        }

        /// <summary>
        /// Получить список всех типов продукции и материалов для справки
        /// </summary>
        public static string GetMaterialsAndProductsInfo()
        {
            try
            {
                using (var db = new Entities())
                {
                    var result = new StringBuilder();
                    result.AppendLine("СПРАВОЧНАЯ ИНФОРМАЦИЯ О ТИПАХ ПРОДУКЦИИ И МАТЕРИАЛОВ");
                    result.AppendLine("====================================================");

                    result.AppendLine("\nТИПЫ ПРОДУКЦИИ:");
                    result.AppendLine("---------------");
                    foreach (var pt in db.ProductType_.ToList())
                    {
                        result.AppendLine($"ID: {pt.idProductType}, Название: {pt.NameProductType}, Коэффициент: {pt.ProductTypeCoefficient}");
                    }

                    result.AppendLine("\nТИПЫ МАТЕРИАЛОВ:");
                    result.AppendLine("----------------");
                    foreach (var mt in db.MaterialsType_.ToList())
                    {
                        result.AppendLine($"ID: {mt.idMaterialsType}, Название: {mt.NameMaterialsType}, Процент потерь: {mt.PercentageOfLossOfRawMaterials:P2}");
                    }

                    result.AppendLine("\nМАТЕРИАЛЫ:");
                    result.AppendLine("----------");
                    foreach (var m in db.Materials_.Take(10).ToList()) // Берем первые 10 для примера
                    {
                        result.AppendLine($"ID: {m.idMaterials}, Название: {m.NameMaterials}, Тип: {m.MaterialsType_.NameMaterialsType}, Ед.изм: {m.UnitMeasurement}");
                    }

                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка получения справочной информации: {ex.Message}";
            }
        }
    }
}