namespace PlaylistManagementSystem.ViewModels
{
	public class PlaylistTrackView
	{
		public int TrackId { get; set; }
		public int TrackNumber { get; set; }
		public string SongName { get; set; }
		public int Milliseconds	{get; set;}
		public bool Remove { get; set; }

        public string Length
        {
            get
            {
                return $"{(int)Milliseconds / 1000 / 60}:{Milliseconds / 1000 % 60}";
            }
        }
		public int NewTrackNumber { get; set; }
	}
}