using RestaurantMaui.ViewModels;

namespace RestaurantMaui.Views;

public partial class MenuPage : ContentPage
{
    private readonly MenuViewModel _vm;

    public MenuPage(MenuViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private void OnCategoryClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
            _vm.SelectedCategory = btn.Text;
    }
}