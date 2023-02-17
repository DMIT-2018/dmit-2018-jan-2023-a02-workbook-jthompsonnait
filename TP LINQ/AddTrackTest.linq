<Query Kind="Program">
  <Connection>
    <ID>3f77383b-7ae5-4f03-8c46-ddbe5b1af50d</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>ChinookSept2018</Database>
    <DisplayName>ChinookSept2018</DisplayName>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

#load ".\ViewModels\*.cs"
using Chinook;
void Main()
{
	try
	{
		//	coded and test the AddTack
		//		The command method will receive no collection but will receive
		//			individual arguments: userName, playlistName, trackID

		//	793	A Castle Full of Rascals
		//	822	A Twist In The Tail
		//	543	Burn
		//	756	Child in Time

		string userName = "HansenB";
		string playlistName = "Jan23A02";
		int trackId = 783;

		//	showing that both the playlist and track does not exist
		Console.WriteLine("Before adding track");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName);
		PlayListTrackServices_AddTrack(userName, playlistName, trackId);

		//	showing that both the playlist and track now exist
		Console.WriteLine("After adding track");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName);
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

public void PlayListTrackServices_AddTrack(string userName, string playlistName, int trackId)
{
	//	create local variables
	//	Check to ensure that the track has not be removed from the catelog/libary
	bool trackExit = false;
	Playlists playlists = null;
	int trackNumber = 0;
	bool playlistTrackExist = false;
	PlaylistTracks playlistTracks = null;
	
	#region Business Logic and Parameter Exceptions
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();
	
	//	Buisness Rules
	//		rule:	a track can only exist once on a playlist
	//		rule:	each track on a playlist is assigned a continous track number
	//		rule:	playlist name cannot be empty
	//		rule:	track must exist in the tracks table
	
	//	If the business rules are passed, consider the data valid:
	//		a)	stage your tranaction work (Adds, Updates, Deletes)
	//		b)	execute a SINGLE .SaveChanges() - commits to database.
	
	//	paramter validation
	if(string.IsNullOrWhiteSpace(userName))
	{
		throw new ArgumentNullException("User name is missing");
	}

	if (string.IsNullOrWhiteSpace(playlistName))
	{
		throw new ArgumentNullException("Playlist name is missing");
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