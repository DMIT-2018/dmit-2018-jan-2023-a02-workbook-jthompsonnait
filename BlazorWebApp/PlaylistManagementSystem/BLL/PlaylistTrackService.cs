#nullable disable
using PlaylistManagementSystem.ViewModels;
using PlaylistManagementSystem.DAL;
using Microsoft.EntityFrameworkCore;

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
        public List<TrackSelectionView> FetchArtistOrAlbumTracks(
            string searchType, string searchValue)
        {
            return null;
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
