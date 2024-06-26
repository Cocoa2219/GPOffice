﻿/* GPOffice (ver. Alpha 0.0.1) */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using GPOffice.Modes;
using MEC;

namespace GPOffice
{
    public class GPOffice : Plugin<Config>
    {
        public static GPOffice Instance;

        public List<string> Owner = new List<string>() { "76561198447505804@steam" };

        public static Dictionary<object, object> Mods = new Dictionary<object, object>()
        {
            {"로켓 런처", "FF8000/무슨 이유로든 피격당하면 승천합니다!"}, {"무제한", "3F13AB/말 그대로 제한이 사라집니다!"}, {"슈퍼 스타", "FE2EF7/모두의 마이크가 공유됩니다!"},
            {"뒤통수 얼얼", "DF0101/아군 공격이 허용됩니다!"}, {"스피드왜건", "FFBF00/모두의 속도가 최대값으로 올라가는 대신에\n최대 체력이 반으로 줄어듭니다!"},
            {"무덤", "000000/살아남으려면 뭐든지 해야 합니다."}, {"랜덤박스", "BFFF00/60초마다 랜덤한 아이템을 얻을 수 있습니다!"}, {"종이 인간", "FFFFFF/종이가 되어라!"}
        };
        public Dictionary<object, object> Players = new Dictionary<object, object>();

        public static object Mode = GetRandomValue(Mods.Keys.ToList());
        public string mod = Mode.ToString();

        public static object GetRandomValue(List<object> list)
        {
            System.Random random = new System.Random();
            int index = random.Next(0, list.Count);
            return list[index];
        }

        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;

            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Instance = null;
        }

        public void OnRoundStarted()
        {
            // 선택된 모드의 설명을 모두에게 띄워줍니다.
            Player.List.ToList().ForEach(x => x.Broadcast(5, $"<size=30>⌈<color=#{Mods[mod].ToString().Split('/')[0]}><b>{mod}</b></color>⌋</size>\n<size=25>{Mods[mod].ToString().Split('/')[1]}</size>"));

            if (mod == "로켓 런처")
            {
                RocketLauncher.Instance = new RocketLauncher();
                RocketLauncher.Instance.OnEnabled();
            }
            else if (mod == "무제한")
            {
                Unlimited.Instance = new Unlimited();
                Unlimited.Instance.OnEnabled();
            }
            else if (mod == "슈퍼 스타")
            {
                SuperStar.Instance = new SuperStar();
                SuperStar.Instance.OnEnabled();
            }
            else if (mod == "뒤통수 얼얼")
            {
                FriendlyFire.Instance = new FriendlyFire();
                FriendlyFire.Instance.OnEnabled();
            }
            else if (mod == "스피드왜건")
            {
                SpeedWagon.Instance = new SpeedWagon();
                SpeedWagon.Instance.OnEnabled();
            }
            else if (mod == "무덤")
            {
                Tomb.Instance = new Tomb();
                Tomb.Instance.OnEnabled();
            }
            else if (mod == "랜덤박스")
            {
                RandomItem.Instance = new RandomItem();
                RandomItem.Instance.OnEnabled();
            }
            else if (mod == "종이 인간")
            {
                PaperHuman.Instance = new PaperHuman();
                PaperHuman.Instance.OnEnabled();
            }

            // OnSpawned or OnChangingRole 이벤트 핸들
            foreach (var player in Player.List)
                player.Role.Set(player.Role);
        }

        public async void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            await Task.Delay(8000);
            Server.ExecuteCommand($"sr");
        }

        public async void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (Round.IsStarted)
                ev.Player.Broadcast(5, $"<size=30>⌈<color=#{Mods[mod].ToString().Split('/')[0]}><b>{mod}</b></color>⌋</size>\n<size=25>{Mods[mod].ToString().Split('/')[1]}</size>");

            else
            {
                string modes = string.Join(", ", Mods.Keys).Trim();
                int colorIndex = 0;

                while (!Round.IsStarted)
                {
                    string[] modeList = modes.Split(',');
                    StringBuilder coloredModes = new StringBuilder();

                    for (int i = 0; i < modeList.Length; i++)
                    {
                        if (i % 2 == colorIndex)
                        {
                            coloredModes.Append($"<color=yellow>{modeList[i]}</color>");
                        }
                        else
                        {
                            coloredModes.Append(modeList[i]);
                        }

                        if (i != modeList.Length - 1)
                        {
                            coloredModes.Append(", ");
                        }
                    }

                    ev.Player.ShowHint($"\n\n<align=left><b>아래 모드들 중 하나의 모드가 선택됩니다.</b>\n<size=20>{coloredModes}</size></align>", 3);
                    await Task.Delay(1000);
                    colorIndex = (colorIndex + 1) % 2;
                }

                ev.Player.ShowHint($"", 1);
            }
        }

    }
}

