using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PartnerRequests
{
    public partial class RequestFormWindow : Window
    {
        private Entities db = new Entities();
        private RequestFormViewModel _request;
        private bool _isEditMode;

        public RequestFormWindow()
        {
            InitializeComponent();
            _request = new RequestFormViewModel
            {
                RequestDate = DateTime.Now,
                Status = "Новая",
                WindowTitle = "Добавление новой заявки"
            };
            _isEditMode = false;
            InitializeWindow();
        }

        public RequestFormWindow(int requestId)
        {
            InitializeComponent();
            _request = new RequestFormViewModel();
            LoadRequestForEdit(requestId);
            _isEditMode = true;
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            try
            {
                _request.Partners = db.Partners.ToList().Select(p => new PartnerViewModel
                {
                    Id = p.idPartner,
                    PartnerType = p.PartnerType,
                    CompanyName = p.CompanyName,
                    DirectorName = p.DirectorName,
                    Address = p.Address,
                    Rating = p.Rating ?? 0,
                    Phone = p.Phone,
                    Email = p.Email,
                    INN = p.INN
                }).ToList();

                _request.AllProducts = db.Products_.ToList().Select(p => new ProductViewModel
                {
                    Id = p.idProduct,
                    ProductType = p.ProductType_.NameProductType,
                    ProductName = p.ProductName,
                    Article = p.Article,
                    MinPartnerPrice = p.MinPartnerPrice
                }).ToList();

                DataContext = _request;

                if (_isEditMode)
                {
                    _request.WindowTitle = "Редактирование заявки";
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка инициализации формы: {ex.Message}");
            }
        }

        private void LoadRequestForEdit(int requestId)
        {
            try
            {
                var dbRequest = db.PartnerRequests_.FirstOrDefault(r => r.idRequest == requestId);
                if (dbRequest == null)
                {
                    MessageBox.Show("Заявка не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                _request.Id = dbRequest.idRequest;
                _request.RequestNumber = $"З-{dbRequest.idRequest:0000}";
                _request.SelectedPartnerId = dbRequest.idPartner;
                _request.RequestDate = dbRequest.RequestDate;
                _request.Status = dbRequest.Status;

                // Загружаем информацию о партнере
                var partner = db.Partners.FirstOrDefault(p => p.idPartner == dbRequest.idPartner);
                if (partner != null)
                {
                    _request.CompanyName = partner.CompanyName;
                    _request.DirectorName = partner.DirectorName;
                    _request.Address = partner.Address;
                    _request.Rating = partner.Rating ?? 0;
                    _request.Phone = partner.Phone;
                    _request.Email = partner.Email;
                }

                // Загружаем продукты заявки
                foreach (var rp in dbRequest.RequestProducts_)
                {
                    var product = db.Products_.FirstOrDefault(p => p.idProduct == rp.idProduct);
                    if (product != null)
                    {
                        _request.Products.Add(new RequestProductViewModel
                        {
                            Id = rp.idRequestProduct,
                            ProductId = rp.idProduct,
                            ProductName = product.ProductName,
                            ProductType = product.ProductType_.NameProductType,
                            Article = product.Article,
                            Quantity = rp.Quantity,
                            UnitPrice = rp.UnitPrice
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки данных заявки: {ex.Message}");
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите продукцию для добавления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество (целое положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedProductId = (int)ProductComboBox.SelectedValue;
                var product = _request.AllProducts.FirstOrDefault(p => p.Id == selectedProductId);

                if (product != null)
                {
                    // Проверяем, нет ли уже этого продукта в заявке
                    var existingProduct = _request.Products.FirstOrDefault(p => p.ProductId == selectedProductId);
                    if (existingProduct != null)
                    {
                        existingProduct.Quantity += quantity;
                    }
                    else
                    {
                        _request.Products.Add(new RequestProductViewModel
                        {
                            ProductId = product.Id,
                            ProductName = product.ProductName,
                            ProductType = product.ProductType,
                            Article = product.Article,
                            Quantity = quantity,
                            UnitPrice = product.MinPartnerPrice
                        });
                    }

                    // Обновляем DataGrid
                    ProductsDataGrid.Items.Refresh();
                    OnPropertyChanged(nameof(_request.TotalCost));

                    // Сбрасываем выбор
                    ProductComboBox.SelectedIndex = -1;
                    QuantityTextBox.Text = "1";
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка добавления продукции: {ex.Message}");
            }
        }

        private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var product = button?.DataContext as RequestProductViewModel;

                if (product != null)
                {
                    var result = MessageBox.Show($"Удалить продукт '{product.ProductName}' из заявки?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _request.Products.Remove(product);
                        ProductsDataGrid.Items.Refresh();
                        OnPropertyChanged(nameof(_request.TotalCost));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка удаления продукции: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            var errors = new List<string>();

            if (_request.SelectedPartnerId == 0)
                errors.Add("• Необходимо выбрать партнера");

            if (_request.Products.Count == 0)
                errors.Add("• Необходимо добавить хотя бы одну позицию продукции");

            // Проверка рейтинга партнера
            if (_request.Rating < 0)
                errors.Add("• Рейтинг партнера должен быть неотрицательным числом");

            // Проверка стоимости продукции
            foreach (var product in _request.Products)
            {
                if (product.UnitPrice < 0)
                {
                    errors.Add($"• Стоимость продукции '{product.ProductName}' не может быть отрицательной");
                }
                if (product.Quantity <= 0)
                {
                    errors.Add($"• Количество продукции '{product.ProductName}' должно быть положительным числом");
                }
            }

            if (errors.Any())
            {
                ShowError(string.Join("\n", errors));
                return false;
            }

            HideError();
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                PartnerRequests_ request;

                if (_isEditMode)
                {
                    // Редактирование существующей заявки
                    request = db.PartnerRequests_.FirstOrDefault(r => r.idRequest == _request.Id);
                    if (request == null)
                    {
                        MessageBox.Show("Заявка не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Удаляем старые продукты
                    var oldProducts = db.RequestProducts_.Where(rp => rp.idRequest == _request.Id).ToList();
                    db.RequestProducts_.RemoveRange(oldProducts);
                }
                else
                {
                    // Добавление новой заявки
                    request = new PartnerRequests_();
                    db.PartnerRequests_.Add(request);
                }

                // Обновление данных заявки
                request.idPartner = _request.SelectedPartnerId;
                request.RequestDate = _request.RequestDate;
                request.Status = _request.Status;

                // Добавляем продукты
                foreach (var productVm in _request.Products)
                {
                    var requestProduct = new RequestProducts_
                    {
                        idProduct = productVm.ProductId,
                        Quantity = productVm.Quantity,
                        UnitPrice = productVm.UnitPrice
                    };
                    request.RequestProducts_.Add(requestProduct);
                }

                db.SaveChanges();

                MessageBox.Show($"Заявка успешно {(_isEditMode ? "отредактирована" : "добавлена")}.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка сохранения заявки: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_request.Products.Count > 0)
            {
                var result = MessageBox.Show("Все несохраненные изменения будут потеряны. Продолжить?", "Подтверждение отмены", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            this.DialogResult = false;
            this.Close();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;
        }

        private void OnPropertyChanged(string propertyName)
        {
            // Обновление привязок
            var binding = ProductsDataGrid.GetBindingExpression(DataGrid.ItemsSourceProperty);
            binding?.UpdateTarget();
        }

        private void PartnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartnerComboBox.SelectedValue != null)
            {
                var partnerId = (int)PartnerComboBox.SelectedValue;
                var partner = _request.Partners.FirstOrDefault(p => p.Id == partnerId);

                if (partner != null)
                {
                    _request.CompanyName = partner.CompanyName;
                    _request.DirectorName = partner.DirectorName;
                    _request.Address = partner.Address;
                    _request.Rating = partner.Rating;
                    _request.Phone = partner.Phone;
                    _request.Email = partner.Email;

                    // Обновляем привязки
                    OnPropertyChanged(nameof(_request.CompanyName));
                    OnPropertyChanged(nameof(_request.DirectorName));
                    OnPropertyChanged(nameof(_request.Address));
                    OnPropertyChanged(nameof(_request.Rating));
                    OnPropertyChanged(nameof(_request.Phone));
                    OnPropertyChanged(nameof(_request.Email));
                }
            }
        }

        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !char.IsDigit(e.Text, 0);
        }
    }
}