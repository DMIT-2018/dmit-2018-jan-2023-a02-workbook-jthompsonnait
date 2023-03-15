using System.Security.AccessControl;
using ChinookSystem.ViewModel;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWebApp.Pages.SamplePages
{
    public partial class AdvancedInvoice
    {
        enum PaymentTypes
        {
            Unknown,
            Cash,
            Chq,
            CreditCards
        }
        #region Fields
        private InvoiceView invoice;
        private string feedback;
        private int counter = 1;

        //  Holds metadata related to a data editing process,
        //      such as flags to indicate which fields have been modified
        //      and the current set of validation messages
        private EditContext? editContext;

        //  Used to store the validation messages
        private ValidationMessageStore? messageStore;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            invoice = new InvoiceView();
            Random rnd = new Random();
            invoice.InvoiceNo = rnd.Next(1000, 2000);

            //  edit context nees to be setup after data has been initialized
            //  setup will used the invoice for it property
            editContext = new EditContext(invoice);

            // set the validation to use the HandleValidationRequested event
            editContext.OnValidationRequested += HandleValidationRequested;

            //  setup the message store to track any validation messages
            messageStore = new ValidationMessageStore(editContext);
        }

        //  Handles the validation requested.  This allows for custom validation outside of using the 
        //      DataAnnotationsValidators
        private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs args)
        {
            //  clear the message store if there is any existing validation errors
            messageStore?.Clear();

            //  custom validation logic
            //  payment type cannot be set to "Unknown"
            if ( invoice.PaymentType == "Unknown")
            {
                messageStore?.Add(() => invoice.PaymentType, "Payment Type cannot be set to Unknown");
            }
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

