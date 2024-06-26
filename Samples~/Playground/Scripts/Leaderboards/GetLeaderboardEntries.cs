﻿using System;
using TaloGameServices;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class GetLeaderboardEntries : MonoBehaviour
{
    public string internalName;
    public int page;

    public async void OnButtonClick()
    {
        await FetchEntries();
    }

    private async Task FetchEntries()
    {
        try
        {
            int score = UnityEngine.Random.Range(0, 10000);
            LeaderboardEntriesResponse res = await Talo.Leaderboards.GetEntries(internalName, page);
            LeaderboardEntry[] entries = res.entries;

            if (entries.Length == 0)
            {
                ResponseMessage.SetText($"No entries for page {page}");
            }
            else
            {
                ResponseMessage.SetText(string.Join(", ", entries.Select((e) => e.ToString()).ToArray()));
            }
        }
        catch (Exception err)
        {
            ResponseMessage.SetText(err.Message);
            throw err;
        }
    }
}
