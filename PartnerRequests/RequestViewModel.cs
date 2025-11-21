using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PartnerRequests
{
    public class RequestViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string PartnerName { get; set; }
        public string PartnerType { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public int ProductsCount { get; set; }
        public decimal TotalCost { get; set; }
        public string PartnerAddress { get; set; }
        public string PartnerPhone { get; set; }
        public int PartnerRating { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RequestFormViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string WindowTitle { get; set; }

        private int _selectedPartnerId;
        public int SelectedPartnerId
        {
            get => _selectedPartnerId;
            set
            {
                _selectedPartnerId = value;
                OnPropertyChanged(nameof(SelectedPartnerId));
            }
        }

        private string _companyName;
        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged(nameof(CompanyName));
            }
        }

        private string _directorName;
        public string DirectorName
        {
            get => _directorName;
            set
            {
                _directorName = value;
                OnPropertyChanged(nameof(DirectorName));
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private int _rating;
        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public List<PartnerViewModel> Partners { get; set; } = new List<PartnerViewModel>();
        public List<ProductViewModel> AllProducts { get; set; } = new List<ProductViewModel>();
        public List<RequestProductViewModel> Products { get; set; } = new List<RequestProductViewModel>();

        public decimal TotalCost
        {
            get
            {
                decimal total = 0;
                foreach (var product in Products)
                {
                    total += product.TotalPrice;
                }
                return total;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RequestProductViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string Article { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public decimal TotalPrice => Quantity * UnitPrice;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PartnerViewModel
    {
        public int Id { get; set; }
        public string PartnerType { get; set; }
        public string CompanyName { get; set; }
        public string DirectorName { get; set; }
        public string Address { get; set; }
        public int Rating { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string INN { get; set; }
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string Article { get; set; }
        public decimal MinPartnerPrice { get; set; }
        public int ProductTypeId { get; set; }
        public decimal ProductTypeCoefficient { get; set; }
    }

    public class MaterialRequirementViewModel : INotifyPropertyChanged
    {
        public int RequestId { get; set; }
        public string ProductName { get; set; }
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public decimal PercentageOfLoss { get; set; }
        public decimal MaterialQuantity { get; set; }
        public int ProductionQuantity { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal RequiredQuantityWithLoss { get; set; }
        public string UnitMeasurement { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MaterialCalculationViewModel : INotifyPropertyChanged
    {
        private string _calculationResult;
        public string CalculationResult
        {
            get => _calculationResult;
            set
            {
                _calculationResult = value;
                OnPropertyChanged(nameof(CalculationResult));
            }
        }

        private string _windowTitle;
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        // Добавляем свойства для ручного ввода
        private int _productTypeId = 1;
        public int ProductTypeId
        {
            get => _productTypeId;
            set
            {
                _productTypeId = value;
                OnPropertyChanged(nameof(ProductTypeId));
            }
        }

        private int _materialTypeId = 1;
        public int MaterialTypeId
        {
            get => _materialTypeId;
            set
            {
                _materialTypeId = value;
                OnPropertyChanged(nameof(MaterialTypeId));
            }
        }

        private int _requiredQuantity = 100;
        public int RequiredQuantity
        {
            get => _requiredQuantity;
            set
            {
                _requiredQuantity = value;
                OnPropertyChanged(nameof(RequiredQuantity));
            }
        }

        private int _stockQuantity = 0;
        public int StockQuantity
        {
            get => _stockQuantity;
            set
            {
                _stockQuantity = value;
                OnPropertyChanged(nameof(StockQuantity));
            }
        }

        private double _productParameter1 = 1.0;
        public double ProductParameter1
        {
            get => _productParameter1;
            set
            {
                _productParameter1 = value;
                OnPropertyChanged(nameof(ProductParameter1));
            }
        }

        private double _productParameter2 = 1.0;
        public double ProductParameter2
        {
            get => _productParameter2;
            set
            {
                _productParameter2 = value;
                OnPropertyChanged(nameof(ProductParameter2));
            }
        }

        private int _manualCalculationResult;
        public int ManualCalculationResult
        {
            get => _manualCalculationResult;
            set
            {
                _manualCalculationResult = value;
                OnPropertyChanged(nameof(ManualCalculationResult));
            }
        }

        private string _manualCalculationStatus = "Введите данные";
        public string ManualCalculationStatus
        {
            get => _manualCalculationStatus;
            set
            {
                _manualCalculationStatus = value;
                OnPropertyChanged(nameof(ManualCalculationStatus));
            }
        }

        // Метод для ручного расчета
        public void CalculateManual()
        {
            try
            {
                ManualCalculationResult = ProductionCalculator.CalculateMaterialQuantity(
                    ProductTypeId,
                    MaterialTypeId,
                    RequiredQuantity,
                    StockQuantity,
                    ProductParameter1,
                    ProductParameter2
                );

                if (ManualCalculationResult == -1)
                {
                    ManualCalculationStatus = "Ошибка: неверные данные или типы не найдены";
                }
                else if (ManualCalculationResult == 0)
                {
                    ManualCalculationStatus = "Вся продукция уже на складе";
                }
                else
                {
                    ManualCalculationStatus = "Расчет выполнен успешно";
                }
            }
            catch (Exception ex)
            {
                ManualCalculationStatus = $"Ошибка расчета: {ex.Message}";
                ManualCalculationResult = -1;
            }
        }

        // Метод для очистки полей ручного ввода
        public void ClearManualInput()
        {
            ProductTypeId = 1;
            MaterialTypeId = 1;
            RequiredQuantity = 100;
            StockQuantity = 0;
            ProductParameter1 = 1.0;
            ProductParameter2 = 1.0;
            ManualCalculationResult = 0;
            ManualCalculationStatus = "Введите данные";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TestCalculationViewModel : INotifyPropertyChanged
    {
        private int _productTypeId = 1;
        public int ProductTypeId
        {
            get => _productTypeId;
            set
            {
                _productTypeId = value;
                OnPropertyChanged(nameof(ProductTypeId));
            }
        }

        private int _materialTypeId = 1;
        public int MaterialTypeId
        {
            get => _materialTypeId;
            set
            {
                _materialTypeId = value;
                OnPropertyChanged(nameof(MaterialTypeId));
            }
        }

        private int _requiredQuantity = 100;
        public int RequiredQuantity
        {
            get => _requiredQuantity;
            set
            {
                _requiredQuantity = value;
                OnPropertyChanged(nameof(RequiredQuantity));
            }
        }

        private int _stockQuantity = 20;
        public int StockQuantity
        {
            get => _stockQuantity;
            set
            {
                _stockQuantity = value;
                OnPropertyChanged(nameof(StockQuantity));
            }
        }

        private double _productParameter1 = 2.5;
        public double ProductParameter1
        {
            get => _productParameter1;
            set
            {
                _productParameter1 = value;
                OnPropertyChanged(nameof(ProductParameter1));
            }
        }

        private double _productParameter2 = 1.5;
        public double ProductParameter2
        {
            get => _productParameter2;
            set
            {
                _productParameter2 = value;
                OnPropertyChanged(nameof(ProductParameter2));
            }
        }

        private int _calculationResult;
        public int CalculationResult
        {
            get => _calculationResult;
            set
            {
                _calculationResult = value;
                OnPropertyChanged(nameof(CalculationResult));
            }
        }

        public void Calculate()
        {
            CalculationResult = ProductionCalculator.CalculateMaterialQuantity(
                ProductTypeId,
                MaterialTypeId,
                RequiredQuantity,
                StockQuantity,
                ProductParameter1,
                ProductParameter2
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProductMaterialsViewModel : INotifyPropertyChanged
    {
        public int ProductMaterialId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public string UnitMeasurement { get; set; }
        public decimal MaterialQuantity { get; set; }
        public decimal? UnitPriceMaterial { get; set; }
        public decimal? QuantityInStock { get; set; }
        public decimal TotalMaterialCost => MaterialQuantity * (UnitPriceMaterial ?? 0);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PartnerProductsViewModel : INotifyPropertyChanged
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public int Rating { get; set; }
        public string PartnerType { get; set; }
        public List<ProductViewModel> RecommendedProducts { get; set; } = new List<ProductViewModel>();
        public List<ProductViewModel> AllProducts { get; set; } = new List<ProductViewModel>();

        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged(nameof(FilterText));
                FilterProducts();
            }
        }

        private void FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                RecommendedProducts = new List<ProductViewModel>(AllProducts);
            }
            else
            {
                RecommendedProducts = AllProducts
                    .Where(p => p.ProductName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                               p.Article.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                               p.ProductType.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }
            OnPropertyChanged(nameof(RecommendedProducts));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MaterialStockViewModel : INotifyPropertyChanged
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public string UnitMeasurement { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal QuantityPerPackage { get; set; }
        public bool IsLowStock => QuantityInStock < MinimumQuantity;
        public string StockStatus => IsLowStock ? "НИЗКИЙ ЗАПАС" : "НОРМА";
        public decimal StockValue => QuantityInStock * UnitPrice;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProductionPlanViewModel : INotifyPropertyChanged
    {
        public int RequestId { get; set; }
        public string PartnerName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalCost { get; set; }
        public List<MaterialRequirementViewModel> MaterialRequirements { get; set; } = new List<MaterialRequirementViewModel>();
        public decimal TotalMaterialCost => MaterialRequirements.Sum(m => m.TotalCost);
        public decimal ProfitMargin => TotalCost - TotalMaterialCost;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}