using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PartnerRequests
{
    public partial class MaterialInputWindow : Window
    {
        private Entities db = new Entities();
        private MaterialCalculationViewModel _viewModel;

        public MaterialInputWindow()
        {
            InitializeComponent();
            _viewModel = new MaterialCalculationViewModel();
            DataContext = _viewModel;
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загружаем типы продукции
                var productTypes = db.ProductType_.ToList();
                ProductTypeComboBox.ItemsSource = productTypes;
                if (productTypes.Any())
                    ProductTypeComboBox.SelectedIndex = 0;

                // Загружаем типы материалов
                var materialTypes = db.MaterialsType_.ToList();
                MaterialTypeComboBox.ItemsSource = materialTypes;
                if (materialTypes.Any())
                    MaterialTypeComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация введенных данных
                if (!ValidateInput())
                    return;

                // Выполняем расчет через метод ViewModel
                _viewModel.CalculateManual();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            // Проверка выбора типа продукции
            if (_viewModel.ProductTypeId == 0)
            {
                MessageBox.Show("Выберите тип продукции.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка выбора типа материала
            if (_viewModel.MaterialTypeId == 0)
            {
                MessageBox.Show("Выберите тип материала.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка требуемого количества
            if (_viewModel.RequiredQuantity <= 0)
            {
                MessageBox.Show("Требуемое количество должно быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка количества на складе
            if (_viewModel.StockQuantity < 0)
            {
                MessageBox.Show("Количество на складе не может быть отрицательным.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка параметров продукции
            if (_viewModel.ProductParameter1 <= 0 || _viewModel.ProductParameter2 <= 0)
            {
                MessageBox.Show("Параметры продукции должны быть положительными числами.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearManualInput();

            if (ProductTypeComboBox.Items.Count > 0)
                ProductTypeComboBox.SelectedIndex = 0;
            if (MaterialTypeComboBox.Items.Count > 0)
                MaterialTypeComboBox.SelectedIndex = 0;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Разрешаем только цифры и точку для вещественных чисел
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}