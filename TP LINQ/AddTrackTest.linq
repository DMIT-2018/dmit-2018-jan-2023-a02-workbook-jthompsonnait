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
		//	coded and test the AddTack
		//		The command method will receive no collection but will receive
		//			individual arguments: userName, playlistName, trackID

		//	793	A Castle Full of Rascals
		//	822	A Twist In The Tail
		//	543	Burn
		//	756	Child in Time

		string userName = "HansenB";
		string playlistName = "Jan23A02";
		int trackId = 543;

		//	showing that both the playlist and track does not exist
		Console.WriteLine("Before adding track");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName).Dump();
		PlayListTrackServices_AddTrack(userName, playlistName, trackId);

		//	showing that both the playlist and track now exist
		Console.WriteLine("After adding track");
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

public void PlayListTrackServices_AddTrack(string userName, string playlistName, int trackId)
{
	//	create local variables
	//	Check to ensure that the track has not be removed from the catelog/libary
	bool trackExist = false;
	Playlists playlist = null;
	int trackNumber = 0;
	bool playlistTrackExist = false;
	PlaylistTracks playlistTrack = null;

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
	if (string.IsNullOrWhiteSpace(userName))
	{
		throw new ArgumentNullException("User name is missing");
	}

	if (string.IsNullOrWhiteSpace(playlistName))
	{
		throw new ArgumentNullException("Playlist name is missing");
	}
	#endregion

	//  check that the incoming data exist
	trackExist = Tracks
					.Where(x => x.TrackId == trackId)
					.Select(x => x.TrackId)
					.Any();
	if (!trackExist)
	{
		throw new ArgumentNullException("Selected track no longer is on the system.  Refresh track table");
	}

	//	Business Processing
	//	Check if the playlist exist.
	playlist = Playlists
				.Where(x => x.Name == playlistName
						&& x.UserName == userName)
				.FirstOrDefault();

	//  does not exist
	if (playlist == null)
	{
		playlist = new Playlists()
		{
			Name = playlistName,
			UserName = userName
		};
		//	stage (on in memory)
		Playlists.Add(playlist);
		trackNumber = 1;
	}
	else
	{
		//	playlist already exist
		//	rule:  unique tracks on the playlist
		playlistTrackExist = PlaylistTracks
							.Any(x => x.Playlist.Name == playlistName
								 && x.Playlist.UserName == userName
								 && x.TrackId == trackId);
		if (playlistTrackExist)
		{
			var songName = Tracks
							.Where(x => x.TrackId == trackId)
							.Select(x => x.Name)
							.FirstOrDefault();
			//	rule violation
			errorList.Add(new Exception($"Select track ({songName}) is already on the playlist"));
		}
		else
		{
			//  get the current track counts
			trackNumber = PlaylistTracks
							.Where(x => x.Playlist.Name == playlistName
									&& x.Playlist.UserName == userName)
							.Count();
			//  increment this by 1
			trackNumber++;
		}
	}

	//	add the track to the playlist
	//	create an instance for the playlist track

	playlistTrack = new PlaylistTracks();

	//	load values
	playlistTrack.TrackId = trackId;
	playlistTrack.TrackNumber = trackNumber;

	//	What about the second part of the primary key:  PlaylistId?
	//	If playlist exists, then we know the id: playlist.PlaylistId
	//	But if the playlist is NEW, we DO NOT KNOW the id.

	//	In the sitution of a NEW playlist, even though we have created the 
	//		playlist instance (see above) it is ONLY stage!!!
	//	This means that the actual SQL record has NOT yet been created.
	//	This means that the IDENTITY value for the new playlist DOES NOT
	//		yet exists.
	//	The value on the playlist instance (playlist) is zero (0)
	//		Therefor, we have a serious problem.

	//	Solution
	//	It is build into the Entity Framework software and is based using
	//		the navigational property in the Playlist pointing to it's "child"

	//	Staging a typical Add in the past was to reference the entity and use the
	//		entity.Add(xxx)
	//	If you use this statement, the playlistId would be zero (0)
	//		causing your transaction to ABORT
	//	Why?	PKeys cannot be zero (0) (FKey to Pkey problem)

	//	INSTEAD, do the staging using the "parent.navChildProperty.Add(xxx)
	playlist.PlaylistTracks.Add(playlistTrack);
	
	//	Staging is complete.
	//	Commit the work (Transaction)
	//	Committing the work needs a .SaveChanges()
	//	A transaction has ONLY ONE .SaveChanges()
	//	But, what if you have discoved errors during the business process???
	if (errorList.Count() > 0)
	{
		throw new AggregateException("Unable to add new track.  Check conerns",errorList);
	}
	else
	{
		//	consider data valid
		//	has passed business processing rules
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