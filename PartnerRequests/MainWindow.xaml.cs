using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace PartnerRequests
{
    public partial class MainWindow : Window
    {
        private Entities db = new Entities();
        private int _selectedRequestId = -1;
        private Border _lastSelectedCard = null;

        public MainWindow()
        {
            InitializeComponent();
            LoadRequests();
        }

        private void LoadRequests()
        {
            try
            {
                var requests = db.PartnerRequests_.ToList().Select(r => new RequestViewModel
                {
                    Id = r.idRequest,
                    RequestNumber = $"З-{r.idRequest:0000}",
                    PartnerName = r.Partners.CompanyName,
                    PartnerType = r.Partners.PartnerType,
                    RequestDate = r.RequestDate,
                    Status = r.Status,
                    ProductsCount = r.RequestProducts_.Count,
                    TotalCost = r.RequestProducts_.Sum(rp => rp.Quantity * rp.UnitPrice),
                    PartnerAddress = r.Partners.Address ?? "Адрес не указан",
                    PartnerPhone = r.Partners.Phone ?? "+7 XXX XXX XX XX",
                    PartnerRating = r.Partners.Rating ?? 0
                })
                .ToList();

                RequestsPanel.ItemsSource = requests;
                _selectedRequestId = -1;
                ResetCardSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}\n\nПроверьте подключение к базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RequestCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            if (e.ClickCount == 2) // Двойной клик - редактирование
            {
                if (border.DataContext is RequestViewModel selectedRequest)
                {
                    OpenRequestForEdit(selectedRequest.Id);
                }
            }
            else // Одинарный клик - выделение
            {
                SelectRequestCard(border);
            }
        }

        private void SelectRequestCard(Border selectedCard)
        {
            ResetCardSelection();

            var style = this.FindResource("SelectedRequestCardStyle") as Style;
            if (style != null)
            {
                selectedCard.Style = style;
            }

            _lastSelectedCard = selectedCard;

            if (selectedCard.DataContext is RequestViewModel selectedRequest)
            {
                _selectedRequestId = selectedRequest.Id;
            }
        }

        private void ResetCardSelection()
        {
            if (_lastSelectedCard != null)
            {
                var defaultStyle = this.FindResource("RequestCardStyle") as Style;
                if (defaultStyle != null)
                {
                    _lastSelectedCard.Style = defaultStyle;
                }
                _lastSelectedCard = null;
            }
        }

        private void AddRequestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var requestForm = new RequestFormWindow();
                var result = requestForm.ShowDialog();
                if (result == true)
                {
                    LoadRequests();
                    MessageBox.Show("Заявка успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия формы добавления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenRequestForEdit(int requestId)
        {
            try
            {
                var requestForm = new RequestFormWindow(requestId);
                var result = requestForm.ShowDialog();
                if (result == true)
                {
                    LoadRequests();
                    MessageBox.Show("Данные заявки успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия формы редактирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRequests();
        }

        private void ViewProductsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedRequestId != -1)
                {
                    // Показываем продукцию для выбранного партнера
                    var selectedRequest = db.PartnerRequests_.FirstOrDefault(r => r.idRequest == _selectedRequestId);
                    if (selectedRequest != null)
                    {
                        var productsWindow = new ProductsWindow(selectedRequest.idPartner, selectedRequest.Partners.CompanyName);
                        productsWindow.Owner = this;
                        productsWindow.ShowDialog();
                    }
                }
                else
                {
                    // Показываем всю продукцию
                    var productsWindow = new ProductsWindow();
                    productsWindow.Owner = this;
                    productsWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия списка продукции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateMaterialsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedRequestId == -1)
                {
                    MessageBox.Show("Выберите заявку для расчета материалов.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string calculationResult = ProductionCalculator.CalculateMaterialsForRequest(_selectedRequestId);
                var resultWindow = new MaterialCalculationWindow(calculationResult);
                resultWindow.Owner = this;
                resultWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчета материалов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Справка по материалам
        private void MaterialsInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string materialsInfo = ProductionCalculator.GetMaterialsAndProductsInfo();
                var infoWindow = new MaterialCalculationWindow(materialsInfo, "Справка по материалам и типам продукции");
                infoWindow.Owner = this;
                infoWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочной информации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MaterialInputButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var materialInputWindow = new MaterialInputWindow();
                materialInputWindow.Owner = this;
                materialInputWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия формы расчета: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}