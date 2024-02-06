using System.Collections.Generic;

using Hazel;
using UnityEngine;

using ExtremeRoles.Extension.UnityEvents;
using ExtremeRoles.Performance;
using ExtremeRoles.Module.CustomMonoBehaviour.UIPart;
using ExtremeRoles.Module.SystemType;
using ExtremeRoles.Module.Interface;

using Behavior = ExtremeRoles.Module.SystemType.RaiseHandSystem.Behavior;

namespace ExtremeRoles.Beta;

public sealed class RaiseHandSystemToggle : IRaiseHandSystem
{
    public bool IsInit => this.raiseHandButton != null;
    public const ExtremeSystemType Type = ExtremeSystemType.RaiseHandSystem;

    private readonly Dictionary<byte, Behavior> allHand = new Dictionary<byte, Behavior>();

    private HashSet<byte> raisedHand = new HashSet<byte>();
    private SimpleButton? raiseHandButton = null;

    public bool IsDirty { get; private set; }

    public void CreateRaiseHandButton()
    {
        this.raiseHandButton = Resources.Loader.CreateSimpleButton(
            MeetingHud.Instance.transform);

        this.raiseHandButton.Text.text = "挙手する";
        this.raiseHandButton.Text.fontSize =
            this.raiseHandButton.Text.fontSizeMax =
            this.raiseHandButton.Text.fontSizeMin = 2.0f;
        this.raiseHandButton.Scale = new Vector3(0.35f, 0.25f, 1.0f);
        this.raiseHandButton.Layer = 5;
        this.raiseHandButton.transform.localPosition = new Vector3(0.0f, -2.25f, -125f);

        this.raiseHandButton.ClickedEvent.AddListener(() =>
        {
            byte playerId = CachedPlayerControl.LocalPlayer.PlayerId;
            var tagetColor = this.raisedHand.Contains(playerId) ? Color.white : Color.yellow; ;

            this.raiseHandButton.DefaultImgColor = tagetColor;
            this.raiseHandButton.DefaultTextColor = tagetColor;

            ExtremeSystemTypeManager.RpcUpdateSystemOnlyHost(
                Type, (x) =>
                {
                    x.Write(playerId);
                });
        });
    }

    public void AddHand(PlayerVoteArea player)
    {
        this.allHand[player.TargetPlayerId] = new Behavior(player);
    }

    public void RaiseHandButtonSetActive(bool active)
    {
        if (this.raiseHandButton != null)
        {
            this.raiseHandButton.gameObject.SetActive(active);
        }
    }

    public void Deteriorate(float deltaTime)
    { }

    public void Reset(ResetTiming timing, PlayerControl? resetPlayer = null)
    {
        if (timing == ResetTiming.MeetingEnd)
        {
            this.allHand.Clear();
            this.raisedHand.Clear();
            this.raiseHandButton = null;
        }
    }

    public void Deserialize(MessageReader reader, bool initialState)
    {
        int readNum = reader.ReadPackedInt32();

        var newRaiseHand = new HashSet<byte>();
        for (int i = 0; i < readNum; ++i)
        {
            byte id = reader.ReadByte();
            newRaiseHand.Add(id);

            if (this.raisedHand.Add(id) &&
                this.allHand.TryGetValue(id, out var hand) &&
                hand != null)
            {
                hand.Raise();
            }
        }

        foreach (byte id in this.raisedHand)
        {
            if (!newRaiseHand.Remove(id) &&
                this.allHand.TryGetValue(id, out var hand) &&
                hand != null)
            {
                hand.Down();
            }
        }
    }

    public void Serialize(MessageWriter writer, bool initialState)
    {
        writer.WritePacked(this.raisedHand.Count);
        foreach (byte playerId in this.raisedHand)
        {
            writer.Write(playerId);
        }
        this.IsDirty = initialState;
    }

    public void UpdateSystem(PlayerControl player, MessageReader msgReader)
    {
        byte playerId = msgReader.ReadByte();
        this.handOps(playerId);
    }

    private void handOps(byte playerId)
    {
        if (!this.allHand.TryGetValue(playerId, out var hand) ||
            hand == null)
        {
            return;
        }

        if (this.raisedHand.Add(playerId))
        {
            hand.Raise();
        }
        else
        {
            hand.Down();
            this.raisedHand.Remove(playerId);
        }
        this.IsDirty = true;
    }
}
