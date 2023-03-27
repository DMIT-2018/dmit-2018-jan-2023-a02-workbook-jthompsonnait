#nullable disable
using PlaylistManagementSystem.ViewModels;
using PlaylistManagementSystem.DAL;
using Microsoft.EntityFrameworkCore;
using PlaylistManagementSystem.Paginator;

namespace PlaylistManagementSystem.BLL
{


    public class PlaylistTrackService
    {
        #region Fields

        private readonly PlaylistManagementContext _playlistManagementContext;
        #endregion

        internal PlaylistTrackService(PlaylistManagementContext playlistManagementContext)
        {
            _playlistManagementContext = playlistManagementContext;
        }

        public WorkingVersionView GetWorkingVersion()
        {
            return _playlistManagementContext.WorkingVersions
                .Select(x => new WorkingVersionView
                {
                    VersionId = x.VersionId,
                    Major = x.Major,
                    Minor = x.Minor,
                    Build = x.Build,
                    Revision = x.Revision,
                    AsOfDate = x.AsOfDate,
                    Comments = x.Comments

                }).FirstOrDefault();
        }

        //public async Task<WorkingVersionView> GetWorkingVersion()
        //{
        //    return await _playlistManagementContext.WorkingVersions
        //        .Select(x => new WorkingVersionView
        //        {
        //            VersionId = x.VersionId,
        //            Major = x.Major,
        //            Minor = x.Minor,
        //            Build = x.Build,
        //            Revision = x.Revision,
        //            AsOfDate = x.AsOfDate,
        //            Comments = x.Comments

        //        }).FirstOrDefaultAsync();
        //}

        // fetch playlist
        public List<PlaylistTrackView> FetchPlaylist(
            string userName, string playlistName)
        {
            return null;
        }
        //  fetch artist or album tracks
        public Task<PagedResult<TrackSelectionView>> FetchArtistOrAlbumTracks(
            string searchType, string searchValue, int page, int pageSize, string sortColumn,
            string sortDirection)
        {
            //  Business Rules
            //  These are processing rules that need to be satisfied for valid data
            //      rule:   search value cannot be empty
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                throw new ArgumentNullException("searchValue value is missing");
            }
            //  Task.FromResult() creates a finished Task that holds a value in its
            //      Result property
            return Task.FromResult(_playlistManagementContext.Tracks
                .Where(x => searchType == "Artist"
                ? x.Album.Artist.Name.Contains(searchValue)
                : x.Album.Title.Contains(searchValue))
                .Select(x => new TrackSelectionView
                    {
                        TrackId = x.TrackId,
                        SongName = x.Name,
                        AlbumTitle = x.Album.Title,
                        ArtistName = x.Album.Artist.Name,
                        Milliseconds = x.Milliseconds,
                        Price = x.UnitPrice
                    }).AsQueryable()
                .OrderBy(sortColumn, sortDirection)// custom sort extension to sort on a string representing a column
                .ToPagedResult(page, pageSize));
        }

        //  add track
        public void AddTrack(string userName,
            string playlistName, int trackId)
        {

        }

        // remove track(s)
        public void RemoveTracks(int playlistId, List<int> trackIds)
        {

        }

        //  move tracks
        public void MoveTrack(int playlistId,
            List<MoveTrackView> moveTracks)
        {

        }
    }
}
