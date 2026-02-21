using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node
{
	List<AudioStreamPlayer> players = [];

	//singleton pringleton
	public static AudioManager Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
	}

	public void PlayStream(AudioStream stream)
	{
		//look for an existing unoccupied player to play the stream
		bool playerFound = false;
		foreach (var player in players)
		{
			if (!player.Playing)
			{
				player.Stream = stream;
				player.Play();

				playerFound = true;
				break;
			}
		}

		//if none are free, create a new one
		if (!playerFound)
		{
			AudioStreamPlayer player = new();
			AddChild(player);
			player.Stream = stream;
			player.Play();

			players.Add(player);
		}
	}
}
