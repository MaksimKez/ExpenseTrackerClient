using System.Windows;

namespace ExpenseTrackerClient;

public partial class CategorySelectionWindow : System.Windows.Window
{
    public string SelectedCategory { get; private set; }
    public CategorySelectionWindow(bool isIncome)
    {
        InitializeComponent();
        LoadCategories(isIncome);
    }
    private void LoadCategories(bool isIncome)
    {
        CategoryListBox.Items.Clear();
        if (isIncome)
        {
            // Добавление категорий доходов
            CategoryListBox.Items.Add("Зарплата");
            CategoryListBox.Items.Add("Подарок");
            CategoryListBox.Items.Add("Дивиденды");
        }
        else
        {
            // Добавление категорий расходов
            CategoryListBox.Items.Add("Кафе");
            CategoryListBox.Items.Add("Продукты");
            CategoryListBox.Items.Add("Дом");
            CategoryListBox.Items.Add("Транспорт");
            CategoryListBox.Items.Add("Медицина");
            CategoryListBox.Items.Add("Развлечения");
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (CategoryListBox.SelectedItem != null)
        {
            SelectedCategory = CategoryListBox.SelectedItem.ToString();
            this.DialogResult = true;
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите категорию.");
        }
    }
}
