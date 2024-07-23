﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace GPOffice.Modes
{
    class RedLightGreenLight
    {
        public static RedLightGreenLight Instance;

        CoroutineHandle timing_OnModeStarted;
        CoroutineHandle timing_RecordPlayerInfo;
        CoroutineHandle timing_CheckRedLight;

        public string Light = "Green";
        public Dictionary<Player, Vector3> PlayerPosition = new Dictionary<Player, Vector3>();
        public Dictionary<Player, Quaternion> PlayerRotation = new Dictionary<Player, Quaternion>();

        public void OnEnabled()
        {
            timing_OnModeStarted = Timing.RunCoroutine(OnModeStarted());
            timing_RecordPlayerInfo = Timing.RunCoroutine(RecordPlayerInfo());
            timing_CheckRedLight = Timing.RunCoroutine(CheckRedLight());
        }

        public void OnDisabled()
        {
            Timing.KillCoroutines(timing_OnModeStarted);
            Timing.KillCoroutines(timing_RecordPlayerInfo);
            Timing.KillCoroutines(timing_CheckRedLight);
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            Player.List.ToList().ForEach(x => x.ShowHint($"<color=green>초록 불</color>! 움직여도 됩니다.", 250));

            while (true)
            {
                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(1, 50));

                Light = "Red";
                Player.List.ToList().ForEach(x => x.ShowHint($"<color=red>빨간 불</color>! 움직이지 마세요!", 250));

                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(1, 5));

                Light = "Green";
                Player.List.ToList().ForEach(x => x.ShowHint($"<color=green>초록 불</color>! 움직여도 됩니다.", 250));
            }
        }

        public IEnumerator<float> RecordPlayerInfo()
        {
            yield return Timing.WaitForSeconds(10f);

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (PlayerPosition.ContainsKey(player))
                    {
                        PlayerPosition[player] = player.Position;
                        PlayerRotation[player] = player.Rotation;
                    }
                    else
                    {
                        PlayerPosition.Add(player, player.Position);
                        PlayerRotation.Add(player, player.Rotation);
                    }

                    yield return Timing.WaitForSeconds(0.1f);
                }
            }
        }

        public IEnumerator<float> CheckRedLight()
        {
            yield return Timing.WaitForSeconds(10f);

            while (true)
            {
                while (Light == "Green")
                    yield return Timing.WaitForSeconds(0.1f);

                yield return Timing.WaitForSeconds(1.5f);

                while (Light == "Red")
                {
                    foreach (var player in Player.List)
                    {
                        if (PlayerPosition.ContainsKey(player) && player.IsAlive)
                        {
                            if (PlayerPosition[player] != player.Position || PlayerRotation[player] != player.Rotation)
                            {
                                Server.ExecuteCommand($"/rocket {player.Id} 0.01");

                                Server.ExecuteCommand($"/clearcassie");
                                Server.ExecuteCommand($"/cassie_sl 안타깝게도 {player.DisplayNickname}(이)가 <b><color=red>빨간 불</color></b>에 건넜습니다.");
                            }
                        }
                    }

                    yield return Timing.WaitForSeconds(0.2f);
                }
            }
        }
    }
}
