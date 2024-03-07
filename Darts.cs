using Cichol.Game;
using pake.Network;
using System;

namespace Cichol.Mods
{
    class MartialDarts : Mod
    {
        private DartBoard board;
        private TurnInfo turn;
        private int[] combo;
        private int count = 0;
        private bool started = false;
        private int targetscore;
        private byte shot1;
        private byte shot2;
        private byte shot3;
        private bool autoShoot = false;

        public override bool OnCommand(string command)
        {
            if ("darts".Equals(command))
            {
                Config config = controller.GetConfig();
                config.darts = !config.darts;
                controller.whisperUserToggleResult("AUTO Darts", config.darts);
                return true;
            }
            else if ("auto darts".Equals(command))
            {
                autoShoot = true;
                controller.whisperUser("Cichol will auto shoot the darts. Must re-enable every login");
                return true;
            }
            return false;
        }

        public override void OnRequestMenu(Menu menu)
        {
            var enabled = controller.GetConfig().darts;
            menu.AddToggleButton(enabled, "Auto Darts", () => {
                ToggleDarts();
                controller.RefreshMenu();
            });
        }

        private void ToggleDarts()
        {
            Config config = controller.GetConfig();
            config.darts = !config.darts;
            config.SaveConfig();
        }

        public override bool OnPacketSent(Packet packet)
        {
            if (!controller.GetConfig().darts) return false;
            if (packet.Op == 0x211C8 && turn != null && turn.getCurrentPlayer() == packet.Id)
            {
                try
                {
                    Target target = board.score[combo[count]];
                    short x = turn.throwx(target);
                    short y = turn.throwy(target);

                    Packet editedThrow = new Packet(packet.Op, packet.Id);
                    editedThrow.PutLong(turn.dartID);
                    editedThrow.PutShort(x);  // Throw Darts x-coord
                    editedThrow.PutShort(y); // Throw Darts y-coord
                    controller.SendPacketToServer(editedThrow);
                    count++;
                    return true;
                }
                catch (Exception e)
                {
                    controller.whisperUser("Error: {0}", e);
                }

            }
            else if (packet.Op == 0x211B4)
            {  // Start packet for Martial Darts
                board = new DartBoard();
                started = false;
            }
            return false;
        }

        public override bool OnPacketRecieved(Packet packet)
        {
            if (controller.GetConfig().darts && packet.Op == 0x211C7)
            {
                turn = new TurnInfo(packet);
                targetscore = turn.getTargetScore();
                shot1 = turn.firstShot();
                shot2 = turn.secondShot();
                shot3 = turn.thirdShot();
                if (!started)
                {
                    count = 0;
                    combo = turn.computeScore(targetscore);
                    started = true;
                }
                else if (started && turn != null && turn.getCurrentPlayer() == controller.userId && autoShoot)
                {
                    if (shot1 == 1 || shot2 == 1 || shot3 == 1)
                    {
                        Shoot();
                        controller.whisperUser("Shooting");
                    }
                }

            }
            return false;
        }

        public void Shoot()
        {
            Target target = board.score[combo[count]];
            short x = turn.throwx(target);
            short y = turn.throwy(target);

            Packet throwedit = new Packet(0x211C9, controller.userId);
            throwedit.PutLong(turn.dartID);
            throwedit.PutShort(x);
            throwedit.PutShort(y);
            controller.SendPacketToServer(throwedit);

            Packet editedThrow = new Packet(0x211C8, controller.userId);
            editedThrow.PutLong(turn.dartID);
            editedThrow.PutShort(x);  // Throw Darts x-coord
            editedThrow.PutShort(y); // Throw Darts y-coord
            controller.SendPacketToServer(editedThrow);
            count++;
        }
    }

    public class Target
    {
        public short x;
        public short y;
    }

