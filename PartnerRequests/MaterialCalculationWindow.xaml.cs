using System.Windows;

namespace PartnerRequests
{
    public partial class MaterialCalculationWindow : Window
    {
        public MaterialCalculationWindow(string calculationResult)
        {
            InitializeComponent();
            CalculationResultText.Text = calculationResult;
        }

        // Новый конструктор для установки пользовательского заголовка
        public MaterialCalculationWindow(string calculationResult, string windowTitle)
        {
            InitializeComponent();
            CalculationResultText.Text = calculationResult;
            this.Title = windowTitle;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}