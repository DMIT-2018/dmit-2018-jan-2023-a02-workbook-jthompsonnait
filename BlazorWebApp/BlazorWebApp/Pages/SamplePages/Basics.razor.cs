using System.Security.AccessControl;
using Microsoft.AspNetCore.Components;
using PlaylistManagementSystem.BLL;
using PlaylistManagementSystem.ViewModels;

namespace BlazorWebApp.Pages.SamplePages
{
    public partial class Basics
    {
        #region Injections
        //  We are now injecting our service into our class using the [Inject] attribute.
        //  Before, we would have used the page constructor to add it.
        //  public Basic(PlaylistTrackService playlistTrackService
        //  {
        //      _playlistTrackService = playlistTrackService;
        //  }
        [Inject]
        protected PlaylistTrackService? PlaylistTrackService { get; set; }
        #endregion

        #region Fields
        private string myName = string.Empty;
        private int oddEven;
        //  working version view
        private WorkingVersionView workingVersion = new();
        #endregion

        //  Methods invoked when the component is ready to start,
        //  having received it initial parameters from it parent in the render
        //      tree
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();


        }

        private void RandomValue()
        {
            Random rnd = new Random();
            oddEven = rnd.Next(0, 25);

            if (oddEven % 2 == 0)
            {
                myName = $"James is even {oddEven}";
            }
            else
            {
                myName = null;
            }
        }

        //  method for retrieving our version information
        private async Task GetDatabase()
        {
            workingVersion = PlaylistTrackService.GetWorkingVersion();
            //  wait for the data to be retrieved before we update the page
            await InvokeAsync(StateHasChanged);
            // workingVersion = await PlaylistTrackService.GetWorkingVersion();
        }

    }
}
