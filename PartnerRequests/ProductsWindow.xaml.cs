using System.Linq;
using System.Windows;

namespace PartnerRequests
{
    public partial class ProductsWindow : Window
    {
        private Entities db = new Entities();
        private int _partnerId;
        private string _partnerName;

        // Конструктор для просмотра всей продукции
        public ProductsWindow()
        {
            InitializeComponent();
            _partnerId = 0;
            _partnerName = "";
            LoadAllProducts();
        }

        // Конструктор для просмотра продукции конкретного партнера
        public ProductsWindow(int partnerId, string partnerName = "")
        {
            InitializeComponent();
            _partnerId = partnerId;
            _partnerName = partnerName;
            LoadPartnerProducts();
        }

        private void LoadAllProducts()
        {
            try
            {
                Title = "Каталог всей продукции";
                TitleTextBlock.Text = "Каталог всей продукции";

                var products = db.Products_.ToList().Select(p => new ProductViewModel
                {
                    Id = p.idProduct,
                    ProductType = p.ProductType_.NameProductType,
                    ProductName = p.ProductName,
                    Article = p.Article,
                    MinPartnerPrice = p.MinPartnerPrice,
                    ProductTypeId = p.idProductType,
                    ProductTypeCoefficient = p.ProductType_.ProductTypeCoefficient
                }).ToList();

                ProductsDataGrid.ItemsSource = products;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продукции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPartnerProducts()
        {
            try
            {
                if (!string.IsNullOrEmpty(_partnerName))
                {
                    Title = $"Продукция для партнера: {_partnerName}";
                    TitleTextBlock.Text = $"Рекомендуемая продукция для: {_partnerName}";
                }
                else
                {
                    var part = db.Partners.FirstOrDefault(p => p.idPartner == _partnerId);
                    if (part != null)
                    {
                        Title = $"Продукция для партнера: {part.CompanyName}";
                        TitleTextBlock.Text = $"Рекомендуемая продукция для: {part.CompanyName}";
                    }
                }

                var allProducts = db.Products_.ToList();
                var partner = db.Partners.FirstOrDefault(p => p.idPartner == _partnerId);
                var filteredProducts = allProducts;

                if (partner != null && partner.Rating.HasValue)
                {
                    if (partner.Rating >= 8)
                    {
                        // Партнеры с высоким рейтингом - премиум продукция
                        filteredProducts = allProducts.Where(p => p.idProductType == 1 || p.idProductType == 5).ToList();
                    }
                    else if (partner.Rating >= 5)
                    {
                        // Партнеры со средним рейтингом - стандартная продукция
                        filteredProducts = allProducts.Where(p => p.idProductType != 1).ToList();
                    }
                }

                var products = filteredProducts.Select(p => new ProductViewModel
                {
                    Id = p.idProduct,
                    ProductType = p.ProductType_.NameProductType,
                    ProductName = p.ProductName,
                    Article = p.Article,
                    MinPartnerPrice = p.MinPartnerPrice,
                    ProductTypeId = p.idProductType,
                    ProductTypeCoefficient = p.ProductType_.ProductTypeCoefficient
                }).ToList();

                ProductsDataGrid.ItemsSource = products;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продукции партнера: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaterialsInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string materialsInfo = ProductionCalculator.GetMaterialsAndProductsInfo();
                var infoWindow = new MaterialCalculationWindow(materialsInfo);
                infoWindow.Title = "Справка по материалам и типам продукции";
                infoWindow.Owner = this;
                infoWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочной информации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}