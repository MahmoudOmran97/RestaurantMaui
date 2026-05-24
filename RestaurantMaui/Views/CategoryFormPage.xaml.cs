using RestaurantMaui.ViewModels;

namespace RestaurantMaui.Views;

public partial class CategoryFormPage : ContentPage
{
    public CategoryFormPage(CategoryFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
