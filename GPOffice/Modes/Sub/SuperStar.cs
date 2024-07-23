﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace GPOffice.SubModes
{
    class SuperStar
    {
        public static SuperStar Instance;

        Task Works;

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            Works = Task.WhenAll(
                OnModeStarted()
                );

            Exiled.Events.Handlers.Player.Left += OnLeft;
        }

        public void OnDisabled()
        {
            Works.Dispose();

            Exiled.Events.Handlers.Player.Left -= OnLeft;
        }

        public async Task OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                    if (!pl.Contains(player.UserId))
                    {
                        Server.ExecuteCommand($"/speak {player} enable");
                        pl.Add(player.UserId);
                    }

                await Task.Delay(1000);
            }
        }

        public void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (pl.Contains(ev.Player.UserId))
                pl.Remove(ev.Player.UserId);
        }
    }
}
