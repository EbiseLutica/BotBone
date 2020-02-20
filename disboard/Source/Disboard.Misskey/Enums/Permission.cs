using System;

namespace Disboard.Misskey.Enums
{
    [Flags]
    public enum Permission
    {
        AccountRead,
        AccountWrite,
        BlocksRead,
        BlocksWrite,
        DriveRead,
        DriveWrite,
        FavoritesRead,
        FavoriteWrite,
        FollowingRead,
        FollowingWrite,
        MessagingRead,
        MessagingWrite,
        MutesRead,
        MutesWrite,
        NoteWrite,
        NotificationsRead,
        NotificationWrite,
        ReactionsRead,
        ReactionWrite,
        VoteWrite,

        // old?
        AccountRead2,
        AccountWrite2
    }
}