    public class DartBoard
    {
        public Target[] score = new Target[61];
        public DartBoard()
        {
            score[1] = new Target();
            score[1].x = 176;
            score[1].y = 150;

            score[2] = new Target();
            score[2].x = 183;
            score[2].y = 50;

            score[3] = new Target();
            score[3].x = 170;
            score[3].y = 88;

            score[4] = new Target();
            score[4].x = 219;
            score[4].y = 99;

            score[5] = new Target();
            score[5].x = 124;
            score[5].y = 69;

            score[6] = new Target();
            score[6].x = 151;
            score[6].y = 255;

            score[7] = new Target();
            score[7].x = 100;
            score[7].y = 217;

            score[8] = new Target();
            score[8].x = 70;
            score[8].y = 176;

            score[9] = new Target();
            score[9].x = 81;
            score[9].y = 99;

            score[10] = new Target();
            score[10].x = 118;
            score[10].y = 50;

            score[11] = new Target();
            score[11].x = 65;
            score[11].y = 150;

            score[12] = new Target();
            score[12].x = 100;
            score[12].y = 81;

            score[13] = new Target();
            score[13].x = 230;
            score[13].y = 124;

            score[14] = new Target();
            score[14].x = 68;
            score[14].y = 122;

            score[15] = new Target();
            score[15].x = 218;
            score[15].y = 199;

            score[16] = new Target();
            score[16].x = 81;
            score[16].y = 200;

            score[17] = new Target();
            score[17].x = 175;
            score[17].y = 228;

            score[18] = new Target();
            score[18].x = 200;
            score[18].y = 80;

            score[19] = new Target();
            score[19].x = 123;
            score[19].y = 229;

            score[20] = new Target();
            score[20].x = 249;
            score[20].y = 182;

            score[21] = new Target();
            score[21].x = 112;
            score[21].y = 201;

            score[22] = new Target();
            score[22].x = 45;
            score[22].y = 150;

            score[24] = new Target();
            score[24].x = 89;
            score[24].y = 170;

            score[26] = new Target();
            score[26].x = 249;
            score[26].y = 117;

            score[27] = new Target();
            score[27].x = 98;
            score[27].y = 112;

            score[28] = new Target();
            score[28].x = 49;
            score[28].y = 118;

            score[30] = new Target();
            score[30].x = 210;
            score[30].y = 169;

            score[32] = new Target();
            score[32].x = 65;
            score[32].y = 212;

            score[33] = new Target();
            score[33].x = 87;
            score[33].y = 150;

            score[34] = new Target();
            score[34].x = 182;
            score[34].y = 249;

            score[36] = new Target();
            score[36].x = 211;
            score[36].y = 64;

            score[38] = new Target();
            score[38].x = 117;
            score[38].y = 249;

            score[39] = new Target();
            score[39].x = 210;
            score[39].y = 131;

            score[40] = new Target();
            score[40].x = 150;
            score[40].y = 44;

            score[42] = new Target();
            score[42].x = 89;
            score[42].y = 130;

            score[45] = new Target();
            score[45].x = 201;
            score[45].y = 187;

            score[48] = new Target();
            score[48].x = 99;
            score[48].y = 187;

            score[50] = new Target();
            score[50].x = 150;
            score[50].y = 150;

            score[51] = new Target();
            score[51].x = 170;
            score[51].y = 209;

            score[54] = new Target();
            score[54].x = 187;
            score[54].y = 98;

            score[57] = new Target();
            score[57].x = 130;
            score[57].y = 210;

            score[60] = new Target();
            score[60].x = 150;
            score[60].y = 85;
        }
    }

    public class TurnInfo
    {
        public long dartID;
        private long currentPlayer;
        private int targetscore;
        private ushort bignumber_x;
        private ushort bignumber_y;
        private byte shot1;
        private byte shot2;
        private byte shot3;

        public TurnInfo(Packet packet)
        {
            dartID = packet.GetLong();
            currentPlayer = packet.GetLong();
            targetscore = packet.GetInt();
            bignumber_x = packet.GetUShort();
            bignumber_y = packet.GetUShort();
            packet.Skip(3);
            shot1 = packet.GetByte();
            packet.Skip(3);
            shot2 = packet.GetByte();
            packet.Skip(3);
            shot3 = packet.GetByte();
        }

        public int getTargetScore()
        {
            return targetscore;
        }

        public long getCurrentPlayer()
        {
            return currentPlayer;
        }

        public byte firstShot()
        {
            return shot1;
        }

        public byte secondShot()
        {
            return shot2;
        }

        public byte thirdShot()
        {
            return shot3;
        }

        public short throwx(Target t)
        {
            short wind_x = bignumber_x > (ushort)short.MaxValue ? (short)(bignumber_x - ushort.MaxValue) : (short)bignumber_x;
            short throw_x = (short)(t.x - wind_x);
            return throw_x;
        }

        public short throwy(Target t)
        {
            short wind_y = bignumber_y > (ushort)short.MaxValue ? (short)(bignumber_y - ushort.MaxValue) : (short)bignumber_y;
            short throw_y = (short)(t.y - wind_y);
            return throw_y;
        }

        public int[] computeScore(int targetscore)
        {
            int[] turn = new int[3];

            if ((targetscore - 120) % 2 == 0 ||
                (targetscore - 120) % 3 == 0 ||
                (targetscore - 120) <= 20)
            {
                turn[0] = 60;
                turn[1] = 60;
                turn[2] = targetscore - 120;
            }
            else if ((targetscore - 110) % 2 == 0 ||
                (targetscore - 110) % 3 == 0 ||
                (targetscore - 110) <= 20)
            {
                turn[0] = 50;
                turn[1] = 60;
                turn[2] = targetscore - 110;
            }
            else if ((targetscore - 100) % 2 == 0 ||
                (targetscore - 100) % 3 == 0 ||
                (targetscore - 100 <= 20))
            {
                turn[0] = 50;
                turn[1] = 50;
                turn[2] = targetscore - 100;
            }
            return turn;
        }
    }
}