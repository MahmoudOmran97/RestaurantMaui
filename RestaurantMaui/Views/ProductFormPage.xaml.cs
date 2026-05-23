using RestaurantMaui.ViewModels;

namespace RestaurantMaui.Views;

public partial class ProductFormPage : ContentPage
{
    public ProductFormPage(ProductFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}