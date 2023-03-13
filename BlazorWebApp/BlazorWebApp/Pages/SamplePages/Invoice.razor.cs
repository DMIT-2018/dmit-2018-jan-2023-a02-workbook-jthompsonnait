using ChinookSystem.ViewModel;
namespace BlazorWebApp.Pages.SamplePages
{
    public partial class Invoice
    {
        #region Fields

        private InvoiceView invoice;
        private string feedback;
        private int counter = 1;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            invoice = new InvoiceView();
            Random rnd = new Random();
            invoice.InvoiceNo = rnd.Next(1000, 2000);
        }

        private void HandleSubmit()
        {
            feedback = $"Submitted Press - {counter++}";
        }
        private void HandleValidSubmit()
        {
            feedback = $"Valid Submit - {counter++}";
        }

        private void HandleInvalidSubmit()
        {
            feedback = $"Invalid Submit - {counter++}";
        }
    }
}
