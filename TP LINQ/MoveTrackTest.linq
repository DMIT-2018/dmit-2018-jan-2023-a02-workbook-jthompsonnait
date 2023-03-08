<Query Kind="Program">
  <Connection>
    <ID>6908ebe3-e58f-4ab6-b1c6-6d010450e634</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.</Server>
    <Database>ChinookSept2018</Database>
    <DisplayName>ChinookEntity</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
      <EFVersion>6.0.10</EFVersion>
      <TrustServerCertificate>True</TrustServerCertificate>
    </DriverData>
  </Connection>
</Query>

#load ".\ViewModels\*.cs"
using Chinook;
void Main()
{
	try
	{
		//  This is the DRIVER area.
		//	coded and test the MoveTrack
		//		The command method will receive a playlistId and a list of MoveTrackView

		//	793	A Castle Full of Rascals
		//	822	A Twist In The Tail
		//	543	Burn
		//	756	Child in Time

		string userName = "HansenB";
		string playlistName = "Jan23A02";
		int playlistId = Playlists
							.Where(x => x.UserName == userName
									&& x.Name == playlistName)
							.Select(x => x.PlaylistId)
							.FirstOrDefault();
		if (playlistId == 0)
		{
			throw new ArgumentNullException($"No playlist exist for {playlistName}");
		}

		List<MoveTrackView> moveTracks = new List<MoveTrackView>();
		moveTracks.Add(new MoveTrackView() { TrackId = 822, TrackNumber = 1 });
		moveTracks.Add(new MoveTrackView() { TrackId = 756, TrackNumber = 2 });
		moveTracks.Add(new MoveTrackView() { TrackId = 793, TrackNumber = 3 });
		moveTracks.Add(new MoveTrackView() { TrackId = 543, TrackNumber = 4 });

		//	showing that both the playlist and track does not exist
		Console.WriteLine("Before moving track");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName).Dump();
		PlaylistTrackServices_MoveTrack(playlistId, moveTracks);

		//	showing that both the playlist and track now exist
		Console.WriteLine("After moving track");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName).Dump();
	}
	#region catch all exceptions
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}

	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}

	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
}

#region Methods
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}
#endregion

public void PlaylistTrackServices_MoveTrack(int playlistId, List<MoveTrackView> moveTracks)
{
	// local variables
	//	hold the playlist that we are working with
	List<PlaylistTracks> scratchPadPlaylistTracks = null;
	int tracknumber = 0;

	//	we need a containter to hold x number of Exception messages
	List<Exception> errorlist = new List<System.Exception>();

	if (playlistId == 0)
	{
		throw new ArgumentNullException("No playlist ID was provided");
	}

	if (moveTracks.Count() == 0)
	{
		throw new ArgumentNullException("No tracks were provided to be move");
	}

	//	check that we have items to move
	var count = moveTracks
					.Where(x => x.TrackNumber > 0)
					.Count();

	if (count == 0)
	{
		throw new ArgumentNullException("No list of tracks were submitted with track number greater than zero");
	}

	//  checking for track numbers being less than zero
	count = moveTracks
			   .Where(x => x.TrackNumber < 0)
			   .Count();
	if (count > 0)
	{
		throw new ArgumentNullException("There are track number with a values less than zero");
	}

	//	check to see if a track number has been submitted more than once
	List<MoveTrackView> repeatTracks = moveTracks
										.Where(x => x.TrackNumber > 0)
										.GroupBy(x => x.TrackNumber)
										.Where(gb => gb.Count() > 1)
										.Select(gb => new MoveTrackView
										{
											TrackId = 0,
											TrackNumber = gb.Key
										}).ToList();
	foreach (var rt in repeatTracks)
	{
		errorlist.Add(new Exception($"Track number {rt.TrackNumber} is used more than once"));
	}

	scratchPadPlaylistTracks = PlaylistTracks
								.Where(x => x.PlaylistId == playlistId)
								.OrderBy(x => x.TrackNumber)
								.Select(x => x).ToList();


	//	reset all of our track numbers to zero
	foreach (var playlistTrack in scratchPadPlaylistTracks)
	{
		playlistTrack.TrackNumber = 0;
	}

	//	update the playlist track number with move track numbers
	foreach (var moveTrack in moveTracks)
	{
		PlaylistTracks playlistTrack = scratchPadPlaylistTracks
										.Where(x => x.TrackId == moveTrack.TrackId)
										.Select(x => x).FirstOrDefault();
		//	check to see if the playlist track exist in the PlaylistTracks
		if (playlistTrack == null)
		{
			var songName = Tracks
							.Where(x => x.TrackId == moveTrack.TrackId)
							.Select(x => x.Name)
							.FirstOrDefault();
			errorlist.Add(new Exception($"The track ({songName}) cannot be found in the playlist"));
		}
		else
		{
			playlistTrack.TrackNumber = moveTrack.TrackNumber;
		}
	}

	if (errorlist.Count() == 0)
	{
		foreach (var playlistTrack in scratchPadPlaylistTracks)
		{
			bool wasFound = true;
			//  only want to process those track number that are empty (zero)
			if (playlistTrack.TrackNumber == 0)
			{
				while (wasFound)
				{
					//	we want to increment the track number and process until
					//		the calue is not found in the scratch pad playlist
					tracknumber++;
					wasFound = scratchPadPlaylistTracks
								.Where(x => x.TrackNumber == tracknumber)
								.Select(x => x)
								.Any();
				}
				playlistTrack.TrackNumber = tracknumber;
			}
		}
	}
	
	if(errorlist.Count() > 0)
	{
		throw new AggregateException("Unable to remove request tracks.  Check concerns", errorlist);
	}
	else
	{
		//	all work has been staged
		SaveChanges();
	}
}

public List<PlaylistTrackView> PlaylistTrackServices_FetchPlaylist(
	string userName, string playlistName)
{
	//  Business Rules
	//	These are processing rules that need to be satisfied 
	//		for valid data
	//		rule:	playlist name cannot be empty
	//		rule:	playlist must exist in the database 
	//					(will be handle on webpage).

	if (string.IsNullOrWhiteSpace(playlistName))
	{
		throw new ArgumentNullException("Playlist name is missing");
	}

	return PlaylistTracks
			.Where(x => x.Playlist.Name == playlistName)
			.Select(x => new PlaylistTrackView
			{
				TrackId = x.TrackId,
				SongName = x.Track.Name,
				TrackNumber = x.TrackNumber,
				Milliseconds = x.Track.Milliseconds
			}).OrderBy(x => x.TrackNumber)
			.ToList();
}