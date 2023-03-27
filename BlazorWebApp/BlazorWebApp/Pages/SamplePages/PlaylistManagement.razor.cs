#nullable disable
using Microsoft.AspNetCore.Components;
using PlaylistManagementSystem.BLL;
using PlaylistManagementSystem.Paginator;
using PlaylistManagementSystem.ViewModels;

namespace BlazorWebApp.Pages.SamplePages
{
    public partial class PlaylistManagement
    {
        //  we are now injecting our service into our class using the [Inject] attribute
        [Inject]
        protected PlaylistTrackService? PlaylistTrackService { get; set; }

        #region Fields
        // search pattern
        private string searchPattern { get; set; } = "Deep";
        // search type
        private string searchType { get; set; } = "Artist";
        //  play list name
        private string playlistName { get; set; } = "HansenB1";
        //  playlist ID (would be return from database but hard coding it)
        private int playlistId { get; set; } = 13;
        //  user name
        private string userName { get; set; } = "HansenB";
        //  feed back messages
        private string feedBack { get; set; }
        #endregion

        protected List<PlaylistTrackView> Playlists { get; set; } = new();

        #region    Paginator
        //  desired current page size
        private const int PAGE_SIZE = 10;

        //  sort colum used with the paginator
        protected string SortField { get; set; } = "Owner";

        // sort direction for paginator
        protected string Direction { get; set; } = "desc";
        //  current page for painator
        protected int CurrentPage { get; set; } = 1;

        //paginator collection of track selection view
        protected PagedResult<TrackSelectionView> PaginatorTrackSelection { get; set; } = new();
        #endregion

        #region Methods
        //  sort method
        private async void Sort(string column)
        {
            Direction = SortField == column ? Direction == "asc" ? "desc" : "asc" : "asc";
            SortField = column;
            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                await FetchArtistOrAlbumTracks();
            }
        }

        private async Task FetchArtistOrAlbumTracks()
        {
            //  we would normal check if the user has enter ina value int the search
            //      pattern, but we will let the service do the error checking
            PaginatorTrackSelection = await PlaylistTrackService.FetchArtistOrAlbumTracks(
                searchType, searchPattern, CurrentPage, PAGE_SIZE, SortField, Direction);
            //  Blazor would not recognize the state change and not refresh the UI
            await InvokeAsync(StateHasChanged)


        }
        #endregion
    }
}
