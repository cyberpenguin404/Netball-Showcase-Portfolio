using System;
using Unity.Services.Lobbies.Models;

namespace PD4.LobbySystem.Model
{
	public class LobbyEventArgs: EventArgs
	{
		public LobbyEventArgs(Lobby lobby)
		{
			Lobby = lobby;
		}

		public Lobby Lobby { get; }
	}
}